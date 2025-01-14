﻿using MediatR;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Orders.Application.Payments.RequestingPayment;

public class RequestPaymentHandler : ICommandHandler<RequestPayment>
{
    private readonly IIntegrationHttpService _integrationHttpService;

    public RequestPaymentHandler(IIntegrationHttpService integrationHttpService)
    {
        _integrationHttpService = integrationHttpService;
    }

    public async Task<Unit> Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        var response = await _integrationHttpService.PostAsync(
            "api/payments",
            new PaymentRequest(
            command.CustomerId.Value,
            command.OrderId.Value,
            command.TotalPrice.Amount,
            command.Currency.Code));

        if (response is null || !response!.Success)
            throw new ApplicationLogicException($"An error occurred requesting payment for order {command.OrderId}.");

        return Unit.Value;
    }
}

public record class PaymentRequest(
    Guid CustomerId,
    Guid OrderId,
    decimal TotalAmount,
    string currencyCode);
