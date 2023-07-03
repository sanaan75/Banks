using System.Text;
using Entities.Permissions;
using Entities.Utilities;

namespace Entities;

public static class AppSetting
{
    public const string Api_Key = "$JiroKey142857";
    public static List<BreadcrumbItem> Breadcrumb = new();
    public static Menu SelectedMenu { get; set; }

    #region JWT

    public static byte[] JwtKey => Encoding.UTF8.GetBytes("ahwan1234a5678b9");

    //ToDo: Change the value of this encryption key
    //value must be exact 16 digits
    public static byte[] JwtEncryptionKey => Encoding.UTF8.GetBytes("abcdefg123456qwe");

    public static int AllowUserAccountNo { get; set; } = 1;

    #endregion
}