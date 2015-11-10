using System;

namespace X.Util.Entities
{
    public class EnumText : Attribute
    {
        public EnumText() { }

        public EnumText(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }
}
