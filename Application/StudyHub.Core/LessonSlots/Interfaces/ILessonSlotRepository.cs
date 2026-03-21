using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.LessonSlots.Interfaces
{
    public interface ILessonSlotRepository
    {
        Task<LessonsSlot?> GetById(Guid id);
        Task<List<LessonsSlot>> GetAll();
        Task AddLessonSlot(LessonsSlot lesson);
        Task DeleteLessonSlot(Guid id);
        Task UpdateLessonSlot(LessonsSlot lesson);
    }
}