using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiningDragon.TFSProd.TFS.Builds
{
    internal struct WorkspaceBranchMapping
    {
        internal WorkspaceBranchMapping(string _sourceServerPath, string _targetServerPath)
            : this()
        {
            SourceServerPath = _sourceServerPath;
            TargetServerPath = _targetServerPath;
        }

        internal string SourceServerPath { get; private set; }
        internal string TargetServerPath { get; private set; }
    }
}
