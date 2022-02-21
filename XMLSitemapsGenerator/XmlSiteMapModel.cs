using System.Collections.Generic;
using System.Xml.Serialization;

namespace XMLSitemapsGenerator
{
  public class XmlSiteMapModel
  {
    [XmlRoot("urlset")]
    public class Urlset
    {
      [XmlElement(ElementName = "url")] public List<Url> Url { get; set; }
    }

    public class Url
    {
      [XmlElement(ElementName = "loc")] public string Loc { get; set; }

      [XmlElement(ElementName = "lastmod")] public string Lastmod { get; set; }

      [XmlElement(ElementName = "changefreq")] public string Changefreq { get; set; }

      [XmlElement(ElementName = "priority")] public string Priority { get; set; }
    }
  }
}