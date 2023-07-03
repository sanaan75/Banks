namespace UseCases.Users;

public interface IEditUser
{
    public void Responce(Request request);

    public class Request
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public bool Enabled { get; set; }
        public bool SysAdmin { get; set; }
    }
}