using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using EnvDTE;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation;

using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.Common;
using ShiningDragon.TFSProd.TFS.Connection;

namespace ShiningDragon.TFSProd.TFS.VersionControl
{
    public class TFSVersionControl : ITFSVersionControl
    {
        public TFSVersionControl(ITFSConnection _tfsConnection, EnvDTE80.DTE2 _dte, ILogger _logger)
        {
            dte = _dte;
            logger = _logger;
            tfsConnection = _tfsConnection;
            tfsConnection.ConnectionChanged += ResetConnection;
            versionControlControlsAssembly = Assembly.Load(versionControlControlsAssemblyString);
            folderDiffOptions = AccessPrivateWrapper.FromType(versionControlControlsAssembly, BindingFlags.NonPublic | BindingFlags.Instance, "FolderDiffOptions");
        }

        public bool Connect()
        {
            return tfsConnection.Connect();
        }

        public void CompareFiles(string sourceFile, string targetFile)
        {
            Difference.VisualDiffItems(versionControlServer,
                Difference.CreateTargetDiffItem(versionControlServer, sourceFile, VersionSpec.Latest, 0, VersionSpec.Latest),
                Difference.CreateTargetDiffItem(versionControlServer, targetFile, VersionSpec.Latest, 0, VersionSpec.Latest),
                false);
        }

        public void CompareFolders(string sourceFolder, string targetFolder)
        {
            VersionControlFolderDifferenceExt.VersionControlFolderDifferenceParams parameters = null;

            try
            {
                folderDiffOptions.Initialize();

                parameters = new
                    VersionControlFolderDifferenceExt.VersionControlFolderDifferenceParams()
                    {
                        ShowItemsDifferentInBoth = folderDiffOptions.ShowItemsWithDifferentContents,
                        ShowItemsEqualInBoth = folderDiffOptions.ShowItemsWithIdenticalContents,
                        ShowItemsInSourcePathOnly = folderDiffOptions.ShowItemsExistOnlyInPath1,
                        ShowItemsInTargetPathOnly = folderDiffOptions.ShowItemsExistOnlyInPath2,
                        Filter = folderDiffOptions.FilterHistory.Count > 0 ? folderDiffOptions.FilterHistory[0] : string.Empty,
                        FilterLocalPathsOnly = folderDiffOptions.FilterLocalPathsOnly,
                        SourcePath = sourceFolder,
                        TargetPath = targetFolder
                    };
            }
            catch(Exception ex)
            {
                logger.LogError("Error reading folder difference params, using defaults", ex);
                parameters = new
                    VersionControlFolderDifferenceExt.VersionControlFolderDifferenceParams()
                {
                    SourcePath = sourceFolder,
                    TargetPath = targetFolder
                };
            }

            FolderDifferenceExt.Show(parameters);
        }

        public async void FindInSourceControlExplorerAsync(string localPath)
        {
            bool isConnected = SourceControlExplorer.Connected;
            logger.Log(string.Format("SCE is connected: {0}", isConnected), LogLevel.Verbose);
            if (!isConnected)
            {
                SourceControlExplorer.Navigate(@"$/");
                isConnected = await Task<bool>.Factory.StartNew(() => Utilities.Poll(() => SourceControlExplorer.Connected, 60, 250));
                logger.Log(string.Format("SCE is connected: {0}", isConnected), LogLevel.Verbose);
            }

            string serverPath = GetServerPathFromLocal(localPath);
            string serverPathFolder = serverPath.Substring(0, serverPath.LastIndexOf("/"));
            string currentFolder = SourceControlExplorer.CurrentFolderItem.SourceServerPath;
            logger.Log(string.Format("serverPathFolder is {0} :  currentFolder is: {1}", serverPathFolder, SourceControlExplorer.CurrentFolderItem.SourceServerPath), LogLevel.Verbose);
            if (serverPathFolder.Equals(currentFolder, StringComparison.CurrentCultureIgnoreCase))
            {
                logger.Log("Navigate to root", LogLevel.Verbose);
                SourceControlExplorer.Navigate(@"$/");
            }

            SourceControlExplorer.Navigate(serverPath);

            PropertyInfo propInfoExplorerToolWindow = typeof(VersionControlExplorerExt).GetProperty("ExplorerToolWindow", BindingFlags.NonPublic | BindingFlags.Instance);
            object explorerToolWindow = propInfoExplorerToolWindow.GetValue(SourceControlExplorer);
            dynamic sccToolWindow = new AccessPrivateWrapper(explorerToolWindow);
            dynamic explorer = new AccessPrivateWrapper(sccToolWindow.SccExplorer);
            dynamic listViewExplorer = explorer.listViewExplorer;
            if (listViewExplorer != null)
            {
                logger.Log("Try to ensure item is selected in list view", LogLevel.Verbose);
                Func<bool> condition = () => listViewExplorer.SelectedIndices.Count > 0;
                Action action = () =>
                {
                    int selectedIndex = listViewExplorer.SelectedIndices[0];
                    listViewExplorer.EnsureVisible(selectedIndex);
                };
                Utilities.DispatcherPoll(condition, action, 60, 250);
            }

        }

