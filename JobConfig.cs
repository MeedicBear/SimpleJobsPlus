using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SimpleJobsPlus
{
    public class JobConfig : IRocketPluginConfiguration
    {
        public string MessageColor;
		public uint PaycheckIntervalSeconds; // How often to pay
        public List<Job> Jobs;

        public void LoadDefaults()
        {
            MessageColor = "yellow";
			PaycheckIntervalSeconds = 600; // Default to 10 minutes
            Jobs = new List<Job>
            {
                new Job { Id = "Police", Salary = 100 },
                new Job { Id = "EMS", Salary = 80 },
                new Job { Id = "Theif", Salary = 80 },
                new Job { Id = "LumberJack", Salary = 80 },
                new Job { Id = "Miner", Salary = 80 },
                new Job { Id = "Fisherman", Salary = 80 },
                new Job { Id = "Bounty Hunter", Salary = 80 },
                new Job { Id = "Farmer", Salary = 80 },
                new Job { Id = "Mechanic", Salary = 80 },
                new Job { Id = "Taxi Driver", Salary = 80 },
                new Job { Id = "Delivery Driver", Salary = 80 }
            };
        }
    }

    public class Job
	{
    public string Id;
    public string DisplayName;
    public uint Salary; // Amount of EXP
	}
}