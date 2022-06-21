namespace X.UI.WebAPI.Test
{
    public enum EnumTestMethodItem
    {
        Test0 = 0,
        Test1 = 1,
    }

    public class TestMethodHelper
    {
        public static string Test0()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string Test1()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
    }
}
