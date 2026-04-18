# SimpleJobs 🛠️

A lightweight job system for Unturned servers running **RocketMod**. SimpleJobs focuses on active roleplay by rewarding players with EXP salaries while preventing AFK farming with a built-in motion tracker.

## ✨ Features

* **EXP-Based Salaries:** Reward your players with Experience points for their hard work.
* **AFK Protection:** Players must move at least 5 meters between paychecks to earn their salary.
* **Job-Specific Groups:** Automatically links with your RocketMod permission groups.
* **Audio/Visual Feedback:** Includes custom sounds and effects when joining jobs or getting paid.
* **Easy Configuration:** Simple XML-based setup for jobs and intervals.

## 🚀 Installation

1.  Download the latest `SimpleJobs.dll`.
2.  Drop the `.dll` into your server's `/Rocket/Plugins/` folder.
3.  Restart your server to generate the configuration file.
4.  Edit `SimpleJobs.configuration.xml` to define your jobs.

## ⚙️ Configuration

```xml
<JobConfig>
  <PaycheckIntervalSeconds>600</PaycheckIntervalSeconds>
  <Jobs>
    <Job Id="Police" DisplayName="Officer" Salary="50" />
    <Job Id="Medic" DisplayName="Doctor" Salary="40" />
    <Job Id="Citizen" DisplayName="Worker" Salary="10" />
  </Jobs>
</JobConfig>
```

* PaycheckIntervalSeconds: How often (in seconds) players receive their salary.
* Job Id: This must match the group name in your `Permissions.config.xml`.
* Salary: The amount of EXP awarded per interval.

## 🎮 Commands
`/job join <jobid>` - Joins the specified job group.

Aliases: `/work`

## 📜 License
---
This project is licensed under the MIT License.
