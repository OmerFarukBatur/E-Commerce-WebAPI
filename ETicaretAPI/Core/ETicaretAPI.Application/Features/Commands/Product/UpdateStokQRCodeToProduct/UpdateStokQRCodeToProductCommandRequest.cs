using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Product.UpdateStokQRCodeToProduct
{
    public class UpdateStokQRCodeToProductCommandRequest : IRequest<UpdateStokQRCodeToProductCommandResponse>
    {
        public string ProductId { get; set; }
        public int Stock { get; set; }
    }
}