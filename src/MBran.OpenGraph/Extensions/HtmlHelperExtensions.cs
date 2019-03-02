using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MBran.OpenGraph.Models;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Composing = Umbraco.Web.Composing;

namespace MBran.OpenGraph.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString OpenGraph(this HtmlHelper helper,
            Models.OpenGraph opengraph)
        {
            return helper.OpenGraph(opengraph.ToList());
        }

        public static MvcHtmlString OpenGraph(this HtmlHelper helper, string propertyName,
            Models.OpenGraph opengraph)
        {
            var curPage = GetCurrentPage();
            return helper.OpenGraph(curPage, propertyName, opengraph.ToList());
        }

        public static MvcHtmlString OpenGraph(this HtmlHelper helper,
            IEnumerable<dynamic> defaultMetadata = null)
        {
            var curPage = GetCurrentPage();
            var cacheName = string.Join("_", nameof(HtmlHelperExtensions), nameof(OpenGraph),
               curPage.ContentType.Alias);

            var currentCachedPropertyName = (string) Composing.Current.AppCaches
                .RuntimeCache
                .Get(cacheName);

            //clear if there is previous cache but property is gone
            if (!string.IsNullOrWhiteSpace(currentCachedPropertyName) &&
                !curPage.HasProperty(currentCachedPropertyName))
                Composing.Current.AppCaches
                    .RuntimeCache
                    .Clear(cacheName);

            //retry if no cache available or if previous cached property does not exist
            if (string.IsNullOrWhiteSpace(currentCachedPropertyName)
                || !curPage.HasProperty(currentCachedPropertyName))
                currentCachedPropertyName = (string)Composing.Current.AppCaches
                    .RuntimeCache
                    .Get(cacheName, () => GetPropertyName(curPage));

            return helper.OpenGraph(curPage, currentCachedPropertyName, defaultMetadata);
        }

        public static MvcHtmlString OpenGraph(this HtmlHelper helper, string propertyName,
            IEnumerable<dynamic> defaultMetadata = null)
        {
            var curPage = GetCurrentPage();
            return helper.OpenGraph(curPage, propertyName, defaultMetadata);
        }

        private static MvcHtmlString OpenGraph(this HtmlHelper helper,
            IPublishedContent content,
            string propertyName,
            IEnumerable<dynamic> defaultMetadata = null)
        {
            var metaData = !string.IsNullOrWhiteSpace(propertyName) && content.HasProperty(propertyName)
                ? (List<OpenGraphMetaData>)content.GetProperty(propertyName).GetValue()
                : new List<OpenGraphMetaData>();

            var defaultMeta = defaultMetadata
                ?.Select(m => JsonConvert.DeserializeObject<OpenGraphMetaData>(JsonConvert.SerializeObject(m) as string))
                .WhereNotNull()
                .Where(d => !metaData.Any(m =>
                    m.Key.Equals(d.Key, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            if (defaultMeta != null) metaData.AddRange(defaultMeta);

            var htmlString = metaData
                .Select(m => $@"<meta property=""{m.Key}"" content=""{HttpUtility.HtmlEncode(m.Value)}"">")
                .ToList();


            return MvcHtmlString.Create(string.Join(string.Empty, htmlString));
        }

        private static string GetPropertyName(IPublishedContent content)
        {
            return content.Properties
                .FirstOrDefault(p => (List<OpenGraphMetaData>)p.GetValue() != null)
                ?.Alias;
        }

        private static IPublishedContent GetCurrentPage()
        {
            return Composing.Current.UmbracoContext.PublishedRequest.PublishedContent;
        }
    }
}