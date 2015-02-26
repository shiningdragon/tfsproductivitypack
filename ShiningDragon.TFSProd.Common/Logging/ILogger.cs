using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShiningDragon.TFSProd.Common.Logging
{
    public interface ILogger
    {
        void Log(string message, LogLevel type);

        void LogError(string message, Exception ex);
    }
}
