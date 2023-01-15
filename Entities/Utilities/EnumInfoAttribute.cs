using System;

namespace Entities
{
    public class EnumInfoAttribute : Attribute
    {
        public string Caption { get; set; }

        public EnumInfoAttribute(string caption)
        {
            Caption = caption;
        }
    }
}