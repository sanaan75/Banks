namespace Entities.Utilities;

public class EnumInfoAttribute : Attribute
{
    public EnumInfoAttribute(string caption)
    {
        Caption = caption;
    }

    public string Caption { get; set; }
}