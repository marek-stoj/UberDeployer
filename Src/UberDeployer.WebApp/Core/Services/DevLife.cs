using System;

namespace UberDeployer.WebApp.Core.Services
{
  public class DevLife
  {
    private const string _BaseUrl = "https://raw.github.com/manifestinteractive/devlife/master/DevLife.AdiumEmoticonSet";

    private static readonly Tuple<string, string>[] _Gifs =
      new[]
      {
        new Tuple<string, string>(_BaseUrl + "/breeze_by.gif", "When the code that I have not tested on dev works perfectly in production"),
        new Tuple<string, string>(_BaseUrl + "/bug_reports.gif", "When the app goes into beta and the first bug reports arrive"),
        new Tuple<string, string>(_BaseUrl + "/carlton.gif", "When I find a solution without searching Google"),
        new Tuple<string, string>(_BaseUrl + "/celebrate.gif", "When the sales people announce they have sold our product to the customer"),
        new Tuple<string, string>(_BaseUrl + "/chuck_approves.gif", "When I did a good job and the client is happy"),
        new Tuple<string, string>(_BaseUrl + "/deploy_code.gif", "When I'm deploying code to production"),
        new Tuple<string, string>(_BaseUrl + "/disbelief.gif", "When I see someone commit 500 files to fix one bug"),
        new Tuple<string, string>(_BaseUrl + "/evil_grin.gif", "When a bug goes unnoticed during a presentation"),
        new Tuple<string, string>(_BaseUrl + "/face_palm.gif", "When they tell me the website has to be supported by IE6"),
        new Tuple<string, string>(_BaseUrl + "/fail_1.gif", "When I launch my script for the first time after several hours of development"),
        new Tuple<string, string>(_BaseUrl + "/fail_2.gif", "When after a productive day I'm so confident that I think I can handle parkour tricks on the way home"),
        new Tuple<string, string>(_BaseUrl + "/fail_3.gif", "When I start coding without doing analysis first"),
        new Tuple<string, string>(_BaseUrl + "/fail_4.gif", "When project manager thinks that I can handle whole project all by myself"),
        new Tuple<string, string>(_BaseUrl + "/fixed_bug.gif", "When I show the boss that I have finally fixed this bug"),
        new Tuple<string, string>(_BaseUrl + "/freedom.gif", "When sysadmin finally gives us the root access"),
        new Tuple<string, string>(_BaseUrl + "/hiding.gif", "When the boss is looking for someone to urgently fix a difficult bug"),
        new Tuple<string, string>(_BaseUrl + "/i_did_what.gif", "When I'm told that my code is broken in production"),
        new Tuple<string, string>(_BaseUrl + "/i_saw_that.gif", "When the project manager suddenly looks on my screen"),
        new Tuple<string, string>(_BaseUrl + "/im_sure.gif", "When they me ask if I have tested it"),
        new Tuple<string, string>(_BaseUrl + "/it_worked.gif", "When my script finally worked"),
        new Tuple<string, string>(_BaseUrl + "/ive_got_this.gif", "When my regex returned exactly what I expected"),
        new Tuple<string, string>(_BaseUrl + "/mmm_hmm.gif", "When a friend of mine asks me to fix his website built with Joomla"),
        new Tuple<string, string>(_BaseUrl + "/my_bad.gif", "When I realize that I have been blocked for two hours because of a forgotten semicolon"),
        new Tuple<string, string>(_BaseUrl + "/new_feature.gif", "When I apply a new CSS for the first time"),
        new Tuple<string, string>(_BaseUrl + "/nope.gif", "When a newbie suggests to add a new feature to project"),
        new Tuple<string, string>(_BaseUrl + "/not_working.gif", "When the client tries to click on the mockups"),
        new Tuple<string, string>(_BaseUrl + "/ok.gif", "When customer wants to change specification 2 days before pushing to production"),
        new Tuple<string, string>(_BaseUrl + "/panic.gif", "When I try a solution without reading the docs first"),
        new Tuple<string, string>(_BaseUrl + "/peace_out.gif", "When I go off for the weekend while everyone else is still trying to fix bugs"),
        new Tuple<string, string>(_BaseUrl + "/pretend_work.gif", "When the project manager enters the office"),
        new Tuple<string, string>(_BaseUrl + "/punch_self.gif", "When I return to development of my code that wasn't commented"),
        new Tuple<string, string>(_BaseUrl + "/sad_baby.gif", "When I'm told that the module on which I have worked all the week will never be used"),
        new Tuple<string, string>(_BaseUrl + "/shut_it.gif", "When the intern tells me that \"the tests are for those who can not program\""),
        new Tuple<string, string>(_BaseUrl + "/sleepy.gif", "When I try to fix a bug at 3 in the morning"),
        new Tuple<string, string>(_BaseUrl + "/sneak_peak.gif", "When I am the only one to notice a bug on Friday night"),
        new Tuple<string, string>(_BaseUrl + "/that_a_boy.gif", "When I manage to replace 200 lines of the algorithm by only 10 lines"),
        new Tuple<string, string>(_BaseUrl + "/throw_computer.gif", "When a thing that worked on Friday no longer works on Monday"),
        new Tuple<string, string>(_BaseUrl + "/tough_love.gif", "When asked to lend a hand on a Friday afternoon"),
        new Tuple<string, string>(_BaseUrl + "/upgrade.gif", "When the boss announces a bonus if the project is completed before the deadline"),
        new Tuple<string, string>(_BaseUrl + "/what_fail.gif", "When I notice I was editing production code and I quickly correct my errors"),
        new Tuple<string, string>(_BaseUrl + "/whats_up.gif", "Ninja-fixing a bug 10 minutes before the demo for client"),
        new Tuple<string, string>(_BaseUrl + "/wtf.gif", "When I am asked to continue work of a newbie colleague"),
        new Tuple<string, string>(_BaseUrl + "/you_did_what.gif", "When I find out that someone has accidentally overwritten my changes in git"),
        new Tuple<string, string>(_BaseUrl + "/rethink.gif", "When I realize the solution i've been working on for day is really the wrong approach"),
      };

    public static Tuple<string, string> GetTodayGif()
    {
      return _Gifs[(DateTime.UtcNow.DayOfYear + 1) % _Gifs.Length];
    }
  }
}
