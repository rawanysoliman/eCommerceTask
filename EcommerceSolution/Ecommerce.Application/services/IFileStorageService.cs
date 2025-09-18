using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.services
{
    public interface IFileStorageService
    {
        Task<string?> SaveFileAsync(IFormFile file, string folder);
        void DeleteFile(string relativePath);
    }
}
