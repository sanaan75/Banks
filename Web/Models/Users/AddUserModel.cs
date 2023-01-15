namespace Web.Models.Users
{
    public class AddUserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Title { get; set; }
        public bool Enabled { get; set; }
        public bool SysAdmin { get; set; }
        
    }
}