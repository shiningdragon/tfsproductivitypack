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
    public class ToggleShowDeletedItemsCommand : AbstractCommand
    {
        public ToggleShowDeletedItemsCommand(IMenuCommandService menuCommandService, ILogger _logger, ITFSVersionControl _tfs)
            : base(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdSCEShowDeleted, menuCommandService, _logger)
        {
            tfsVersionControl = _tfs;
            SetShowDeletedItemsCommandText();
        }

        public override void Exec(object sender, EventArgs e)
        {
            try
            {
                tfsVersionControl.ShowDeletedItems = !tfsVersionControl.ShowDeletedItems;
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error toggling show deleted files\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        public override void QueryStatus(object sender, EventArgs e)
        {
            SetShowDeletedItemsCommandText();
        }

        private void SetShowDeletedItemsCommandText()
        {
            if (tfsVersionControl.ShowDeletedItems)
            {
                this.Text = "Show Deleted Items OFF";
            }
            else
            {
                this.Text = "Show Deleted Items ON";
            }
        }

        private ITFSVersionControl tfsVersionControl;

    }
}
