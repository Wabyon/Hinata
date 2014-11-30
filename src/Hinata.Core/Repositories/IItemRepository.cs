using System.Collections.Generic;
using System.Threading.Tasks;
using Hinata.Models;

namespace Hinata.Repositories
{
    public interface IItemRepository
    {
        Task<Item> FindByIdAsync(string id);

        Task DeleteAsync(Item item);

        Task SaveAsync(Item item);

        Task<IEnumerable<Item>> GetPublicAsync(string userId, int skip, int take);

        Task<IEnumerable<Item>> GetFollowingAsync(string userId, int skip, int take);

        Task<IEnumerable<Item>> GetUserPublicAsync(string userId, int skip, int take);

        Task<IEnumerable<Item>> GetUserPrivateAsync(string userId, int skip, int take);
    }
}