using System;

namespace Shorthand.RealtorBannerGenerator.Core.Models {
    public record RealtorProperty {
        public string Identifier { get; init; }
        public string Generator { get; init; }
        public string PropertyImageUrl { get; init; }
        public string StreetAddress { get; init; }
        public string City { get; init; }
        public Int32? StartingPrice { get; init; }
        public string RealtorName { get; init; }
        public string RealtorImageUrl { get; init; }
    }
}
