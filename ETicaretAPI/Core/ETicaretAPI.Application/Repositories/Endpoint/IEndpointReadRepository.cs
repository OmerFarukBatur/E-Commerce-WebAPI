﻿using E = ETicaretAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Repositories.Endpoint
{
    public interface IEndpointReadRepository : IReadRepository<E.Endpoint>
    {
    }
}
