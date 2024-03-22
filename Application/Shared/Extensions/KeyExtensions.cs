using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Application.Shared.Extensions;

public static class KeyExtensions
{
    /// <summary>
    /// Return key to store user's order.
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns></returns>
    public static string OrderUserKey(long userId) => $"order-user-{userId}";
}
