import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os

file_paths = {
    "-1": "Analytics/Analytics/Beta_Overview/level_-1.csv",
    "0": "Analytics/Analytics/Beta_Overview/level_0.csv",
    "1": "Analytics/Analytics/Beta_Overview/level_1.csv",
    "2": "Analytics/Analytics/Beta_Overview/level_2.csv"
}

level_name_map = {
    "-1": "Basic Tutorial",
    "0": "Ally Tutorial",
    "1": "First Level",
    "2": "Second Level"
}

def label_time_bins(t):
    if pd.isna(t):
        return 'Unknown'
    if t < 20:
        return '0–20s'
    elif t < 40:
        return '20–40s'
    elif t < 60:
        return '40–60s'
    elif t < 80:
        return '60–80s'
    elif t < 100:
        return '80–100s'
    else:
        return '100s+'

data_frames = []
for level_num, path in file_paths.items():
    if os.path.exists(path):
        df = pd.read_csv(path)
        df["level"] = level_num
        df["level_name"] = level_name_map[level_num]
        data_frames.append(df)

df_all = pd.concat(data_frames, ignore_index=True)
df_all["completionTime"] = pd.to_numeric(df_all["completionTime"], errors='coerce')
df_all["completed"] = df_all["completed"].astype(bool)

avg_deaths = df_all.groupby("level_name")["deaths"].mean()
completion_rate = df_all[df_all["completed"]].groupby("level_name")["player_id"].nunique() / df_all.groupby("level_name")["player_id"].nunique()

fig, ax1 = plt.subplots(figsize=(10, 6))
ax1.bar(avg_deaths.index, avg_deaths.values, color='lightblue')
ax2 = ax1.twinx()
ax2.plot(completion_rate.index, completion_rate.values * 100, color='orange', marker='o')
ax1.set_ylabel('Avg Deaths')
ax2.set_ylabel('Completion Rate (%)')
plt.title("Average Deaths & Completion Rate by Level")
ax1.tick_params(axis='x', rotation=45)
plt.tight_layout()
plt.show()

first_attempts = df_all[df_all["attempt"] == "attempt_1"]
first_attempt_completion = first_attempts[first_attempts["completed"]].groupby("level_name")["player_id"].nunique()
total_players = df_all.groupby("level_name")["player_id"].nunique()
first_attempt_failures = total_players - first_attempt_completion

completion_df = pd.DataFrame({
    "Completed on First Attempt": first_attempt_completion,
    "Failed First Attempt": first_attempt_failures
}).fillna(0)

completion_df.plot(kind="bar", stacked=True, figsize=(10, 6))
plt.ylabel("Number of Players")
plt.title("First Attempt Completion per Level")
plt.xticks(rotation=45)
plt.tight_layout()
plt.show()

fig, axes = plt.subplots(2, 2, figsize=(12, 8))
axes = axes.flatten()

for idx, level_name in enumerate(df_all['level_name'].unique()):
    level_data = df_all[(df_all['completed']) & (df_all['level_name'] == level_name)]
    axes[idx].hist(level_data["retries"], bins=range(0, level_data["retries"].max() + 2), color='skyblue', edgecolor='black')
    axes[idx].set_title(f"Retry Count - {level_name}")
    axes[idx].set_xlabel("Retries")
    axes[idx].set_ylabel("Player Count")

plt.tight_layout()
plt.suptitle("Retry Count Distribution per Level", fontsize=16, y=1.03)
plt.show()

completed_only = df_all[df_all["completed"] & df_all["completionTime"].notna()]
plt.figure(figsize=(10, 6))
sns.boxplot(x="level_name", y="completionTime", data=completed_only)
plt.title("Completion Time Distribution per Level")
plt.ylabel("Completion Time (seconds)")
plt.xticks(rotation=45)
plt.tight_layout()
plt.show()

df_all["time_range"] = df_all["completionTime"].apply(label_time_bins)
time_range_counts = df_all[df_all["completed"] == True].groupby(['level_name', 'time_range']).size().reset_index(name='count')

plt.figure(figsize=(12, 6))
sns.barplot(data=time_range_counts, x='time_range', y='count', hue='level_name')
plt.title("Completion Time Distribution in Ranges")
plt.xlabel("Time Range")
plt.ylabel("Number of Players")
plt.legend(title="Level")
plt.tight_layout()
plt.show()