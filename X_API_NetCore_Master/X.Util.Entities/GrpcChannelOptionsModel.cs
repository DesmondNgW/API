using Grpc.Net.Client;

namespace X.Util.Entities
{
    public class GrpcChannelOptionsModel
    {
        public GrpcChannelOptions GrpcChannelOptions { get; set; }

        public string GrpcAddress { get; set; }

        public string GrpcName { get; set; }
    }
}
