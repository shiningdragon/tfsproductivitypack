using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;

using Microsoft.VisualStudio.Shell;

using ShiningDragon.TFSProd.Common.Logging;

namespace ShiningDragon.TFSProd.Common.Commands
{
    public abstract class AbstractCommand : ICommand
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_dte"></param>
        /// <param name="_logger"></param>
        public AbstractCommand(Guid guidId, int id, IMenuCommandService menuCommandService, ILogger _logger)
        {
            logger = _logger;

            menuCommand = RegisterCommand(guidId, id, menuCommandService);
        }

        /// <summary>
        /// The dispaly text for the command
        /// </summary>
        public virtual string Text
        {
            get
            {
                return menuCommand.Text;
            }
            set
            {
                menuCommand.Text = value;
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        abstract public void Exec(object sender, EventArgs e);

        /// <summary>
        /// Optionally update the command status, default behaviour is to do nothing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void QueryStatus(object sender, EventArgs e)
        {

        }

        private OleMenuCommand RegisterCommand(Guid guidId, int id, IMenuCommandService menuCommandService)
        {
            var menuCommandID = new CommandID(guidId, id);
            var menuItem = new OleMenuCommand(Exec, menuCommandID);
            menuItem.BeforeQueryStatus += QueryStatus;
            menuCommandService.AddCommand(menuItem);
            return menuItem;
        }

        protected ILogger logger;
        protected OleMenuCommand menuCommand;
    }
}
