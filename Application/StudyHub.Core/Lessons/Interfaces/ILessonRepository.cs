using StudyHub.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Lessons.Interfaces
{
    public interface ILessonRepository
    {
        Task<LessonDto?> GetById(Guid id);
        Task<List<LessonDto?>> GetAll();
        Task AddLesson(LessonDto lesson);
        Task DeleteLesson(Guid id);
        Task UpdateLesson(LessonDto lesson);
    }
}