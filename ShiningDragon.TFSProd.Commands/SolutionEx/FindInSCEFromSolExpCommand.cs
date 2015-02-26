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
using ShiningDragon.TFSProd.Common;
using ShiningDragon.TFSProd.Commands.Base;
using ShiningDragon.TFSProd.Common.VSCT;


namespace ShiningDragon.TFSProd.Commands.SolutionEx
{
    public class FindInSCEFromSolExpCommand : FindInSCECommand
    {
        public FindInSCEFromSolExpCommand(IMenuCommandService menuCommandService, ILogger _logger, DTE2 _dte, ITFSVersionControl _tfs)
            : base(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdFindInSCEFromSolExp, menuCommandService, _logger, _dte, _tfs)
        {
            solutionExplorer = new SolutionExplorer(_dte);
        }

        public override void Exec(object sender, EventArgs e)
        {
            string localPath = string.Empty;
            try
            {
                localPath = solutionExplorer.GetSelectedItems()[0];
                logger.Log(string.Format("Try to find {0} in source control explorer", localPath), LogLevel.Verbose);
                FindInSourceControlExplorer(localPath);
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error finding {0} in source control explorer\n {1}", localPath, ex.ToString()), LogLevel.Error);
            }
        }

        public override void QueryStatus(object sender, EventArgs e)
        {
            try
            {
                menuCommand.Visible = false;
                menuCommand.Enabled = false;
                if (solutionExplorer.GetSelectedItems().Count == 1)
                {
                    string localPath = solutionExplorer.GetSelectedItems()[0];
                    logger.Log(string.Format("QueryStatus FindInSCEFromSolExpCommand, localPath: {0}", localPath), LogLevel.Verbose);
                    if (tfsVersionControl.IsVersionControlled(localPath))
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error QueryStatus FindInSCEFromSolExpCommand\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        private SolutionExplorer solutionExplorer;
    }
}
