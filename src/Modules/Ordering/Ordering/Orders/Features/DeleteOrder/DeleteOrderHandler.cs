﻿using Ordering.Data.Repository;

namespace Ordering.Orders.Features.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId)
    : ICommand<DeleteOrderResult>;
public record DeleteOrderResult(bool IsSuccess);
public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
{
    public DeleteOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderName is required");
    }
}

internal class DeleteOrderHandler(IOrderingUnitOfWork unitOfWork)
    : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
{
    public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId, cancellationToken);

        if (order is null)
        {
            throw new OrderNotFoundException(command.OrderId);
        }

        unitOfWork.Orders.Remove(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new DeleteOrderResult(true);
    }
}
