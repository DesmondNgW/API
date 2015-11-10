using System.Collections.Generic;

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
