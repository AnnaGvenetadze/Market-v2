using System.Security.Cryptography;
using System.Text;
using Market.DTO;
using Market.Services.Interfaces;
using Market.Services.Interfaces.Services;

namespace Market.Services
{
    public sealed class RefreshTokenService
    {
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenService(
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _tokenService = tokenService
                ?? throw new ArgumentNullException(nameof(tokenService));

            _unitOfWork = unitOfWork
                ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<RefreshTokenDTO> CreateAndSaveAsync(
            int accountId,
            CancellationToken cancellationToken = default)
        {
            if (accountId <= 0)
                throw new ArgumentOutOfRangeException(nameof(accountId));

            cancellationToken.ThrowIfCancellationRequested();

            var refreshToken =
                _tokenService.GenerateRefreshToken(accountId);

            byte[] tokenHash = SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshToken.Token));

            await _unitOfWork.AccountRepository.RefreshTokenAsync(
                accountId,
                tokenHash,
                refreshToken.ExpiredAt);

            return refreshToken;
        }
    }
}