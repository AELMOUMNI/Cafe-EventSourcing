﻿using Cafe.Api.Resources;
using Cafe.Core.AuthContext;
using Cafe.Core.OrderContext.Commands;
using Cafe.Core.OrderContext.Queries;
using Cafe.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cafe.Api.Controllers
{
    public class OrderController : ApiController
    {
        public OrderController(IResourceMapper resourceMapper, IMediator mediator)
            : base(resourceMapper, mediator)
        {
        }

        /// <summary>
        /// Retrieves a to-go order by id.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder([FromRoute] Guid id) =>
            (await Mediator.Send(new GetToGoOrder { Id = id }))
                .Match(Ok, NotFound);

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllOrders() =>
            (await Mediator.Send(new GetAllToGoOrders()))
                .Match(Ok, Error);

        /// <summary>
        /// Retrieves orders for a given status.
        /// </summary>
        [HttpGet("{status}")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByStatus(ToGoOrderStatus status) =>
            (await Mediator.Send(new GetOrdersByStatus { Status = status }))
                .Match(Ok, Error);

        /// <summary>
        /// Opens a new to-go order.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = AuthConstants.Policies.IsAdminOrCashier)]
        public async Task<IActionResult> OrderToGo([FromBody] OrderToGo command) =>
            (await Mediator.Send(command))
                .Match(Ok, Error);

        /// <summary>
        /// Confirms a to-go order.
        /// </summary>
        [HttpPut("confirm")]
        [Authorize(Policy = AuthConstants.Policies.IsAdminOrCashier)]
        public async Task<IActionResult> ConfirmToGoOrder([FromBody] ConfirmToGoOrder command) =>
            (await Mediator.Send(command))
                .Match(Ok, Error);

        /// <summary>
        /// Completes a to-go order.
        /// </summary>
        [HttpPut("complete")]
        [Authorize(Policy = AuthConstants.Policies.IsAdminOrBarista)]
        public async Task<IActionResult> CompleteToGoOrder([FromBody] CompleteToGoOrder command)
        {
            command.BaristaId = BaristaId;

            var result = (await Mediator.Send(command))
                .Match(Ok, Error);

            return result;
        }
    }
}
