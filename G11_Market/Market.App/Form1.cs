using Market.Repositories;
using Market.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Market.App;

public partial class Form1 : Form
{
    private readonly TextBox _username = new();
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };
    private readonly TextBox _employeeId = new();
    private readonly TextBox _roleId = new();
    private readonly Button _changeRoleButton = new() { Text = "Invoke ChangeRole via proxy" };

    public Form1()
    {
        InitializeComponent();

        Text = "Manual ChangeRole authorization check";
        ClientSize = new Size(580, 300);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16),
            ColumnCount = 2,
            RowCount = 6
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 6; i++)
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6));

        AddInput(layout, 0, "Username", _username);
        AddInput(layout, 1, "Password", _password);
        AddInput(layout, 2, "Target employee ID", _employeeId);
        AddInput(layout, 3, "New role ID", _roleId);

        _changeRoleButton.Dock = DockStyle.Fill;
        _changeRoleButton.Click += async (_, _) => await ChangeRoleAsync();
        layout.Controls.Add(_changeRoleButton, 0, 4);
        layout.SetColumnSpan(_changeRoleButton, 2);

        var warning = new Label
        {
            Text = "Use a test database: an authorized call really changes an employee's role.",
            AutoSize = true,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };
        layout.Controls.Add(warning, 0, 5);
        layout.SetColumnSpan(warning, 2);

        Controls.Add(layout);
    }

    private static void AddInput(TableLayoutPanel layout, int row, string label, TextBox input)
    {
        layout.Controls.Add(new Label
        {
            Text = label,
            AutoSize = true,
            Anchor = AnchorStyles.Left
        }, 0, row);

        input.Dock = DockStyle.Fill;
        layout.Controls.Add(input, 1, row);
    }

    private async Task ChangeRoleAsync()
    {
        if (string.IsNullOrWhiteSpace(_username.Text) ||
            string.IsNullOrWhiteSpace(_password.Text) ||
            !int.TryParse(_employeeId.Text, out int employeeId) || employeeId <= 0 ||
            !int.TryParse(_roleId.Text, out int roleId) || roleId <= 0)
        {
            MessageBox.Show("Enter login credentials and positive employee/role IDs.");
            return;
        }

        _changeRoleButton.Enabled = false;
        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string connectionString = configuration.GetConnectionString("MarketDb")
                ?? throw new InvalidOperationException("MarketDb connection string is missing.");

            string databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
            if (!databaseName.Contains("Test", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("This manual check only permits a test database.");

            var confirmation = MessageBox.Show(
                $"Database: {databaseName}\nChange employee {employeeId} to role {roleId}?",
                "Confirm real database change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation != DialogResult.Yes)
                return;

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var unitOfWork = UnitOfWorkFactory.Create(connection);
            try
            {
                using var logger = new LoggerConfiguration().CreateLogger();
                var tokenService = new TokenService();
                var refreshTokenService = new RefreshTokenService(tokenService, unitOfWork);
                var authService = new AuthService(unitOfWork, tokenService, refreshTokenService);

                var loginResult = await authService.Login(_username.Text, _password.Text);
                if (!loginResult.IsSuccess || string.IsNullOrWhiteSpace(loginResult.AccessToken))
                {
                    MessageBox.Show($"Login failed: {loginResult.Message}");
                    return;
                }

                var employeeService = ManualProxyFactory.CreateEmployeeService(
                    unitOfWork,
                    tokenService,
                    loginResult.AccessToken,
                    logger);

                // This goes through AuthorizationProxy<IEmployeeService>, not EmployeeService directly.
                employeeService.ChangeRole(employeeId, roleId);

                MessageBox.Show("Authorized: ChangeRole completed and committed.");
            }
            finally
            {
                (unitOfWork as IDisposable)?.Dispose();
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            MessageBox.Show($"Access denied by authorization: {ex.Message}",
                "Authorization", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Execution failed",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _changeRoleButton.Enabled = true;
        }
    }
}
