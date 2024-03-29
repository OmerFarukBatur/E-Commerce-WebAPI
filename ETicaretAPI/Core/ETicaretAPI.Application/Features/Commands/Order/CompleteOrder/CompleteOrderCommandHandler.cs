﻿using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.Order;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.Order.CompleteOrder
{
    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommandRequest, CompleteOrderCommandResponse>
    {
        readonly IOrderService _orderService;
        readonly IMailService _mailService;

        public CompleteOrderCommandHandler(IOrderService orderService, IMailService mailService)
        {
            _orderService = orderService;
            _mailService = mailService;
        }

        public async Task<CompleteOrderCommandResponse> Handle([FromRoute]CompleteOrderCommandRequest request, CancellationToken cancellationToken)
        {
            (bool succeded,CompletedOrderDTO dto) = await _orderService.CompleteOrderAsync(request.Id);
            if (succeded)
            {
                await _mailService.SendCompletedOrderMailAsync(dto.To,dto.OrderCode, dto.OrderDate, dto.UserName);
            }
            return new();
        }
    }
}
