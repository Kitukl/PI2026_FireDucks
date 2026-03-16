using StudyHub.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Subjects.Interfaces
{
    public interface ISubjectRepository
    {
        Task<SubjectDto?> GetById(Guid id);
        Task<List<SubjectDto?>> GetAll();
        Task AddSubject(SubjectDto lesson);
        Task DeleteSubject(Guid id);
        Task UpdateSubject(SubjectDto lesson);
    }
}