using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Build.Controls.Extensibility;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.TeamFoundation.Build;
using Microsoft.TeamFoundation.VersionControl.Client;

using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.TFS.Connection;
using ShiningDragon.TFSProd.Common;
using System.Text.RegularExpressions;


namespace ShiningDragon.TFSProd.TFS.Builds
{
    public class TFSBuildService : ITFSBuildService
    {
        public TFSBuildService(ITFSConnection _tfsConnection, IServiceProvider serviceProvider, ILogger _logger)
        {
            tfsConnection = _tfsConnection;
            tfsConnection.ConnectionChanged += ResetConnection;
            logger = _logger;
            teamExplorer = serviceProvider.GetService(typeof(ITeamExplorer)) as ITeamExplorer;
            vsTfsBuild = serviceProvider.GetService(typeof(IVsTeamFoundationBuild)) as IVsTeamFoundationBuild;
        }

        public bool Connect()
        {
            return tfsConnection.Connect();
        }
   
        public void BranchBuildDefinition(BuildDefnDetails buildDetail)
        {
            IBuildDefinition defn = buildServer.GetBuildDefinition(buildDetail.TeamProjectName, buildDetail.Name);
            if(defn.GetDefaultSourceProvider().Name != "TFVC")
            {
                MessageBox.Show("Branch build definition is only supported for Team Foundation Version Controlled builds", "Microsoft Visual Studio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            List<WorkspaceBranchMapping> originalMappings = 
                (from mapping in defn.Workspace.Mappings select new WorkspaceBranchMapping(mapping.ServerItem, mapping.ServerItem)).ToList();

            VersionControlServer versionControlServer = (VersionControlServer)tfsConnection.ProjectCollection.GetService(typeof(VersionControlServer));
            WorkSpaceMappingDialog dlg = new WorkSpaceMappingDialog(originalMappings, versionControlServer);
            dlg.Text = string.Format("Branch Build - {0}", defn.Name);
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                IBuildDefinition branchedDefn = buildServer.CreateBuildDefinition(buildDetail.TeamProjectName);
                branchedDefn.CopyFrom(defn);
                branchedDefn.Name = string.Format("Branch of {0}", buildDetail.Name);

                // Update new build workspace
                foreach (IWorkspaceMapping mapping in branchedDefn.Workspace.Mappings)
                {
                    foreach(WorkspaceBranchMapping branchedMapping in dlg.BranchedMappings)
                    {
                        string pattern = branchedMapping.SourceServerPath.Replace("$", @"\$");
                        mapping.ServerItem = Regex.Replace(mapping.ServerItem, pattern, branchedMapping.TargetServerPath, RegexOptions.IgnoreCase);
                    }
                }

                // Update process parameters
                foreach (WorkspaceBranchMapping branchedMapping in dlg.BranchedMappings)
                {
                    string pattern = branchedMapping.SourceServerPath.Replace("$", @"\$");
                    branchedDefn.ProcessParameters = Utilities.ReplaceTFSServerPaths(branchedDefn.ProcessParameters, branchedMapping.SourceServerPath, branchedMapping.TargetServerPath);
                    //Regex.Replace(branchedDefn.ProcessParameters, pattern, branchedMapping.TargetServerPath, RegexOptions.IgnoreCase);
                    //branchedDefn.Process.ServerPath = Regex.Replace(branchedDefn.ProcessParameters, pattern, branchedMapping.TargetServerPath, RegexOptions.IgnoreCase);
                }
       
                try
                {
                    branchedDefn.Save();
                }
                catch (BuildDefinitionAlreadyExistsException ex)
                {
                    MessageBox.Show(string.Format("Team Foundation Error\n\r\n\r{0}", ex.Message), "Microsoft Visual Studio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (vsTfsBuild != null)
                {
                    vsTfsBuild.DefinitionManager.OpenDefinition(branchedDefn.Uri);
                }

                teamExplorer.CurrentPage.Refresh();
            }
        }

        public List<BuildDefnDetails> SelectedBuildDefinitions
        {
            get
            {
                List<BuildDefnDetails> selectedBuildDefinitions = new List<BuildDefnDetails>();
                IBuildsPageExt buildsPageExt = ITeamExplorerPageExtensions.GetService<IBuildsPageExt>(teamExplorer.CurrentPage);
                if(buildsPageExt != null)
                {
                    selectedBuildDefinitions = (from def in buildsPageExt.SelectedDefinitions.Concat(buildsPageExt.SelectedFavoriteDefinitions) 
                                                select new BuildDefnDetails()
                                                {
                                                    Name = def.Name,
                                                    TeamProjectName = tfsConnection.CurrentTeamProjectName,
                                                    BuildUri = def.Uri
                                                }).ToList();
                }

                return selectedBuildDefinitions;
            }

        }

        private void ResetConnection(object sender, EventArgs e)
        {
            _buildServer = null;
        }

        #region instance data

        private IBuildServer buildServer
        {
            get
            {
                if (null == _buildServer)
                {
                    _buildServer = tfsConnection.ProjectCollection.GetService<IBuildServer>();
                }
                return _buildServer;
            }
        }
        private IBuildServer _buildServer;

        IVsTeamFoundationBuild vsTfsBuild;
        private ITFSConnection tfsConnection;
        private ITeamExplorer teamExplorer;
        private ILogger logger;        
       
        #endregion
    }
}

