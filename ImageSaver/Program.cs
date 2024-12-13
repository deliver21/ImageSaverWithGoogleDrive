using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace ImageSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            string credentialsPath = "credentials.json";
            string folderId = "1WSWCHLxD_kjS6hS_K8NYsiDPlYreYbD0";
            string[] filesToUpload = { "Image 25.jpg" };

            UploadFilesToGoogleDrive(credentialsPath, folderId, filesToUpload);
        }      
        static void UploadFilesToGoogleDrive(string credentialsPath, string folderId, string[] filesPath)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(new[]
                {
                    DriveService.ScopeConstants.DriveFile
                });

                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Drive Upload Mate_Code"
                });

                foreach (var filePath in filesPath)
                {
                    var fileMetaData = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = Path.GetFileName(filePath),
                        Parents = new List<string> { folderId }
                    };

                    FilesResource.CreateMediaUpload request;
                    using (var streams = new FileStream(filePath, FileMode.Open))
                    {
                        request = service.Files.Create(fileMetaData, streams, "");
                        request.Fields = "id";
                        request.Upload();
                    }

                    var uploadedFile = request.ResponseBody;
                    Console.WriteLine($"File '{fileMetaData.Name}' uploaded with ID: {uploadedFile.Id}");
                }                
            }
        }
    }
}