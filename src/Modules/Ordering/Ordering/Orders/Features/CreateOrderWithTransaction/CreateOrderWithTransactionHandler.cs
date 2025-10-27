using Ordering.Data.Repository;

namespace Ordering.Orders.Features.CreateOrderWithTransaction;

public record CreateOrderWithTransactionCommand(OrderDto Order)
    : ICommand<CreateOrderWithTransactionResult>;
public record CreateOrderWithTransactionResult(Guid Id);

/// <summary>
/// Example handler showing how to use UnitOfWork with transactions.
/// This demonstrates the power of the Repository and UnitOfWork patterns.
/// </summary>
internal class CreateOrderWithTransactionHandler(IOrderingUnitOfWork unitOfWork)
    : ICommandHandler<CreateOrderWithTransactionCommand, CreateOrderWithTransactionResult>
{
    public async Task<CreateOrderWithTransactionResult> Handle(CreateOrderWithTransactionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Begin transaction
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            // Create order
            var order = CreateNewOrder(command.Order);
            await unitOfWork.Orders.AddAsync(order, cancellationToken);

            // You could add more operations here that need to be in the same transaction
            // For example: updating inventory, creating audit logs, etc.
            
            // Save changes within transaction
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Commit transaction
            await unitOfWork.CommitTransactionAsync(cancellationToken);

            return new CreateOrderWithTransactionResult(order.Id);
        }
        catch
        {
            // Rollback on any error
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private Order CreateNewOrder(OrderDto orderDto)
    {
        var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);

        var newOrder = Order.Create(
                id: Guid.NewGuid(),
                customerId: orderDto.CustomerId,
                orderName: $"{orderDto.OrderName}_{new Random().Next()}",
                shippingAddress: shippingAddress,
                billingAddress: billingAddress,
                payment: Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod)
                );

        orderDto.Items.ForEach(item =>
        {
            newOrder.Add(
                item.ProductId,
                item.Quantity,
                item.Price);
        });

        return newOrder;
    }
}
