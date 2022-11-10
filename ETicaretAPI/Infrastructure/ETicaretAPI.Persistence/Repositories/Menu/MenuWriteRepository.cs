using ETicaretAPI.Application.Repositories.Menu;
using ETicaretAPI.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Repositories.Menu
{
    public class MenuWriteRepository : WriteRepository<ETicaretAPI.Domain.Entities.Menu>, IMenuWriteRepository
    {
        public MenuWriteRepository(ETicaretAPIDbContext context) : base(context)
        {
        }
    }
}
