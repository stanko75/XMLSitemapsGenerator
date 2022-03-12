using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace XMLSitemapsGenerator
{
  class Program
  {
    static void Main(string[] args)
    {
      Uri domain = new UriBuilder("www.milosev.com").Uri;
      List<XmlSiteMapModel.Url> urls = new List<XmlSiteMapModel.Url>();
      //add urls
      AddUrls(domain, domain, urls);

      //urls.Add(new XmlSiteMapModel.Url
      //{
      //  Changefreq = "monthly", Lastmod = "2005-01-01", Loc = "uriLink.AbsoluteUri", Priority = "0.8"
      //});

      //save to XML
      SaveToXml(urls);
    }

    private static void AddUrls(Uri uri, Uri domain, List<XmlSiteMapModel.Url> urls)
    {
      try
      {
        HtmlWeb hw = new HtmlWeb();
        HtmlDocument doc = hw.Load(uri);
        HtmlNodeCollection htmlNodeCollection = doc.DocumentNode.SelectNodes("//a[@href]");
        if (htmlNodeCollection is not null)
        {
          foreach (HtmlNode link in htmlNodeCollection)
          {
            string href = link.Attributes["href"].Value;
            Uri uriLink = GetAbsoluteUrlString(domain, href);

            if (uriLink.AbsoluteUri.Contains(domain.AbsoluteUri))
            {
              if (urls.All(url => url.Loc != uriLink.AbsoluteUri))
              {
                urls.Add(new XmlSiteMapModel.Url
                {
                  Changefreq = "monthly", Lastmod = DateTime.Now.ToString("yyyy-MM-dd"), Loc = uriLink.AbsoluteUri,
                  Priority = "0.8"
                });
                Console.WriteLine(uriLink.AbsoluteUri);
                AddUrls(uriLink, domain, urls);
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error: {e.Message}");
      }
    }

    static Uri GetAbsoluteUrlString(Uri baseUrl, string url)
    {
      Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);
      if (!uri.IsAbsoluteUri)
        uri = new Uri(baseUrl, uri);
      return uri;
    }

    static void SaveToXml(List<XmlSiteMapModel.Url> urls)
    {
      XmlSiteMapModel.Urlset serializedProject = new XmlSiteMapModel.Urlset
      {
        Url = urls
        , SchemaLocation = "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"
      };

      TextWriter txtWriter = new StreamWriter(Path.ChangeExtension(typeof(Program).Assembly.Location, ".xml"));

      XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
      ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
      ns.Add("xhtml", "http://www.w3.org/1999/xhtml");
      ns.Add(string.Empty, "http://www.sitemaps.org/schemas/sitemap/0.9");

      XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlSiteMapModel.Urlset), "http://www.sitemaps.org/schemas/sitemap/0.9");
      xmlSerializer.Serialize(txtWriter, serializedProject, ns);

      txtWriter.Close();
    }

  }
}