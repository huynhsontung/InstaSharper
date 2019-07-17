using System.Linq;
using InstaSharper.Classes.Models.Collection;
using InstaSharper.Classes.Models.Media;
using InstaSharper.Classes.ResponseWrappers.Collection;

namespace InstaSharper.Converters.Collections
{
    internal class InstaCollectionConverter : IObjectConverter<InstaCollectionItem, InstaCollectionItemResponse>
    {
        public InstaCollectionItemResponse SourceObject { get; set; }

        public InstaCollectionItem Convert()
        {
            var instaMediaList = new InstaMediaList();

            if (SourceObject.Media != null)
                instaMediaList.AddRange(SourceObject.Media.Medias
                    .Select(ConvertersFabric.Instance.GetSingleMediaConverter)
                    .Select(converter => converter.Convert()));

            return new InstaCollectionItem
            {
                CollectionId = SourceObject.CollectionId,
                CollectionName = SourceObject.CollectionName,
                HasRelatedMedia = SourceObject.HasRelatedMedia,
                Media = instaMediaList,
                CoverMedia = SourceObject.CoverMedia != null
                    ? ConvertersFabric.Instance.GetCoverMediaConverter(SourceObject.CoverMedia).Convert()
                    : null,
                NextMaxId = SourceObject.NextMaxId
            };
        }
    }
}