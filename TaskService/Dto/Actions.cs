using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class Actions : List<ApproveAction>
    {

        public ApproveAction this[string name]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Name == name)
                    {
                        return this[i];
                    }
                }
                throw new Exception("The action name '" + name + "' does not exist!");
            }
            internal set
            {
            }
        }

    }
}
