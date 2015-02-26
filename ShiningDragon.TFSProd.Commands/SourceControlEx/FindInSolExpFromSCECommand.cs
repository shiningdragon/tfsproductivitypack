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

namespace ShiningDragon.TFSProd.Commands.SourceControlEx
{
    public class FindInSolExpFromSCECommand : FindInSolExpCommand
    {
        public FindInSolExpFromSCECommand(IMenuCommandService menuCommandService, ILogger _logger, DTE2 _dte, ITFSVersionControl _tfs)
            : base(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdFindInSolExpFromSCE, menuCommandService, _logger, _dte)
        {
            tfsVersionControl = _tfs;
        }

        public override void Exec(object sender, EventArgs e)
        {
            string selectedItem = tfsVersionControl.SCESelectedItems[0];
            try
            {
                logger.Log(string.Format("Try to find {0} in solution explorer", selectedItem), LogLevel.Verbose);

            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error finding {0} in solution explorer\n {1}", selectedItem, ex.ToString()), LogLevel.Error);
            }
        }


        public override void QueryStatus(object sender, EventArgs e)
        {
            try
            {
                menuCommand.Visible = true;
                menuCommand.Enabled = false;
                if (solutionExplorer.IsSolutionOpen && tfsVersionControl.SelectedItemsCanBeFoundInSolutionExplorer)
                {
                    menuCommand.Visible = true;
                    menuCommand.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error QueryStatus FindInSolExpFromSCECommand\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        private ITFSVersionControl tfsVersionControl;
    }
}
