using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

using EnvDTE80;

using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.Common.Commands;
using ShiningDragon.TFSProd.TFS.VersionControl;


namespace ShiningDragon.TFSProd.Commands.Base
{
    public abstract class FindInSCECommand : AbstractCommand
    {
        public FindInSCECommand(Guid guidId, int id, IMenuCommandService menuCommandService, ILogger _logger, DTE2 _dte, ITFSVersionControl _tfs)
            : base(guidId, id, menuCommandService, _logger)
        {
            dte = _dte;
            tfsVersionControl = _tfs;
        }

        protected virtual void FindInSourceControlExplorer(string localPath)
        {
            try
            {
                tfsVersionControl.FindInSourceControlExplorerAsync(localPath);
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error finding file {0} in source control explorer\n {1}", localPath, ex.ToString()),LogLevel.Error);
            }
        }

        protected ITFSVersionControl tfsVersionControl;
        protected DTE2 dte;
    }
}
