using System;

namespace sistema.Utilidades
{
    public static class CatalogUrlHelper
    {
        public static bool TryGetSafeUrl(string rawUrl, out string safeUrl)
        {
            safeUrl = Normalize(rawUrl);
            return safeUrl != null;
        }

        public static string Normalize(string rawUrl)
        {
            if (string.IsNullOrWhiteSpace(rawUrl))
                return null;

            var url = rawUrl.Trim();

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return null;

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return null;

            if (string.IsNullOrWhiteSpace(uri.Host) || IsBlockedHost(uri.Host))
                return null;

            return uri.ToString();
        }

        private static bool IsBlockedHost(string host)
        {
            if (string.Equals(host, "0.0.0.0", StringComparison.OrdinalIgnoreCase)
                || string.Equals(host, "0.0.0.1", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
