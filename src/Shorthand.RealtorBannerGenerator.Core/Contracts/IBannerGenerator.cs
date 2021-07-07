using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shorthand.RealtorBannerGenerator.Core.Contracts {
    public interface IBannerGenerator {
        Task<Memory<byte>> GetBannerImageAsync(string identifier, CancellationToken cancellationToken);
    }
}
