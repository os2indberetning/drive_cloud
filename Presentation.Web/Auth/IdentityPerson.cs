using Core.DomainModel;
using Microsoft.AspNetCore.Identity;

namespace Presentation.Web.Auth
{
    public class IdentityPerson : IdentityUser<string>
    {
        public IdentityPerson(Person person)
        {
            Person = person;
        }
        public Person Person { get; private set; }
    }
}