        public void FindInSourceControlExplorer(string localPath)
        {
            throw new NotImplementedException("FindInSourceControlExplorer not implemented");
        }

        public bool ShowDeletedItems
        {
            get
            {
                return SourceControlExplorer.ShowDeletedItems;
            }
            set
            {
                SourceControlExplorer.ShowDeletedItems = value;
            }
        }

        public bool IsVersionControlled(string localPath)
        {
            string serverPath = GetServerPathFromLocal(localPath);
            return !string.IsNullOrWhiteSpace(serverPath);
        }

        public string GetServerPathFromLocal(string localPath)
        {
            if (string.IsNullOrWhiteSpace(localPath))
            {
                return string.Empty;
            }

            if (!Connect())
            {
                return string.Empty;
            }

            Workspace workspace = null;
            try
            {
                workspace = versionControlServer.GetWorkspace(localPath);
            }
            catch (ItemNotMappedException)
            {
                return string.Empty;
            }

            if (workspace != null)
            {
                return workspace.TryGetServerItemForLocalItem(localPath);
            }
            else
            {
                return string.Empty;
            }
        }

        public List<string> SCESelectedItems
        {
            get
            {
                return (from item in SourceControlExplorer.SelectedItems select item.SourceServerPath).ToList();
            }
        }

        public bool SelectedItemsAllFolders
        {
            get
            {
                var selectedItems = from item in SourceControlExplorer.SelectedItems select item.IsFolder;
                if (selectedItems.Count() == 0)
                    return false;

                bool val = selectedItems.First();
                return selectedItems.All(x => x == val) ? val : false;
            }
        }

        public bool SelectedItemsCanBeCompared
        {
            get
            {
                var selectedItems = from item in SourceControlExplorer.SelectedItems select item.IsFolder;
                if (selectedItems.Count() == 0)
                    return false;

                var deletedItems = from item in SourceControlExplorer.SelectedItems where item.IsDeleted select item;
                if (deletedItems.Count() > 0)
                    return false;

                bool val = selectedItems.First();
                return selectedItems.All(x => x == val);
            }
        }

        public bool SelectedItemsCanBeFoundInSolutionExplorer
        {
            get
            {
                var selectedItems = from item in SourceControlExplorer.SelectedItems select item;
                if (selectedItems.Count() != 1)
                    return false;

                var selectedItem = selectedItems.First();
                if (selectedItem.IsDeleted || selectedItem.IsFolder || string.IsNullOrWhiteSpace(selectedItem.LocalPath))
                    return false;

                return true;
            }
        }

        public bool SingleNonDeletedItemSelected
        {
            get
            {
                var selectedItems = from item in SourceControlExplorer.SelectedItems select item;
                if (selectedItems.Count() != 1)
                    return false;

                var selectedItem = selectedItems.First();
                if (selectedItem.IsDeleted)
                    return false;

                return true;
            }
        }

        public List<string> GetBranches(string serverItem)
        {
            List<string> branches = new List<string>();
            foreach (ItemIdentifier itemIdentifier in versionControlServer.QueryMergeRelationships(serverItem))
            {
                if (!itemIdentifier.IsDeleted && !string.IsNullOrEmpty(itemIdentifier.Item))
                {
                    branches.Add(itemIdentifier.Item);
                }
            }
            return branches;
        }

        private void ResetConnection(object sender, EventArgs e)
        {
            _versionControlServer = null;
        }

        #region instance data

        private VersionControlExplorerExt SourceControlExplorer
        {
            get
            {
                if (null == sourceControlExplorer)
                {
                    VersionControlExt vce = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
                    sourceControlExplorer = vce.Explorer;
                }

                return sourceControlExplorer;
            }
        }
        private VersionControlExplorerExt sourceControlExplorer = null;

        private VersionControlFolderDifferenceExt FolderDifferenceExt
        {
            get
            {
                if (null == folderDifferenceExt)
                {
                    VersionControlExt vce = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
                    folderDifferenceExt = vce.FolderDifference;
                }

                return folderDifferenceExt;
            }
        }
        private VersionControlFolderDifferenceExt folderDifferenceExt;

        private VersionControlServer versionControlServer
        {
            get
            {
                if (null == _versionControlServer)
                {
                    _versionControlServer = tfsConnection.ProjectCollection.GetService<VersionControlServer>();
                }
                return _versionControlServer;
            }
        }
        private VersionControlServer _versionControlServer;

        private ITFSConnection tfsConnection;
        private ILogger logger;
        private EnvDTE80.DTE2 dte;
        private readonly string versionControlControlsAssemblyString = "Microsoft.TeamFoundation.VersionControl.Controls, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
        private Assembly versionControlControlsAssembly;
        private dynamic folderDiffOptions;

        #endregion
    }
}
