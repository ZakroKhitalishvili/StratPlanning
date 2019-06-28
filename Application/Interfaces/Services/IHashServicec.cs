using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    /// <summary>
    /// Hashing service interface for handling hashing-related requirement
    /// </summary>
    public interface IHashService
    {
        /// <summary>
        /// Hashes passed string
        /// </summary>
        /// <param name="source">Initial string that needs to be hashed</param>
        /// <returns></returns>
        string Hash(string source);
    }
}
