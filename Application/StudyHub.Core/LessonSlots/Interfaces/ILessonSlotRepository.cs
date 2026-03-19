using StudyHub.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyHub.Core.LessonSlots.Interfaces
{
    public interface ILessonSlotRepository
    {
        Task<LessonSlotDto?> GetById(Guid id);
        Task<List<LessonSlotDto?>> GetAll();
        Task AddLessonSlot(LessonSlotDto lesson);
        Task DeleteLessonSlot(Guid id);
        Task UpdateLessonSlot(LessonSlotDto lesson);
    }
}