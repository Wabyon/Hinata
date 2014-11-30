using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Hinata.Identity
{
    internal class EmptyIdentityServiceService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }
}