using System;
using System.Collections.Generic;

namespace SimpleJobsPlus
{
    public class PlayerJobData
    {
        public string SteamId;
        public string JobName;
        public uint Level;
        public uint XP;
        public DateTime LastSalaryTime;
        public DateTime LastDailyTaskTime;
    }

    public class PlayerJobsDatabase
    {
        public List<PlayerJobData> Players = new List<PlayerJobData>();

        public PlayerJobData GetOrCreate(string steamId)
        {
            var data = Players.Find(p => p.SteamId == steamId);
            if (data == null)
            {
                data = new PlayerJobData()
                {
                    SteamId = steamId,
                    JobName = "Unemployed",
                    Level = 1,
                    XP = 0,
                    LastSalaryTime = DateTime.MinValue,
                    LastDailyTaskTime = DateTime.MinValue
                };
                Players.Add(data);
            }
            return data;
        }
    }
}
