using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE80;

using ShiningDragon.TFSProd.Common.Commands;
using ShiningDragon.TFSProd.Common.VSCT;
using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.TFS.VersionControl;
using Microsoft.VisualStudio.Shell;

namespace ShiningDragon.TFSProd.Commands.SourceControlEx
{
    public class CompareToBranchCommand
    {
        public CompareToBranchCommand(IMenuCommandService menuCommandService, ILogger _logger, ITFSVersionControl _tfs)
        {
            branches = new List<string>();
            logger = _logger;
            tfsVersionControl = _tfs;
            CommandID compareToBranchId = new CommandID(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdDynamicCompareToBranchCommand);
            compareToBranchOleCommand = new OleDynamicCommand(compareToBranchId, IsValidDynamicItem, Exec, QueryStatus);
            menuCommandService.AddCommand(compareToBranchOleCommand);
        }

        public void Exec(object sender, EventArgs e)
        {
            string sourceItem = string.Empty;
            string targetItem = string.Empty;
            try
            {
                OleDynamicCommand invokedCommand = (OleDynamicCommand)sender;
                sourceItem = tfsVersionControl.SCESelectedItems[0];
                targetItem = invokedCommand.Text;

                logger.Log(string.Format("Compare to branch: {0} {1}", sourceItem, targetItem), LogLevel.Verbose);
                if (tfsVersionControl.SelectedItemsAllFolders)
                {
                    tfsVersionControl.CompareFolders(sourceItem, targetItem);
                }
                else
                {
                    tfsVersionControl.CompareFiles(sourceItem, targetItem);
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error Compare to branch: {0} {1}\n {2}",
                    sourceItem, targetItem, ex.ToString()), LogLevel.Error);
            }
        }

        public void QueryStatus(object sender, EventArgs e)
        {
            try
            {
                OleDynamicCommand matchedCommand = (OleDynamicCommand)sender;
                if (branches.Count == 0)
                {
                    matchedCommand.Enabled = false;
                    matchedCommand.Visible = false;
                    return;
                }
                else
                {
                    matchedCommand.Enabled = true;
                    matchedCommand.Visible = true;
                }

                // Find out whether the command ID is 0, which is the ID of the root item.
                // If it is the root item, it matches the constructed DynamicItemMenuCommand,
                // and IsValidDynamicItem won't be called.
                bool isRootItem = (matchedCommand.MatchedCommandId == 0);

                int index = (isRootItem ? 0 : (matchedCommand.MatchedCommandId - (int)PkgCmdIDList.cmdIdDynamicCompareToBranchCommand));

                matchedCommand.Text = branches[index];

                // Clear the ID because we are done with this item.
                matchedCommand.MatchedCommandId = 0;
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error QueryStatus CompareToBranchCommand\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        public void ParentMenuQueryStatus(object sender, EventArgs e)
        {
            try
            {
                OleMenuCommand parentCommand = (OleMenuCommand)sender;
                parentCommand.Visible = true;
                parentCommand.Enabled = false;
                if (tfsVersionControl.Connect() && tfsVersionControl.SingleNonDeletedItemSelected)
                {
                    string currentItem = tfsVersionControl.SCESelectedItems[0];
                    if (currentItem.Equals(serverItem, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // do nothing
                    }
                    else
                    {
                        branches = tfsVersionControl.GetBranches(currentItem);
                        serverItem = currentItem;
                    }
                    if (branches.Count > 0)
                    {
                        parentCommand.Visible = true;
                        parentCommand.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error ParentQueryStatus CompareToBranchCommand\n {0}", ex.ToString()), LogLevel.Error);
            }
        }

        private bool IsValidDynamicItem(int commandId)
        {
            return (commandId >= (int)PkgCmdIDList.cmdIdDynamicCompareToBranchCommand) &&
                ((commandId - (int)PkgCmdIDList.cmdIdDynamicCompareToBranchCommand) < branches.Count);
        }

        private OleDynamicCommand compareToBranchOleCommand;
        private ITFSVersionControl tfsVersionControl;
        private ILogger logger;
        private List<string> branches;
        private string serverItem;
    }
}
