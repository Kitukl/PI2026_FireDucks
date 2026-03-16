using StudyHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.DTOs
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
