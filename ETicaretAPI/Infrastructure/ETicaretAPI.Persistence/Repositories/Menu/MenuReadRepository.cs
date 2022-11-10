using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Application.Repositories.Menu;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.Menu
{
    public class MenuReadRepository : ReadRepository<ETicaretAPI.Domain.Entities.Menu>, IMenuReadRepository
    {
        public MenuReadRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
