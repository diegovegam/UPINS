using Microsoft.AspNetCore.Identity;

namespace UPINS.Repositories
{
    public interface IUserRepository
    {
        public Task<IEnumerable<IdentityUser>> GetUsers();
    }
}
