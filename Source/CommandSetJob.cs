using Rocket.API;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace SimpleJobsPlus
{
    public class CommandSetJob : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "setjob";
        public string Help => "Choose a job";
        public string Syntax => "/setjob <job>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "jobs.set" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            var player = (UnturnedPlayer)caller;

            if (command.Length < 1)
            {
                UnturnedChat.Say(player, "Usage: /setjob <job>");
                return;
            }

            string job = command[0];
            JobsPlugin.Instance.SetJob(player, job);
        }
    }
}
