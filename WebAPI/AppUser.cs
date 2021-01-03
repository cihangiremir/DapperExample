using System.Collections.Generic;

namespace WebAPI
{
    public class AppUser : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
    }
}
