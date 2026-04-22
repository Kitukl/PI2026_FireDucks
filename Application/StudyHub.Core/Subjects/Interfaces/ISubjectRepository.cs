using StudyHub.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.Core.Subjects.Interfaces
{
    public interface ISubjectRepository
    {
        Task<Subject?> GetById(Guid id);
        Task<List<Subject>> GetAll();
    }
}