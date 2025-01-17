using Microsoft.AspNetCore.Http;

namespace PIP.BL.IManagers;

public interface ICloudBucketManager
{
    void GenerateQrCode(string type,long id);
    void DeleteQrCode(long flowId);
    Task<string> UploadPicture(IFormFile photo, long id, string type);

    Task<string> UploadFile(IFormFile file, long id, string type);
    Task<string> UploadOnlyFile(IFormFile file);
}