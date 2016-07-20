using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.Service.Dto
{
    public class XmlField
    {
        public string Name { get; set; }
        //
        //public string Type { get; set; }

        private string mValue = null;

        public string Value
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

        public string Category { get; set; }

        public string Schema { get; set; }

        public bool Hidden { get; set; }

        public string MetaData { get; set; }

        public string Xsl { get; set; }

    }
}
