﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services
{
    public interface IProductService
    {
        Task<byte[]> QrCodeToProductsAsync(string productId);
        Task StokUpdateToProductAsync(string productId, int stock);
    }
}
