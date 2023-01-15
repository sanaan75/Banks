using Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Users
{
    public interface IAddUser
    {
        public User Responce(Request request);

        public class Request
        {
            public string Title { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool Enabled { get; set; }
            public bool SysAdmin { get; set; }
        }
    }    
}
