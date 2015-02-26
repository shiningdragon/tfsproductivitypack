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
using ShiningDragon.TFSProd.Common.VSCT;

namespace ShiningDragon.TFSProd.Commands.SourceControlEx
{
    public class TFSQuickCompareCommand : AbstractCommand
    {
        public TFSQuickCompareCommand(IMenuCommandService menuCommandService, ILogger _logger, ITFSVersionControl _tfs)
            : base(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdSCECompare, menuCommandService, _logger)
        {
            tfsVersionControl = _tfs;
        }

        public override void Exec(object sender, EventArgs e)
        {
            try
            {
                if (tfsVersionControl.SCESelectedItems.Count == 2 && tfsVersionControl.SelectedItemsCanBeCompared)
                {
                    logger.Log(string.Format("Comapring {0} with {1}", tfsVersionControl.SCESelectedItems[0], tfsVersionControl.SCESelectedItems[1]), LogLevel.Verbose);
                    if (tfsVersionControl.SelectedItemsAllFolders)
                    {
                        tfsVersionControl.CompareFolders(tfsVersionControl.SCESelectedItems[0], tfsVersionControl.SCESelectedItems[1]);
                    }
                    else
                    {
                        tfsVersionControl.CompareFiles(tfsVersionControl.SCESelectedItems[0], tfsVersionControl.SCESelectedItems[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error comparing {0} with {1}\n {2}", tfsVersionControl.SCESelectedItems[0], 
                    tfsVersionControl.SCESelectedItems[1], ex.ToString()), LogLevel.Error);
            }
        }

        public override void QueryStatus(object sender, EventArgs e)
        {
            try
            {
                if (menuCommand != null)
                {
                    // start by assuming that the menu will not be shown
                    menuCommand.Visible = true;
                    menuCommand.Enabled = false;

                    if (tfsVersionControl.Connect() && tfsVersionControl.SCESelectedItems.Count == 2 && tfsVersionControl.SelectedItemsCanBeCompared)
                    {
                        menuCommand.Visible = true;
                        menuCommand.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error QueryStatus TFSQuickCompareCommand\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        private ITFSVersionControl tfsVersionControl;
    }
}
