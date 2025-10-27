﻿using Basket.Basket.Exceptions;
using Basket.Data.Repository;

namespace Basket.Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;

public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketHandler(IBasketUnitOfWork unitOfWork) : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
       var result = await unitOfWork.Baskets.DeleteBasketByUserNameAsync(command.UserName, cancellationToken);
       
       if (result)
       {
           await unitOfWork.SaveChangesAsync(cancellationToken);
       }
        
        return new DeleteBasketResult(result);
    }
}