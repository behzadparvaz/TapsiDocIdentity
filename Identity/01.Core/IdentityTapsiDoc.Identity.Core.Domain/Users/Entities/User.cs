using Microsoft.AspNetCore.Identity;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Entities
{
    public class User : IdentityUser
    {
        public new required string PhoneNumber { get; set; }
        //TODO: check with behzad : can FName and LName is Nullable?.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
