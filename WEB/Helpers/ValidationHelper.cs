using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Helpers
{
    public class ValidationHelper
    {
        public static bool ValidateFileExtension(IFormFile file, string[] validExtensions)
        {
            if (validExtensions == null)
            {
                return true;
            }

            var extension = Path.GetExtension(file.FileName);

            if (validExtensions.Any(x => x.Contains(extension)))
            {
                return true;
            }

            return false;
        }

        public static bool ValidateFileSize(IFormFile file, int megaBytes)
        {
            if (file.Length <= megaBytes * 1024 * 1024)
            {
                return true;
            }

            return false;
        }
    }
}
