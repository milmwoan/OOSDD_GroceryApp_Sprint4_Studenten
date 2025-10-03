
using Grocery.Core.Enums;
namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public Role UserRole { get; set; } = Role.None; // zet de default waarde van UserRole naar None
        public Client(int id, string name, string emailAddress, string password, Role role) : base(id, name)
        {
            EmailAddress=emailAddress;
            Password=password;
            UserRole=role;
        }
    }
}
