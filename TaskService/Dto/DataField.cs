using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class DataField
    {
        public string Name { get; set; }
        //
        //public string Type { get; set; }

        private object mValue = null;

        public object Value
        {
            get
            {
                return this.mValue;
            }
            set
            {
                if (mValue != null)
                {
                    this.mIsChanged = true;
                }
                this.mValue = value;
            }
        }

        private bool mIsChanged = false;

        internal bool IsChanged
        {
            get
            {
                return this.mIsChanged;
            }
            private set
            {
                this.mIsChanged = value;
            }
        }

        //
        //public string Category { get; set; }
    }
}
