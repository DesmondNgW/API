namespace X.Util.Entities
{
    public class Event<T>
    {
        public int HashCode { get; set; }
        public string Name { get; set; }
        public T Target { get; set; }
    }
}
