using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ILoggerManager
    {
        void Error(string message);
        void Warn(string message);
        void Debug(string message);
        void Info(string message);
    }
}
