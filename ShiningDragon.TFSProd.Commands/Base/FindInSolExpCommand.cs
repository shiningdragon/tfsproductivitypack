using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

using EnvDTE80;

using ShiningDragon.TFSProd.Common.Logging;
using ShiningDragon.TFSProd.Common.Commands;
using ShiningDragon.TFSProd.Common;

namespace ShiningDragon.TFSProd.Commands.Base
{
    public abstract class FindInSolExpCommand : AbstractCommand
    {
        public FindInSolExpCommand(Guid guidId, int id, IMenuCommandService menuCommandService, ILogger _logger, DTE2 _dte)
            : base(guidId, id, menuCommandService, _logger)
        {
            solutionExplorer = new SolutionExplorer(_dte);
            dte = _dte;
        }

        protected SolutionExplorer solutionExplorer;
        protected DTE2 dte;
    }
}
