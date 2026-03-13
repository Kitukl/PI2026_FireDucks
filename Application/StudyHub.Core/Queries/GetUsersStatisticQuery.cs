using MediatR;
using StudyHub.Core.DTOs;

namespace StudyHub.Core.Queries;

public record GetUsersStatisticQuery() : IRequest<UsersStatisticDto>;