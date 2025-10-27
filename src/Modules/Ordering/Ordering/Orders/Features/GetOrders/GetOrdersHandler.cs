﻿using Ordering.Data.Repository;
using Shared.Pagination;

namespace Ordering.Orders.Features.GetOrders;

public record GetOrdersQuery(PaginationRequest PaginationRequest)
    : IQuery<GetOrdersResult>;
public record GetOrdersResult(PaginatedResult<OrderDto> Orders);

internal class GetOrdersHandler(IOrderingUnitOfWork unitOfWork)
    : IQueryHandle<GetOrdersQuery, GetOrdersResult>
{
    public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;

        var (orders, totalCount) = await unitOfWork.Orders.GetOrdersAsync(
            pageIndex, 
            pageSize, 
            null, 
            cancellationToken);

        var orderDtos = orders.ToList().Adapt<List<OrderDto>>();

        return new GetOrdersResult(
            new PaginatedResult<OrderDto>(
                pageIndex,
                pageSize,
                totalCount,
                orderDtos));
    }
}
