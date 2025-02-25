namespace Dashboard.Api.Helpers
{
    public class FileHelper
    {

        public static async Task<string> StoreImageAsync(IFormFile image, IWebHostEnvironment  environment)
        {
            var imagePath = Path.Combine( environment.WebRootPath, "images", image.FileName);
            var uniqueImagePath = GetUniqueFilePath(imagePath);

            using (var stream = new FileStream(uniqueImagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/images/{Path.GetFileName(uniqueImagePath)}";
        }

        private static string GetUniqueFilePath(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);
            var uniqueFilePath = filePath;
            var counter = 1;

            while (System.IO.File.Exists(uniqueFilePath))
            {
                uniqueFilePath = Path.Combine(directory, $"{fileName}_{counter}{extension}");
                counter++;
            }

            return uniqueFilePath;
        }
    }
}
