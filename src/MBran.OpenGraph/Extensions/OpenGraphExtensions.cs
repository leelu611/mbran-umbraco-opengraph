using System.Collections.Generic;
using MBran.OpenGraph.Models;
using Umbraco.Web;
using Composing = Umbraco.Web.Composing;

namespace MBran.OpenGraph.Extensions
{
    public static class OpenGraphExtensions
    {
        public static IEnumerable<OpenGraphMetaData> ToList(this Models.OpenGraph opengraph)
        {
            var model = new List<OpenGraphMetaData>();

            if (opengraph == null) return model;

            if (opengraph.ImageId != null)
            {
                var media = Composing.Current.UmbracoHelper.Media(opengraph.ImageId);
                var mediaUrl = media.Url;
                if (!string.IsNullOrEmpty(mediaUrl))
                {
                    var url = Composing.Current.UmbracoContext
                        .HttpContext.Request.Url?.AbsoluteUri
                        .TrimEnd('/');
                    model.Add(new OpenGraphMetaData
                    {
                        Key = "og:image",
                        Value = url + mediaUrl
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(opengraph.Title))
                model.Add(new OpenGraphMetaData
                {
                    Key = "og:title",
                    Value = opengraph.Title
                });
            if (!string.IsNullOrWhiteSpace(opengraph.Type))
                model.Add(new OpenGraphMetaData
                {
                    Key = "og:type",
                    Value = opengraph.Type
                });
            if (!string.IsNullOrWhiteSpace(opengraph.Description))
                model.Add(new OpenGraphMetaData
                {
                    Key = "og:description",
                    Value = opengraph.Description
                });

            model.AddRange(opengraph.Metadata);
            return model;
        }
    }
}