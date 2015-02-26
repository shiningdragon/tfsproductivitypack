using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiningDragon.TFSProd.TFS.Builds
{
    public struct BuildDefnDetails
    {
        public string Name { get; set; }
        public string TeamProjectName { get; set; }
        public Uri BuildUri { get; set; }
    }
}
