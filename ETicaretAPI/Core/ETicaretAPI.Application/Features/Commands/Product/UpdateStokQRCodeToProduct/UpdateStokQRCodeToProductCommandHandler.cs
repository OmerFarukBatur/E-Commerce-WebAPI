using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateStokQRCodeToProduct
{
    public class UpdateStokQRCodeToProductCommandHandler : IRequestHandler<UpdateStokQRCodeToProductCommandRequest, UpdateStokQRCodeToProductCommandResponse>
    {
        readonly IProductService _productService;

        public UpdateStokQRCodeToProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<UpdateStokQRCodeToProductCommandResponse> Handle(UpdateStokQRCodeToProductCommandRequest request, CancellationToken cancellationToken)
        {
            await _productService.StokUpdateToProductAsync(request.ProductId, request.Stock);

            return new();
        }
    }
}
