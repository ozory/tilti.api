using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data.Postgreesql.Shared.Abstractions;

public interface IMapDomain<TSource, TDestination>
{
    TSource ToDomain(TDestination source);
    TDestination ToRepository(TSource source);
}
