using System.Linq;
using System.Net.Cache;
using Entities;
using Entities.Journals;

namespace UseCases
{
    public interface ILogin
    {
        public Actor Respond(string username, string password);

        public Actor GetActor();
    }
}
