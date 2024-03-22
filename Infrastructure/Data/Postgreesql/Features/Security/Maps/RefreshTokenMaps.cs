using Infrastructure.Data.Postgreesql.Features.Security.Entities;
using ApplicationRefreshToken = Application.Features.Security.Contracts.RefreshTokens;

namespace Infrastructure.Data.Postgreesql.Features.Security.Maps
{
    public static class RefreshTokenMaps
    {
        public static RefreshTokens ToPersistanceRefreshToken(this ApplicationRefreshToken refreshToken)
        {
            RefreshTokens pRefreshToken = new RefreshTokens
            {
                UserId = refreshToken.UserId,
                RefreshToken = refreshToken.RefreshToken,
                LastLogin = refreshToken.LastLogin
            };

            return pRefreshToken;
        }

        public static ApplicationRefreshToken ToAppRefreshToken(this RefreshTokens refreshToken)
        {
            ApplicationRefreshToken pRefreshToken = new ApplicationRefreshToken
            {
                UserId = refreshToken.UserId,
                RefreshToken = refreshToken.RefreshToken,
                LastLogin = refreshToken.LastLogin
            };

            return pRefreshToken;
        }
    }
}