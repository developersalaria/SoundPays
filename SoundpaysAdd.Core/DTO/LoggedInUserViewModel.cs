using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Core.DTO
{
    public class LoggedInUserViewModel
    {
        public LoggedInUserViewModel()
        {
            Roles = new List<UserRoleModel>();
        }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Organization { get; set; }
        public int OrganizationId { get; set; }
        public List<UserRoleModel> Roles { get; set; }

        public bool IsCoordinator { get => Roles.Any(x => x.UserIdentityRoleId == Enums.Roles.Advertiser.ToString()); }
        public bool IsAdmin { get => Roles.Any(x => x.UserIdentityRoleId == Enums.Roles.Administrator.ToString()); }

    }
}
