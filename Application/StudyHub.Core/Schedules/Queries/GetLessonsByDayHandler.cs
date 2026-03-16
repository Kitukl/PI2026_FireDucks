using MediatR;
using StudyHub.Core.DTOs;
using StudyHub.Core.Schedule.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Schedules.Queries
{
    public record GetLessonsByDayRequest(ScheduleDayDto sdd): IRequest<List<LessonDto>>;

    public class GetLessonsByDayHandler: IRequestHandler<GetLessonsByDayRequest, List<LessonDto>>
    {
        private readonly IScheduleRepository _repo;

        public GetLessonsByDayHandler(IScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<LessonDto>> Handle(GetLessonsByDayRequest request, CancellationToken cancellationToken)
        {
            return await _repo.GetLessonsByDay(request.sdd.Id, request.sdd.Day);
        }
    }
}
