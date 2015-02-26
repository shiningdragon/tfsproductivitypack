using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TeamFoundation;
using ShiningDragon.TFSProd.Common.Logging;

namespace ShiningDragon.TFSProd.TFS.Connection
{
    public class TFSConnection : ITFSConnection
    {
        public TFSConnection(EnvDTE80.DTE2 _dte, ILogger _logger)
        {
            logger = _logger;
            teamFoundationServerExt = _dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
        }

        public bool Connect()
        {
            try
            {
                if (teamFoundationServerExt.ActiveProjectContext == null)
                {
                    return false;
                }

                if (tfsCollection == null || !tfsConnectionUrl.Equals(teamFoundationServerExt.ActiveProjectContext.DomainUri, StringComparison.CurrentCultureIgnoreCase))
                {
                    tfsConnectionUrl = teamFoundationServerExt.ActiveProjectContext.DomainUri;
                    if (string.IsNullOrWhiteSpace(tfsConnectionUrl))
                    {
                        return false;
                    }
                    if(ConnectionChanged != null)
                    {
                        ConnectionChanged(this, null);
                    }
                    logger.Log(string.Format("Connecting to TFS server '{0}'", tfsConnectionUrl), LogLevel.Info);
                    tfsCollection = new TfsTeamProjectCollection(new Uri(tfsConnectionUrl));
                    tfsCollection.Connect(Microsoft.TeamFoundation.Framework.Common.ConnectOptions.None);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Log(string.Format("Error trying to connect to tfs server\n {0}", ex.ToString()), LogLevel.Error);
                return false;
            }

        }

        public TfsTeamProjectCollection ProjectCollection
        {
            get
            {
                return tfsCollection;
            }
        }

        public string CurrentTeamProjectName 
        { 
            get
            {
                return teamFoundationServerExt.ActiveProjectContext.ProjectName;
            }
        }

        public event EventHandler ConnectionChanged;

        #region instance data

        private string tfsConnectionUrl = string.Empty;
        private TeamFoundationServerExt teamFoundationServerExt;
        private TfsTeamProjectCollection tfsCollection;
        private ILogger logger;

        #endregion
    }
}
