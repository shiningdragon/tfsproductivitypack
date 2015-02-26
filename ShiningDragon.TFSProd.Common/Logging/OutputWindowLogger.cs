using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;
using EnvDTE80;

namespace ShiningDragon.TFSProd.Common.Logging
{
    public class OutputWindowLogger : ILogger
    {
        private OutputWindowPane outputWindowPane = null;

        public OutputWindowLogger(DTE2 _applicationObject, string _eventSource, LogLevel level)
        {
            try
            {
                // Create an output pane for this package
                Window window = _applicationObject.Windows.Item(Constants.vsWindowKindOutput);
                OutputWindow outputWindow = (OutputWindow)window.Object;
                outputWindowPane = null;

                for (int i = 1; i <= outputWindow.OutputWindowPanes.Count; ++i)  // index starts from 1!
                {
                    if (outputWindow.OutputWindowPanes.Item(i).Name.Equals(_eventSource, StringComparison.CurrentCultureIgnoreCase))
                    {
                        outputWindowPane = outputWindow.OutputWindowPanes.Item(i);
                        break;
                    }
                }

                if (outputWindowPane == null)
                    outputWindowPane = outputWindow.OutputWindowPanes.Add(_eventSource);
            }
            catch
            {
                // Swallow it, never let errors in logging stop the add in
            }

            logLevel = level;
        }

        public void Log(string message, LogLevel type)
        {
            try
            {
                if (type <= logLevel)
                {
                    outputWindowPane.OutputString(string.Format("{0}\n", message));
                }
            }
            catch
            {
                // Swallow, never let errors in logging stop the add in
            }
        }

        public void LogError(string message, Exception ex)
        {
            Log(string.Format("{0}\n{1}", message, ex.ToString()), LogLevel.Error);
        }

        private LogLevel logLevel = LogLevel.Info;
    }
}
