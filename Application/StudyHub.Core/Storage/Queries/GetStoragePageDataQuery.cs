using MediatR;
using Application.Models;

namespace StudyHub.Core.Storage.Queries;

public class GetStoragePageDataQuery : IRequest<StoragePageViewModel>
{
    public Guid UserId { get; set; }
}