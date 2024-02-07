using Application.Features.Subscriptions.Contracts;
using Application.Features.Subscriptions.Queries.GetMemberById;
using Application.Shared.Abstractions;
using FluentResults;

namespace Application.Features.Subscriptions.Queries.GetSubscriptionById
{
    public class GetSubscriptionByIdQueryHandler
        : IQueryHandler<GetSubscriptionByIdQuery, SubscriptionResponse>
    {

        public GetSubscriptionByIdQueryHandler()
        {

        }

        public Task<Result<SubscriptionResponse>> Handle(
            GetSubscriptionByIdQuery request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}