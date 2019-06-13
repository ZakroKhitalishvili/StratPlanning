using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public class UploadHelper
    {

        public static string Upload(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            var uniqueName = Guid.NewGuid().ToString();

            var uploadName = $"{uniqueName}{extension}";

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads",
                                    uploadName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/Uploads/{uploadName}";
        }


        public static bool Delete(string relPath)
        {
            var path=Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads",
                                    Path.GetFileName(relPath));
            if(File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch(Exception)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
