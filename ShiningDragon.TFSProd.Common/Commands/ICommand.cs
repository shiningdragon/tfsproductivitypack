using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShiningDragon.TFSProd.Common.Commands
{
    public interface ICommand
    {
        void Exec(object sender, EventArgs e);

        void QueryStatus(object sender, EventArgs e);

        string Text { get; set; }
    }
}
