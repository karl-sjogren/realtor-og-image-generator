using System.Threading;
using System.Threading.Tasks;
using Shorthand.RealtorBannerGenerator.Core.Models;

namespace Shorthand.RealtorBannerGenerator.Core.Contracts {
    public interface IPropertyProvider {
        Task<RealtorProperty> GetRealtorPropertyAsync(string identifier, CancellationToken cancellationToken);
    }
}
