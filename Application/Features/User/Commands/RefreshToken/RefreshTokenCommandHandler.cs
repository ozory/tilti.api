using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Security.Entities;
using Application.Features.User.Contracts;
using Application.Shared.Abstractions;
using Domain.Features.User.Repository;
using FluentResults;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.User.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
    {

        private readonly IUserRepository _repository;
        private readonly ISecurityRepository _securityRepository;
        private readonly ISecurityExtensions _securityExtensions;
        private readonly IValidator<RefreshTokenCommand> _validator;

        public RefreshTokenCommandHandler(
            ISecurityRepository securityRepository,
            ISecurityExtensions securityExtensions,
            IUserRepository repository,
            IValidator<RefreshTokenCommand> validator)
        {
            _securityRepository = securityRepository;
            _securityExtensions = securityExtensions;
            _repository = repository;
            _validator = validator;
        }

        public async Task<Result<AuthenticationResponse>> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {

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
    }
}