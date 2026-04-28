using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace EO.Services.Common
{
    public interface ICommonService
    {
        Task<string?> SaveFileAsync(IFormFile file, string folderName, string userId);
    }
}