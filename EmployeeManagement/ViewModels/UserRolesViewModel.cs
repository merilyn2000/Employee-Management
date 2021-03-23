namespace EmployeeManagement.ViewModels
{
    public class UserRolesViewModel
    {
        //public string UserId { get; set; } il punem cu viewbag deoarece nu trebuie duplicat ONE TO MANY
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
