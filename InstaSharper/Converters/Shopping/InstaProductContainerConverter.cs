/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.Models.Shopping;
using InstaSharper.Classes.ResponseWrappers.Shopping;

namespace InstaSharper.Converters.Shopping
{
    internal class InstaProductContainerConverter : IObjectConverter<InstaProductTag, InstaProductContainerResponse>
    {
        public InstaProductContainerResponse SourceObject { get; set; }

        public InstaProductTag Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException($"Source object");
            var productTag = new InstaProductTag
            {
                Product = ConvertersFabric.Instance.GetProductConverter(SourceObject.Product).Convert()
            };

            if (SourceObject.Position != null)
                productTag.Position = new InstaPosition(SourceObject.Position[0], SourceObject.Position[1]);

            return productTag;
        }
    }
}
