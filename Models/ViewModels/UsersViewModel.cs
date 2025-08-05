namespace UPINS.Models.ViewModels
{
    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; }
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool AdminRole { get; set; }


    }
}
