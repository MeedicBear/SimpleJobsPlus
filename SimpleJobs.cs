using System;
using System.Collections.Generic;
using System.Timers;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Core;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.Core.Logging;
using UnityEngine;
using SDG.Unturned;
using Rocket.Core.Commands;
using Steamworks;

namespace SimpleJobsPlus
{
    public class SimpleJobsPlus : RocketPlugin<JobConfig>
    {
        public static SimpleJobsPlus Instance;
        private Timer paycheckTimer;
        private Dictionary<string, Vector3> lastPlayerPositions = new Dictionary<string, Vector3>();

        protected override void Load()
        {
            Instance = this;

            // Initialize the Salary Timer
            paycheckTimer = new Timer(Configuration.Instance.PaycheckIntervalSeconds * 1000);
            paycheckTimer.Elapsed += OnPaycheckTick;
            paycheckTimer.AutoReset = true;
            paycheckTimer.Enabled = true;

            // Use the full path to be safe
            Rocket.Core.Logging.Logger.Log("--------------------------------------");
            Rocket.Core.Logging.Logger.Log("SimpleJobsPlus Loaded! EXP Salaries Active.");
            Rocket.Core.Logging.Logger.Log("--------------------------------------");
        }

        protected override void Unload()
        {
            paycheckTimer.Stop();
            paycheckTimer.Dispose();
            lastPlayerPositions.Clear();

            Rocket.Core.Logging.Logger.Log("SimpleJobsPlus Unloaded.");
        }

        private void OnPaycheckTick(object sender, ElapsedEventArgs e)
        {
            // We use the Dispatcher to ensure we don't crash the game when modifying player stats from a timer thread
            Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(() =>
            {
                foreach (var steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    if (player == null) continue;

                    string playerId = player.CSteamID.ToString();

                    // --- AFK CHECK ---
                    if (lastPlayerPositions.ContainsKey(playerId))
                    {
                        float distanceMoved = Vector3.Distance(lastPlayerPositions[playerId], player.Position);
                        if (distanceMoved < 5f)
                        {
                            UnturnedChat.Say(player, "You missed a paycheck for being AFK!", Color.red);
                            lastPlayerPositions[playerId] = player.Position;
                            continue;
                        }
                    }
                    lastPlayerPositions[playerId] = player.Position;

                    // --- PAYROLL CHECK ---
                    foreach (var job in Configuration.Instance.Jobs)
                    {
                        if (player.GetPermissions().Exists(p => p.Name.ToLower() == job.Id.ToLower()))
                        {
                            player.Experience += job.Salary;

                            // Plays the "Quest Completed/Reward" sound and effect at the player's position
                            player.TriggerEffect(21);

                            UnturnedChat.Say(player, $"[ {job.DisplayName} ] You received {job.Salary} EXP!", Color.yellow);
                            break; 
                        }
                    }
                }
            });
        }

        // --- THE JOB COMMAND ---
        [RocketCommand("job", "Join a role", "/job <join> <id>", AllowedCaller.Player)]
        public void ExecuteJobCommand(IRocketPlayer caller, string[] parameters)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (parameters.Length < 2)
            {
                UnturnedChat.Say(player, "Syntax: /job join <jobname>", Color.red);
                return;
            }

            // Get the first word (e.g., "join") and the second word (e.g., "police")
            string action = parameters.ToString().ToLower();
            string jobId = parameters.ToString().ToLower();

            if (action == "join")
            {
                // Find if the job exists in our config
                var selectedJob = Configuration.Instance.Jobs.Find(j => j.Id.ToLower() == jobId);
                
                if (selectedJob == null)
                {
                    UnturnedChat.Say(player, "That job doesn't exist!", Color.red);
                    return;
                }

                // Logic: Add player to the RocketMod group
                // Note: Ensure these groups exist in your Permissions.config.xml!
                Rocket.Core.R.Permissions.AddPlayerToGroup(selectedJob.Id, player);
                
                // Vibe Polish: Visual effect and sound
                player.TriggerEffect(21); // Quest completed effect
                UnturnedChat.Say(player, $"Welcome to the team! You are now a {selectedJob.DisplayName}.", Color.cyan);
            }
        }
    }
}