using System.Collections.Generic;

namespace X.Interface.Dto.HttpRequest
{
    public class ApiRequestDto
    {
        public string Uri { get; set; }

        public object Postdata { get; set; }

        public Dictionary<string, string> ExtendHeaders { get; set; }
    }
}
