using E = ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories.CompletedOrder
{
    public interface ICompletedOrderReadRepository : IReadRepository<E.CompletedOrder>
    {
    }
}
