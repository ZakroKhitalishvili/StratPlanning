using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Helpers
{
    /// <summary>
    /// Helper class that handles additional validations
    /// </summary>
    public class ValidationHelper
    {
        /// <summary>
        /// Validates a file for  a valid extension
        /// </summary>
        /// <param name="file">File, that need to be validates</param>
        /// <param name="validExtensions">extensions' array, that is used for validation</param>
        /// <returns></returns>
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

        /// <summary>
        /// Validates a file for valid size
        /// </summary>
        /// <param name="file">File, that need to be validates</param>
        /// <param name="megaBytes">Allowed size in megabytes</param>
        /// <returns></returns>
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
