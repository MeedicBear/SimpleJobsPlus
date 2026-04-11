using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace SimpleJobsPlus
{
    public class CommandDailyTask : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "jobtask";
        public string Help => "Shows or completes your daily job task";
        public string Syntax => "/jobtask [complete]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "jobs.task" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;
            var def = JobsPlugin.Instance.GetJobDef(JobsPlugin.Instance.GetJobName(player));

            if (def == null)
            {
                UnturnedChat.Say(player, "You do not have a job.");
                return;
            }

            if (command.Length == 0)
            {
                UnturnedChat.Say(player, $"Daily task for {def.Name}: {def.DailyTaskDescription}");
                return;
            }

            if (command[0].ToLower() == "complete")
            {
                JobsPlugin.Instance.TryCompleteDailyTask(player);
            }
            else
            {
                UnturnedChat.Say(player, "Usage: /jobtask [complete]");
            }
        }
    }
}
