using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using Domain.Features.Orders.Entities;


namespace Domain.Features.Orders.Repository;

public interface IRateRepository : IGenericRepository<Rate>
{

}
