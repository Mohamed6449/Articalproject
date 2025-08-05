using Articalproject.Services.InterFaces;

namespace Articalproject.Services.Implementations
{
    public class FileServices : IFileServiece
    {
        private readonly IWebHostEnvironment _webHostEnvironment;//عشان نوصل ل wwwroot

        
        public FileServices(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

		public bool DeleteSource(string path)
		{
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + path);
            if (File.Exists(directory))
            {
                File.Delete(directory);
                return true;
            }
            return false;
		}

		public async Task<string> Upload(IFormFile file ,string Location)
        {
                try
                {
                var path = _webHostEnvironment.WebRootPath +Location;//folder معين جواه 
                var extension = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString().Replace("-", "") + extension;

                if (!Directory.Exists(path)){
                        Directory.CreateDirectory(path);

                 }

                using (FileStream fileStream = File.Create(path + fileName))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Flush();
                }
           
                return $"{Location}/{fileName}";
                }
                catch(Exception ex)
            {
                return ex.Message + "--"+ex.InnerException;
            }



        }
    }
}
