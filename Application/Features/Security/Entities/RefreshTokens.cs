using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Features.Security.Entities
{
    public class RefreshTokens
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime LastLogin { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}