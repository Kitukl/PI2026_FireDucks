using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Lecturers.Interfaces
{
    public interface ILecturerRepository
    {
        Task<Lecturer?> GetById(Guid id);
        Task<List<Lecturer>> GetAll();
        Task AddLecturer(Lecturer lesson);
        Task DeleteLecturer(Guid id);
        Task UpdateLecturer(Lecturer lesson);
    }
}