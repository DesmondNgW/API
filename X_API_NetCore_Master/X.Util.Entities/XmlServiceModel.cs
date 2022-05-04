using System;
using System.Collections.Generic;
using System.Text;

namespace X.Util.Entities
{
    public class XmlServiceModel
    {
        public List<string> Endpoints { get; set; }
        public string ConfigurationName { get; set; }
        public int MaxPoolSize { get; set; }
        public int Index { get; set; }
    }
}
