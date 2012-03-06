using System;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ProjectDepsVisualizer.Core
{
  public class VersionFileAnalyzer
  {
    public string GetProjectVersion(string versionFileContents)
    {
      if (versionFileContents == null) throw new ArgumentNullException("versionFileContents");

      XDocument xDocument = XDocument.Parse(versionFileContents);
      string major = null;
      string minor = null;

      foreach (XElement xElement in xDocument.XPathSelectElements("project/property"))
      {
        string name = xElement.Attribute("name").Value;
        string value = xElement.Attribute("value").Value;

        switch (name.ToUpper())
        {
          case "VERSION.MAJOR": major = value; break;
          case "VERSION.MINOR": minor = value; break;
          case "VERSION.HOTFIX": break; // don't care
          default: break;
        }
      }

      if (major == null || minor == null)
      {
        return null;
      }

      return string.Format("{0}.{1}", major, minor);
    }
  }
}
