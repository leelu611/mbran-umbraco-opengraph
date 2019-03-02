using System;
using System.Collections.Generic;
using MBran.OpenGraph.Extensions;
using MBran.OpenGraph.Models;
using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace MBran.OpenGraph
{
    public class OpenGraphValueConverter : PropertyValueConverterBase
    {
        private const string Alias = "MBran.OpenGraph";

        public override bool IsConverter(PublishedPropertyType propertyType) => Alias.Equals(propertyType.EditorAlias);
        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.Snapshot;

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview) => source?.ToString();

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType,
            PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            var opengraph = JsonConvert.DeserializeObject<Models.OpenGraph>(inter.ToString());
            return opengraph == null ? new List<OpenGraphMetaData>() : opengraph.ToList();

        }
    }
}