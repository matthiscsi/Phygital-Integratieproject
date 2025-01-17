using Google.Cloud.Storage.V1;
using PIP.BL.IManagers;
using QRCoder;
using Microsoft.AspNetCore.Http;

namespace PIP.BL.Managers
{
    public class CloudBucketManager : ICloudBucketManager
    {
        public void GenerateQrCode(string type, long id)
        {
            string qrHyperlink;

            if (type == "flow")
            {
                qrHyperlink = $"https://phygital.programmersinparis.net/Idea/Index?flowid={id}";
            }
            else if (type == "flowSessionClient")
            {
                qrHyperlink = $"https://phygital.programmersinparis.net/FlowStep/ClientFlowStepIndex/{id}";
            }
            else if (type == "flowSessionHost")
            {
                qrHyperlink = $"https://phygital.programmersinparis.net/FlowStep/HostFlowStepIndex/{id}";
            }
            else if (type == "flowSessionBegeleider")
            {
                qrHyperlink = $"https://phygital.programmersinparis.net/FlowStep/CompanionFlowStepIndex/{id}";
            }
            else
            {
                //ThankYou
                qrHyperlink = $"https://phygital.programmersinparis.net/Idea/Index?flowid={id}";
            }
            

            

            // Generate the QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrHyperlink, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            // Upload the QR code to a Google Cloud Storage bucket
            StorageClient storageClient = StorageClient.Create();
            using (var stream = new MemoryStream(qrCodeImage))
            {
                // Use the flow id as part of the image name
                string imageName = $"qr-code-{type}-{id}.png";
                storageClient.UploadObject("phygitalmediabucket", imageName, "image/png", stream);
            }
        }
        
        public void DeleteQrCode(long flowId)
        {
            // Use the flow id as part of the image name
            string imageName = $"qr-code-flow-{flowId}.png";

            // Create a storage client
            StorageClient storageClient = StorageClient.Create();

            // Delete the object (QR code image) from the bucket
            storageClient.DeleteObject("phygitalmediabucket", imageName);
        }
        
        public async Task<string> UploadPicture(IFormFile photo, long id, string type)
        {
            var storageClient = await StorageClient.CreateAsync();
            var bucketName = "phygitalmediabucket";
            var extension = Path.GetExtension(photo.FileName); // Get the file extension
            var objectName = $"{type}-{id}-picture{extension}"; // Include the file extension in the name

            using var stream = photo.OpenReadStream();
            await storageClient.UploadObjectAsync(bucketName, objectName, null, stream);

            // Get the URL of the uploaded photo
            var urlPhoto = $"https://storage.googleapis.com/{bucketName}/{objectName}";

            return urlPhoto;
        }
        
        public async Task<string> UploadFile(IFormFile file, long id, string type)
        {
            var storageClient = await StorageClient.CreateAsync();
            var bucketName = "phygitalmediabucket";
            var extension = Path.GetExtension(file.FileName); // Get the file extension
            var objectName = $"{type}-{id}-picture{extension}"; // Include the file extension in the name

            var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0; // Reset stream position to the beginning

            await storageClient.UploadObjectAsync(bucketName, objectName, null, memoryStream);

            // Get the URL of the uploaded photo
            var urlPhoto = $"https://storage.googleapis.com/{bucketName}/{objectName}";

            return urlPhoto;
        }




        public async Task<string> UploadOnlyFile(IFormFile file)
        {
            var storageClient = await StorageClient.CreateAsync();
            var bucketName = "phygitalmediabucket";
            var extension = Path.GetExtension(file.FileName); // Get the file extension
            var objectName = $"Info-{Guid.NewGuid()}{extension}"; // Use a GUID to ensure the file name is unique

            using var stream = file.OpenReadStream();
            await storageClient.UploadObjectAsync(bucketName, objectName, null, stream);

            // Get the URL of the uploaded file
            var urlFile = $"https://storage.googleapis.com/{bucketName}/{objectName}";

            return urlFile;
        }
    }
}