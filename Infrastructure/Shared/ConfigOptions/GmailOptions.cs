using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Shared.ConfigOptions;

public class GmailOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Host))
            throw new ArgumentException("Host cannot be null or empty.", nameof(Host));
        if (Port <= 0)
            throw new ArgumentOutOfRangeException(nameof(Port), "Port must be a positive number.");
        if (string.IsNullOrEmpty(Email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(Email));
        if (string.IsNullOrEmpty(Password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(Password));
    }
}
