using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Serialization;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace SimpleJobsPlus
{
    public class JobsPlugin : RocketPlugin<JobsConfig>
    {
        public static JobsPlugin Instance;

        public PlayerJobsDatabase Database = new PlayerJobsDatabase();
        private string _dataPath;

        protected override void Load()
        {
            Instance = this;
            _dataPath = Path.Combine(Directory, "PlayerJobs.xml");
            LoadDatabase();

            StartCoroutine(SalaryLoop());

            Logger.Log("SimpleJobsPlus loaded.");
        }

        protected override void Unload()
        {
            SaveDatabase();
            Logger.Log("SimpleJobsPlus unloaded.");
        }

        #region Persistence

        public void LoadDatabase()
        {
            if (File.Exists(_dataPath))
            {
                try
                {
                    Database = XML.Deserialize<PlayerJobsDatabase>(_dataPath);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Failed to load PlayerJobs.xml: {ex.Message}");
                    Database = new PlayerJobsDatabase();
                }
            }
            else
            {
                Database = new PlayerJobsDatabase();
                SaveDatabase();
            }
        }

        public void SaveDatabase()
        {
            try
            {
                XML.Serialize(Database, _dataPath);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to save PlayerJobs.xml: {ex.Message}");
            }
        }

        #endregion

        #region Core job logic

        public PlayerJobData GetData(UnturnedPlayer player)
        {
            return Database.GetOrCreate(player.CSteamID.ToString());
        }

        public JobDefinition GetJobDef(string jobName)
        {
            return Configuration.Instance.GetJob(jobName);
        }

        public void SetJob(UnturnedPlayer player, string jobName)
        {
            var def = GetJobDef(jobName);
            if (def == null)
            {
                UnturnedChat.Say(player, "That job does not exist.");
                return;
            }

            var data = GetData(player);
            data.JobName = def.Name;
            if (data.Level == 0) data.Level = 1;
            SaveDatabase();

            ApplyUniform(player, def);

            UnturnedChat.Say(player, $"You are now a {def.Name}.");
        }

        public string GetJobName(UnturnedPlayer player)
        {
            return GetData(player).JobName ?? "Unemployed";
        }

        public uint GetLevel(UnturnedPlayer player)
        {
            return GetData(player).Level;
        }

        public void AddXP(UnturnedPlayer player, uint baseXP)
        {
            var data = GetData(player);
            var def = GetJobDef(data.JobName);
            float multiplier = def != null ? def.XPMultiplier : 1f;
            uint gained = (uint)Mathf.RoundToInt(baseXP * multiplier);

            data.XP += gained;

            if (def != null)
            {
                while (data.Level < def.MaxLevel && data.XP >= def.XPPerLevel)
                {
                    data.XP -= def.XPPerLevel;
                    data.Level++;
                    UnturnedChat.Say(player, $"Job level up! {def.Name} level {data.Level}.");
                }
            }

            SaveDatabase();
        }

        #endregion

        #region Salaries

        private IEnumerator SalaryLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);

                foreach (var steamPlayer in Provider.clients)
                {
                    var player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    var data = GetData(player);
                    var def = GetJobDef(data.JobName);
                    if (def == null) continue;
                    if (def.SalaryXP == 0 || def.SalaryIntervalSeconds == 0) continue;

                    var now = DateTime.UtcNow;
                    if ((now - data.LastSalaryTime).TotalSeconds >= def.SalaryIntervalSeconds)
                    {
                        data.LastSalaryTime = now;
                        player.Experience += def.SalaryXP;
                        UnturnedChat.Say(player, $"You received {def.SalaryXP} XP salary for your job as {def.Name}.");
                    }
                }

                SaveDatabase();
            }
        }

        #endregion

        #region Uniforms

        private void ApplyUniform(UnturnedPlayer player, JobDefinition def)
        {
            if (def.ShirtID != 0) player.Player.clothing.askWearShirt(def.ShirtID, 0, new byte[0], true);
            if (def.PantsID != 0) player.Player.clothing.askWearPants(def.PantsID, 0, new byte[0], true);
            if (def.HatID != 0) player.Player.clothing.askWearHat(def.HatID, 0, new byte[0], true);
            if (def.VestID != 0) player.Player.clothing.askWearVest(def.VestID, 0, new byte[0], true);
            if (def.BackpackID != 0) player.Player.clothing.askWearBackpack(def.BackpackID, 0, new byte[0], true);
        }

        #endregion

        #region Daily task

        public bool TryCompleteDailyTask(UnturnedPlayer player)
        {
            var data = GetData(player);
            var def = GetJobDef(data.JobName);
            if (def == null || def.DailyTaskRewardXP == 0)
            {
                UnturnedChat.Say(player, "Your job has no daily task configured.");
                return false;
            }

            var now = DateTime.UtcNow;
            if ((now - data.LastDailyTaskTime).TotalHours < 24)
            {
                UnturnedChat.Say(player, "You have already completed your daily task today.");
                return false;
            }

            data.LastDailyTaskTime = now;
            AddXP(player, def.DailyTaskRewardXP);
            UnturnedChat.Say(player, $"Daily task completed! You earned {def.DailyTaskRewardXP} job XP.");
            SaveDatabase();
            return true;
        }

        #endregion

        #region RP helpers

        public string GetRoleTag(UnturnedPlayer player)
        {
            var def = GetJobDef(GetJobName(player));
            return def != null && !string.IsNullOrEmpty(def.RoleTag) ? def.RoleTag : "";
        }

        public Color GetJobChatColor(UnturnedPlayer player)
        {
            var def = GetJobDef(GetJobName(player));
            if (def == null || string.IsNullOrEmpty(def.JobChatColor))
                return Color.cyan;

            Color color;
            if (ColorUtility.TryParseHtmlString(def.JobChatColor, out color))
                return color;

            return Color.cyan;
        }

        #endregion

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "job_chat_format", "{0} {1}: {2}" } // {roleTag} {playerName}: {message}
        };
    }
}
