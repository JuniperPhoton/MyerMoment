using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogExt
{
    public class DialogEventArgs : EventArgs
    {
        public string ButtonClickName { get; set; }

        internal DialogEventArgs(string btnName)
        {
            this.ButtonClickName = btnName;
        }
        internal DialogEventArgs()
        {

        }
    }
}
