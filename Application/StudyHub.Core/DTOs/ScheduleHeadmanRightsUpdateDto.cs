using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.DTOs
{
    public class ScheduleHeadmanRightsUpdateDto
    {
        public Guid Id { get; set; }
        public bool CanHeadmanUpdate { get; set; }
    }
}
