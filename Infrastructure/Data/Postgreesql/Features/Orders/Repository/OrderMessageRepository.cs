using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Features.Orders.Entities;
using Domain.Features.Orders.Repository;
using Infrastructure.Data.Postgreesql.Shared;

namespace Infrastructure.Data.Postgreesql.Features.Orders.Repository;

public class OrderMessageRepository :
    GenericRepository<Message>,
    IOrderMessageRepository
{
    public OrderMessageRepository(TILTContext context) : base(context)
    {
    }
}
