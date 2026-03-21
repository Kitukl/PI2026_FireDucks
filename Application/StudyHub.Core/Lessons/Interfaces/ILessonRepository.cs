using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Lessons.Interfaces
{
    public interface ILessonRepository
    {
        Task<Lesson?> GetById(Guid id);
        Task<List<Lesson>> GetAll();
        Task AddLesson(Lesson lesson);
        Task DeleteLesson(Guid id);
        Task UpdateLesson(Lesson lesson);
    }
}