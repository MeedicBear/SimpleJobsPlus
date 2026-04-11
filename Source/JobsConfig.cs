using Rocket.API;
using System.Collections.Generic;

namespace SimpleJobsPlus
{
    public class JobDefinition
    {
        public string Name;
        public uint SalaryXP;
        public uint SalaryIntervalSeconds;
        public float XPMultiplier;
        public uint XPPerLevel;
        public uint MaxLevel;

        // Uniform (0 = ignore)
        public ushort ShirtID;
        public ushort PantsID;
        public ushort HatID;
        public ushort VestID;
        public ushort BackpackID;

        // RP
        public string JobChatColor; // e.g. "#00AAFF"
        public string RoleTag;      // e.g. "[Police]"

        // Daily task
        public string DailyTaskDescription;
        public uint DailyTaskRewardXP;
    }

    public class JobsConfig : IRocketPluginConfiguration
    {
        public List<JobDefinition> Jobs;

        public void LoadDefaults()
        {
            Jobs = new List<JobDefinition>()
            {
                new JobDefinition()
                {
                    Name = "Police",
                    SalaryXP = 25,
                    SalaryIntervalSeconds = 300,
                    XPMultiplier = 1.2f,
                    XPPerLevel = 100,
                    MaxLevel = 10,
                    ShirtID = 300,
                    PantsID = 301,
                    HatID = 302,
                    VestID = 303,
                    BackpackID = 0,
                    JobChatColor = "#00AAFF",
                    RoleTag = "[Police]",
                    DailyTaskDescription = "Arrest a criminal (RP)",
                    DailyTaskRewardXP = 50
                },
                new JobDefinition()
                {
                    Name = "Farmer",
                    SalaryXP = 20,
                    SalaryIntervalSeconds = 300,
                    XPMultiplier = 1.1f,
                    XPPerLevel = 80,
                    MaxLevel = 10,
                    ShirtID = 310,
                    PantsID = 311,
                    HatID = 0,
                    VestID = 0,
                    BackpackID = 0,
                    JobChatColor = "#55AA00",
                    RoleTag = "[Farmer]",
                    DailyTaskDescription = "Harvest crops",
                    DailyTaskRewardXP = 40
                }
            };
        }

        public JobDefinition GetJob(string name)
        {
            if (Jobs == null) return null;
            return Jobs.Find(j => j.Name.ToLower() == name.ToLower());
        }
    }
}
