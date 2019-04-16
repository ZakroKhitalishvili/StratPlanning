using System;
using Application.Interfaces;
using NLog;

namespace Application.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger _logger;

        public LoggerManager()
        {
            if (_logger == null)
            {
                _logger = LogManager.GetCurrentClassLogger();
            }
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}
