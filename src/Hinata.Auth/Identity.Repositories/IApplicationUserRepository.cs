using System.Threading.Tasks;

namespace Hinata.Identity.Repositories
{
    public interface IApplicationUserRepository
    {
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task CreateAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(ApplicationUser user);
    }
}