using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TeamFoundation.VersionControl;

namespace ShiningDragon.TFSProd.TFS.VersionControl
{
    public interface ITFSVersionControl
    {
        bool Connect();

        void CompareFiles(string sourceFile, string targetFile);

        void CompareFolders(string sourceFile, string targetFile);

        List<string> GetBranches(string serverItem);

        void FindInSourceControlExplorerAsync(string localPath);

        void FindInSourceControlExplorer(string localPath);

        bool ShowDeletedItems { get; set; }

        bool IsVersionControlled(string localPath);

        string GetServerPathFromLocal(string localPath);

        bool SelectedItemsAllFolders { get; }

        bool SelectedItemsCanBeCompared { get; }

        bool SelectedItemsCanBeFoundInSolutionExplorer { get; }

        bool SingleNonDeletedItemSelected { get; }

        List<string> SCESelectedItems { get; }
    }
}
