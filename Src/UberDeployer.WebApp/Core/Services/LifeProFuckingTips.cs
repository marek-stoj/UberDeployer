using System;

namespace UberDeployer.WebApp.Core.Services
{
  public class LifeProFuckingTips
  {
    private static readonly string[] _Tips =
      new[]
        {
          "When in doubt, shut the fuck up.",
          "Always go for the decision that will make for a better fucking story.",
          "Turn off your fucking brights when oncoming traffic is present.",
          "Make a list if you’re having trouble getting shit done.",
          "Spend more money on experiences and less on fucking possessions.",
          "Wearing glasses without lenses makes you look like an asshole.",
          "Spend more time doing fun shit.",
          "Full beard, stubble, or shave. goatees are for douchebags.",
          "You get better customer service when you’re fucking polite.",
          "Try turning it off and on before calling fucking tech support.",
          "Don’t fucking text so much when you’re with people.",
          "No one remembers all your fuck ups except you.",
          "Correcting someone’s grammar makes you look like a dick.",
          "Don’t waste your fucking time being jealous of others.",
          "Stop blaming others for your fucking troubles.",
          "Nice guys don’t fucking finish last, boring guys do.",
          "Don’t ever start fucking smoking.",
          "Do not deal drugs, no matter how fucking desperate for money you are.",
          "Don’t cry after sex. it’s fucking weird.",
          "Don’t be the fucking person who wasted their talent because they didn’t work hard.",
          "Chew sunflower seeds on long drives to avoid falling the fuck asleep.",
          "To avoid embarrassing emergency room situations, don’t stick household items in your ass.",
          "Don’t spend more money than you fucking earn.",
          "Spend holidays with your goddamn family.",
          "Tailor your fucking dress clothes.",
          "The vast majority of people don’t give a fuck about you.",
          "Pictures on the internet are forever. think twice before posting that shit.",
        };

    public static string GetTodayTip()
    {
      return _Tips[(DateTime.UtcNow.Day - 1) % _Tips.Length];
    }
  }
}
