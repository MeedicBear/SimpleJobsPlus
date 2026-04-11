using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace SimpleJobsPlus
{
    public class CommandMyJob : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "myjob";
        public string Help => "Shows your current job, level and XP";
        public string Syntax => "/myjob";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;
            var data = JobsPlugin.Instance.GetData(player);
            var jobName = JobsPlugin.Instance.GetJobName(player);

            UnturnedChat.Say(player, $"Job: {jobName} | Level: {data.Level} | XP: {data.XP}");
        }
    }
}
