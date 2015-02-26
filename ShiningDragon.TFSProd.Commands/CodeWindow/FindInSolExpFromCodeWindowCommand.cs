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
using ShiningDragon.TFSProd.Commands.Base;
using ShiningDragon.TFSProd.Common.VSCT;

namespace ShiningDragon.TFSProd.Commands.CodeWindow
{
    public class FindInSolExpFromCodeWindowCommand : FindInSolExpCommand
    {
        public FindInSolExpFromCodeWindowCommand(IMenuCommandService menuCommandService, ILogger _logger, DTE2 _dte)
            : base(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdFindInSolExpFromCodeWindow, menuCommandService, _logger, _dte)
        {

        }

        public override void Exec(object sender, EventArgs e)
        {
            try
            {
                logger.Log("Try to find current active document in solution explorer", LogLevel.Verbose);
                solutionExplorer.FindCurrentActiveDocumentInSolutionExplorer();
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error finding {0} in source control explorer\n {1}", "current active document", ex.ToString()), LogLevel.Error);
            }

        }

        public override void QueryStatus(object sender, EventArgs e)
        {
            try
            {
                menuCommand.Visible = true;
                menuCommand.Enabled = false;
                if (dte.ActiveDocument != null && dte.ActiveDocument.ProjectItem != null && solutionExplorer.IsSolutionOpen)
                {
                    menuCommand.Visible = true;
                    menuCommand.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error QueryStatus FindInSolExpFromCodeWindowCommand\n {0}", ex.ToString()), LogLevel.Error);
            }
        }
    }
}
