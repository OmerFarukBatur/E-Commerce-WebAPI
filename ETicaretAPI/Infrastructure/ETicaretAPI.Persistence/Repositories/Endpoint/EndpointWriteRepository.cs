using ETicaretAPI.Application.Repositories.Endpoint;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.Endpoint
{
    public class EndpointWriteRepository : WriteRepository<ETicaretAPI.Domain.Entities.Endpoint>, IEndpointWriteRepository
    {
        public EndpointWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
