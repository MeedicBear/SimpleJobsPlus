using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace SimpleJobsPlus
{
    public class CommandJobChat : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "jobchat";
        public string Help => "Talk only to players with the same job";
        public string Syntax => "/jobchat <message>";
        public List<string> Aliases => new List<string>() { "jc" };
        public List<string> Permissions => new List<string>() { "jobs.chat" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                UnturnedChat.Say(player, "Usage: /jobchat <message>");
                return;
            }

            string msg = string.Join(" ", command);
            string jobName = JobsPlugin.Instance.GetJobName(player);
            string roleTag = JobsPlugin.Instance.GetRoleTag(player);
            var color = JobsPlugin.Instance.GetJobChatColor(player);

            foreach (var sp in Provider.clients)
            {
                var target = UnturnedPlayer.FromSteamPlayer(sp);
                if (JobsPlugin.Instance.GetJobName(target).ToLower() == jobName.ToLower())
                {
                    UnturnedChat.Say(target,
                        JobsPlugin.Instance.Translate("job_chat_format", roleTag, player.DisplayName, msg),
                        color);
                }
            }
        }
    }
}
