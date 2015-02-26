using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.Common.Commands;
using ShiningDragon.TFSProd.TFS.VersionControl;
using ShiningDragon.TFSProd.TFS.Builds;
using ShiningDragon.TFSProd.Common.VSCT;

namespace ShiningDragon.TFSProd.Commands.Builds
{
    public class BranchBuildDefinitionCommand : AbstractCommand
    {
        public BranchBuildDefinitionCommand(IMenuCommandService menuCommandService, ILogger _logger, ITFSBuildService _tfsBuildService, ITFSVersionControl _tfsVersionControl)
            : base(GuidList.guidTFSProductivityPackCmdSet, PkgCmdIDList.cmdIdBranchBuildDefinition, menuCommandService, _logger)
        {
            tfsVersionControl = _tfsVersionControl;
            tfsBuildService = _tfsBuildService;
        }

        public override void Exec(object sender, EventArgs e)
        {
            BuildDefnDetails buildDefnDetail = new BuildDefnDetails() { Name = string.Empty, TeamProjectName = string.Empty };
            try
            {     
                List<BuildDefnDetails> selectedItems = tfsBuildService.SelectedBuildDefinitions;
                if (selectedItems.Count == 1 && tfsBuildService.Connect())
                {
                    buildDefnDetail = selectedItems[0];
                    logger.Log(string.Format("Branch build defintion {0} in project {1}", buildDefnDetail.Name, buildDefnDetail.TeamProjectName), LogLevel.Verbose);

                    tfsBuildService.BranchBuildDefinition(buildDefnDetail);
                } 
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error branching build {0} in project {1}", buildDefnDetail.Name, buildDefnDetail.TeamProjectName), ex);
            }
        }
        

        public override void QueryStatus(object sender, EventArgs e)
        {
            try
            {
                menuCommand.Visible = false;
                menuCommand.Enabled = false;
                if (tfsBuildService.Connect() && tfsBuildService.SelectedBuildDefinitions.Count == 1)
                {
                    menuCommand.Visible = true;
                    menuCommand.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error QueryStatus BranchBuildDefinitionCommand"), ex);
            }
        }

        private ITFSVersionControl tfsVersionControl;
        private ITFSBuildService tfsBuildService;
    }
}
