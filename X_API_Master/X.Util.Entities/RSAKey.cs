namespace X.Util.Entities
{
    public class RsaKey : MongoBaseModel
    {
        public string D { get; set; }
        public string Dp { get; set; }
        public string Dq { get; set; }
        public string Exponent { get; set; }
        public string InverseQ { get; set; }
        public string Modulus { get; set; }
        public string P { get; set; }
        public string Q { get; set; }
        public bool TmpKey { get; set; }
    }
}
