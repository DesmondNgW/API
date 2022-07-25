using System.ComponentModel;

namespace X.UI.WebAPI.Test
{
    public enum EnumTestMethodItem
    {
        GetGUID,
        GetDateTime,
    }

    public class TestMethodHelper
    {
        public static string GetGUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        
    }
}
