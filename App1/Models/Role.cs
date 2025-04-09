namespace App1.Models
{
    public enum RoleType
    {
        Banned = 0,
        User = 1,
        Admin = 2,
        Manager = 3
    }

    public class Role
    {
        public RoleType RoleType { get; set; }
        public string RoleName { get; set; }

        public Role(RoleType roleType, string roleName)
        {
            RoleType = roleType;
            RoleName = roleName;
        }
    }
}
