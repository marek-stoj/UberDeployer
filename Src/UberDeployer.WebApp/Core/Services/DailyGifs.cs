using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

namespace UberDeployer.WebApp.Core.Services
{
  public class DailyGifs
  {
    private static readonly TimeSpan _GifsLoadInterval = TimeSpan.FromHours(6);

    private static DateTime? _UtcDateLastGifsLoaded;
    private static DailyGif _todayGif;

    public static DailyGif GetTodayGif()
    {
      LoadGifsFromDevOpsReactionsIfNeeded();

      return _todayGif;
    }

    private static void LoadGifsFromDevOpsReactionsIfNeeded()
    {
      if (_todayGif != null &&
          _UtcDateLastGifsLoaded.HasValue &&
          (DateTime.UtcNow - _UtcDateLastGifsLoaded.Value < _GifsLoadInterval))
      {
        return;
      }

      string feedContent;

      using (var wc = new WebClient())
      {
        feedContent = wc.DownloadString("http://devopsreactions.tumblr.com/rss");
      }

      SyndicationFeed syndicationFeed;

      using (var sr = new StringReader(feedContent))
      using (XmlReader xr = XmlReader.Create(sr))
      {
        syndicationFeed = SyndicationFeed.Load(xr);
      }

      if (syndicationFeed == null)
      {
        throw new InternalException("Couldn't load DevOps Reactions RSS Feed.");
      }

      _UtcDateLastGifsLoaded = DateTime.UtcNow;

      SyndicationItem syndicationItem = syndicationFeed.Items.First();

      _todayGif =
        new DailyGif
        {
          Url = ExtractGifLink(syndicationItem.Summary.Text),
          Description = syndicationItem.Title.Text,
        };
    }

    private static string ExtractGifLink(string s)
    {
      return Regex.Match(s, "(?<Url>https?://.+?\\.gif)").Groups["Url"].Value;
    }
  }
}
