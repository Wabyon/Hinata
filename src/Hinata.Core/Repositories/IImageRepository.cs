using System.Threading.Tasks;
using Hinata.Models;

namespace Hinata.Repositories
{
    public interface IImageRepository
    {
        Task SaveAsync(Image image);

        Task<Image> FindAsync(string name);
    }
}
