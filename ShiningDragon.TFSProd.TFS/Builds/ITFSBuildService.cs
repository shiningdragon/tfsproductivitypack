using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiningDragon.TFSProd.TFS.Builds
{
    public interface ITFSBuildService
    {
        List<BuildDefnDetails> SelectedBuildDefinitions { get; }

        void BranchBuildDefinition(BuildDefnDetails buildDetail);

        bool Connect();
    }
}
