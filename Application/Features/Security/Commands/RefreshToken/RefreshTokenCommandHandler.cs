using Application.Features.Security.Contracts;
using Application.Features.Users.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.Users.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
    {

        private readonly IUserRepository _repository;
        private readonly ISecurityRepository _securityRepository;
        private readonly ISecurityExtensions _securityExtensions;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly IValidator<RefreshTokenCommand> _validator;
        private readonly string className = nameof(RefreshTokenCommandHandler);

        public RefreshTokenCommandHandler(
            ISecurityRepository securityRepository,
            ISecurityExtensions securityExtensions,
            IUserRepository repository,
            IValidator<RefreshTokenCommand> validator,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _securityRepository = securityRepository;
            _securityExtensions = securityExtensions;
            _repository = repository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<AuthenticationResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            try
            {

                _logger.LogInformation("[{className}] Validando usuário {Email}", className, request);

                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid) return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage));

                var principal = _securityExtensions.GetPrincipalFromExpiredToken(request.Token);
                if (principal is null) return Result.Fail("Principal not found");

                var id = principal.Claims.FirstOrDefault(x => x.Type.Contains("NameIdentifier", StringComparison.CurrentCultureIgnoreCase));
                if (id is null) return Result.Fail("Claim id not found");

                var refreshToken = await _securityRepository.GetRefreshTokenAsync(request.RefreshToken);
                if (refreshToken is null) return Result.Fail("RefreshToken not found");

                var user = await _repository.GetByIdAsync(long.Parse(id!.Value));
                if (user is null) return Result.Fail("User not found");

                // Se foi tudo certo até aqui começo a criar o novo Token
                RefreshTokens rf = new()
                {
                    UserId = user.Id,
                    RefreshToken = _securityExtensions.GenerateRefreshToken(),
                    LastLogin = refreshToken.LastLogin
                };

                var authenticateResponse = new AuthenticationResponse(
                    user.Id,
                    user.Name.Value,
                    user.Email.Value,
                    _securityExtensions.GenerateToken(user),
                    rf.RefreshToken);

                await _securityRepository.DeleteToken(user.Id);
                await _securityRepository.SaveToken(rf);

                return Result.Ok(authenticateResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{className}] Authentication Error : {request} Error: {ex}", className, request, ex);
                throw;
            }
        }
    }
}