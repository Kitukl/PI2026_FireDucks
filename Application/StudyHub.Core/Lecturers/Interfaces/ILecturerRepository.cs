using StudyHub.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.Lecturers.Interfaces
{
    public interface ILecturerRepository
    {
        Task<LecturerDto?> GetById(Guid id);
        Task<List<LecturerDto?>> GetAll();
        Task AddLecturer(LecturerDto lesson);
        Task DeleteLecturer(Guid id);
        Task UpdateLecturer(LecturerDto lesson);
    }
}