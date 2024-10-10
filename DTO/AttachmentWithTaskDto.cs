using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = PmsApi.Models.Task;

namespace PmsApi.DTO
{
    public record AttachmentWithTaskDto(
        int Id,
        string FileName,
        string FileData,
        DateOnly CreationDate,
        int TaskId,
        Task Task
    );
}