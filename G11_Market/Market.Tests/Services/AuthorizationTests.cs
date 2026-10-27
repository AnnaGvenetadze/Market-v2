using Market.DTO;
using Market.Services;
using Market.Services.Enums;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Repositories;
using Market.Services.Interfaces.Services;
using Moq;
using Serilog;

namespace Market.Tests.Services;

[TestFixture]
public class AuthorizationTests
{
    private static IEmployeeService CreateProxy(
        bool isAuthenticated,
        bool isAdministrator,
        IUnitOfWork unitOfWork)
    {
        var userContext = new Mock<ICurrentUserContext>();

        userContext.Setup(x => x.IsAuthenticated)
            .Returns(isAuthenticated);

        userContext.Setup(x => x.HasRole(Role.Administrator))
            .Returns(isAdministrator);

        var logger = new LoggerConfiguration()
            .CreateLogger();

        var authorizationService = new AuthorizationService(
            userContext.Object,
            logger);

        var employeeService = new EmployeeService(
            unitOfWork,
            logger);

        return AuthorizationProxy<IEmployeeService>.Wrap(
            employeeService,
            authorizationService);
    }

    [Test]
    public void GetById_WithoutAttribute_WhenNotAuthenticated_ShouldThrow()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateProxy(
            false,
            false,
            unitOfWork.Object);

        Assert.Throws<UnauthorizedAccessException>(() =>
            service.GetById(1));

        unitOfWork.VerifyNoOtherCalls();
    }

    [Test]
    public void GetProductById_WithoutAttribute_WhenAuthenticated_ShouldExecute()
    {
        var userContext = new Mock<ICurrentUserContext>();
        var productRepository = new Mock<IProductRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userContext
            .Setup(x => x.IsAuthenticated)
            .Returns(true);

        var product = new ProductDTO { Id = 1 };

        productRepository
            .Setup(x => x.GetById(1))
            .Returns(product);

        unitOfWork
            .SetupGet(x => x.ProductRepository)
            .Returns(productRepository.Object);

        using var logger = new LoggerConfiguration().CreateLogger();

        var authorizationService = new AuthorizationService(
            userContext.Object,
            logger);

        var productService = new ProductService(
            unitOfWork.Object,
            logger);

        var service = AuthorizationProxy<IProductService>.Wrap(
            productService,
            authorizationService);

        var result = service.GetProductById(1);

        Assert.That(result, Is.SameAs(product));

        productRepository.Verify(
            x => x.GetById(1),
            Times.Once);

        userContext.Verify(
            x => x.HasRole(It.IsAny<Role>()),
            Times.Never);
    }

    [Test]
    public void ChangeRole_Cache_ShouldReuseRequiredRoles()
    {
        var employeeRepository = new Mock<IEmployeeRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        employeeRepository
            .Setup(x => x.GetRoles(2))
            .Returns(new[]
            {
            new RoleDTO { Id = 2 }
            });

        employeeRepository
            .Setup(x => x.GetRoles(3))
            .Returns(new[]
            {
            new RoleDTO { Id = 1 }
            });

        unitOfWork
            .SetupGet(x => x.EmployeeRepository)
            .Returns(employeeRepository.Object);

        var service = CreateProxy(
            true,
            true,
            unitOfWork.Object);

        Assert.DoesNotThrow(() =>
            service.ChangeRole(2, 3));

        Assert.DoesNotThrow(() =>
            service.ChangeRole(3, 2));

        employeeRepository.Verify(
            x => x.GetRoles(2),
            Times.Once);

        employeeRepository.Verify(
            x => x.GetRoles(3),
            Times.Once);

        unitOfWork.Verify(
            x => x.Commit(),
            Times.Exactly(2));
    }

    [TestCase(false, false)]
    [TestCase(true, false)]
    public void CreateEmployee_WithoutPermission_ShouldThrow(
        bool isAuthenticated,
        bool isAdministrator)
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateProxy(
            isAuthenticated,
            isAdministrator,
            unitOfWork.Object);

        Assert.Throws<UnauthorizedAccessException>(() =>
            service.CreateEmployee(null!));

        unitOfWork.VerifyNoOtherCalls();
    }

    [TestCase(false, false)]
    [TestCase(true, false)]
    public void ChangeRole_WithoutPermission_ShouldThrow(
        bool isAuthenticated,
        bool isAdministrator)
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateProxy(
            isAuthenticated,
            isAdministrator,
            unitOfWork.Object);

        Assert.Throws<UnauthorizedAccessException>(() =>
            service.ChangeRole(0, 1));

        unitOfWork.VerifyNoOtherCalls();
    }

    [Test]
    public void CreateEmployee_AsAdministrator_ShouldReachService()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateProxy(
            true,
            true,
            unitOfWork.Object);

        Assert.Throws<ArgumentNullException>(() =>
            service.CreateEmployee(null!));
    }

    [Test]
    public void ChangeRole_AsAdministrator_ShouldReachService()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        var service = CreateProxy(
            true,
            true,
            unitOfWork.Object);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.ChangeRole(0, 1));
    }

    [Test]
    public void ChangeRole_AsAdministrator_ShouldExecuteSuccessfully()
    {
        var employeeRepository = new Mock<IEmployeeRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        employeeRepository
            .Setup(x => x.GetRoles(10))
            .Returns(new[]
            {
                new RoleDTO { Id = 1 }
            });

        unitOfWork
            .SetupGet(x => x.EmployeeRepository)
            .Returns(employeeRepository.Object);

        var service = CreateProxy(
            true,
            true,
            unitOfWork.Object);

        Assert.DoesNotThrow(() => service.ChangeRole(10, 2));

        employeeRepository.Verify(x =>
            x.UnassignRole(It.Is<EmployeeRoleDTO>(r =>
                r.EmployeeId == 10 && r.RoleId == 1)),
            Times.Once);

        employeeRepository.Verify(x =>
            x.AssignRole(It.Is<EmployeeRoleDTO>(r =>
                r.EmployeeId == 10 && r.RoleId == 2)),
            Times.Once);

        unitOfWork.Verify(x => x.Commit(), Times.Once);

        unitOfWork.Verify(x => x.Rollback(), Times.Never);
    }
}