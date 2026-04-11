# SimpleJobsPlus (Unturned, SDG Rocket)

Features:
- Jobs with uniforms, salaries (XP), XP multipliers, levels.
- Daily job tasks with XP rewards.
- Job-only chat channel.
- Persistent storage in `PlayerJobs.xml`.

Commands:
- `/setjob <job>` — choose a job.
- `/myjob` — see your job, level, XP.
- `/jobchat <msg>` or `/jc <msg>` — talk to same-job players.
- `/jobtask` — see your daily task.
- `/jobtask complete` — mark daily task as completed.

Permissions:
- `jobs.set`
- `jobs.chat`
- `jobs.task`

Install:
1. Compile into a DLL referencing SDG Rocket assemblies.
2. Place DLL in `Servers/<YourServer>/Rocket/Plugins/`.
3. Start server to generate config.
