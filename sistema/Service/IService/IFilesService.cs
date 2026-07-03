using Database.Shared.DataBindings;
using Database.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sistema.Service.IService
{
    public interface IFilesService
    {
        Task<DtoFilesUploadFile> UploadFile(IFormFile file, string folder);
    }
}