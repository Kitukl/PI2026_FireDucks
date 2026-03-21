using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.DTOs
{
    public class ScheduleHeadmanRightsUpdateDtoRequest
    {
        public Guid Id { get; set; }
        public bool CanHeadmanUpdate { get; set; }
    }
}
