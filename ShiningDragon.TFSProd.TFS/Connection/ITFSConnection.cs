using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.Client;

namespace ShiningDragon.TFSProd.TFS.Connection
{
    public interface ITFSConnection
    {
        bool Connect();

        TfsTeamProjectCollection ProjectCollection { get; }

        string CurrentTeamProjectName { get; }

        event EventHandler ConnectionChanged;
    }
}
