import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import os
from sklearn.cluster import KMeans
from scipy.stats import ttest_ind

file_paths = {
    "-1": "Analytics/Beta_Data/Beta_Overview/level_-1.csv",
    "0": "Analytics/Beta_Data/Beta_Overview/level_0.csv",
    "1": "Analytics/Beta_Data/Beta_Overview/level_1.csv",
    "2": "Analytics/Beta_Data/Beta_Overview/level_2.csv"
}

level_name_map = {
    "-1": "Basic Tutorial",
    "0": "Ally Tutorial",
    "1": "First Level",
    "2": "Second Level"
}

level_order = ["Basic Tutorial", "Ally Tutorial", "First Level", "Second Level"]

def label_time_bins(t):
    if pd.isna(t): return 'Unknown'
    if t < 40: return '<40s'
    elif t < 60: return '40–60s'
    elif t < 80: return '60–80s'
    elif t < 100: return '80–100s'
    else: return '100s+'

time_range_order = ['<40s', '40–60s', '60–80s', '80–100s', '100s+']

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
df_all["level_name"] = pd.Categorical(df_all["level_name"], categories=level_order, ordered=True)

df_all["deaths"] = pd.to_numeric(df_all["deaths"], errors="coerce")
df_all["retries"] = pd.to_numeric(df_all["retries"], errors="coerce")

avg_deaths = df_all.groupby("level_name")["deaths"].mean().reindex(level_order)
completion_rate = (
    df_all[df_all["completed"]].groupby("level_name")["player_id"].nunique() /
    df_all.groupby("level_name")["player_id"].nunique()
).reindex(level_order)

fig, ax1 = plt.subplots(figsize=(10, 6))
ax1.bar(avg_deaths.index, avg_deaths.values, color='lightblue', label='Avg Deaths')
ax2 = ax1.twinx()
ax2.plot(completion_rate.index, completion_rate.values * 100, color='orange', marker='o', label='Completion Rate (%)')
ax1.set_ylabel('Avg Deaths')
ax2.set_ylabel('Completion Rate (%)')
plt.title("Average Deaths & Completion Rate by Level")
ax1.tick_params(axis='x', rotation=45)
lines, labels = ax1.get_legend_handles_labels()
lines2, labels2 = ax2.get_legend_handles_labels()
ax1.legend(lines + lines2, labels + labels2, loc='upper right')
plt.tight_layout()
plt.show()

first_attempts = df_all[df_all["attempt"] == "attempt_1"]
first_attempt_completion = first_attempts[first_attempts["completed"]].groupby("level_name")["player_id"].nunique()
total_players = df_all.groupby("level_name")["player_id"].nunique()
first_attempt_failures = total_players - first_attempt_completion

completion_df = pd.DataFrame({
    "Completed on First Attempt": first_attempt_completion,
    "Failed First Attempt": first_attempt_failures
}).fillna(0).reindex(level_order)

completion_df_percent = completion_df.div(completion_df.sum(axis=1), axis=0) * 100

ax = completion_df_percent.plot(kind="bar", stacked=True, figsize=(10, 6), colormap='Set2')
plt.ylabel("Percentage of Players")
plt.title("First Attempt Completion Rate per Level (in %)")
plt.xticks(rotation=45)

for i, (index, row) in enumerate(completion_df_percent.iterrows()):
    bottom = 0
    for col in completion_df_percent.columns:
        height = row[col]
        if height > 0:
            ax.text(i, bottom + height / 2, f"{height:.1f}%", ha='center', va='center', fontsize=10)
        bottom += height

plt.tight_layout()
plt.show()

fig, axes = plt.subplots(2, 2, figsize=(12, 8))
axes = axes.flatten()
max_retry = df_all[df_all['completed']]["retries"].max()
bins = list(range(0, int(max_retry) + 2))

for idx, level_name in enumerate(level_order):
    level_data = df_all[(df_all['completed']) & (df_all['level_name'] == level_name)]
    axes[idx].hist(level_data["retries"], bins=bins, color='skyblue', edgecolor='black', align='left', rwidth=0.8)
    axes[idx].set_title(f"Retry Count - {level_name}")
    axes[idx].set_xlabel("Retries")
    axes[idx].set_ylabel("Player Count")
    axes[idx].set_xticks(bins)

plt.tight_layout()
plt.suptitle("Retry Count Distribution per Level", fontsize=16, y=1.03)
plt.show()

completed_only = df_all[df_all["completed"] & df_all["completionTime"].notna()]
plt.figure(figsize=(10, 6))
sns.boxplot(x="level_name", y="completionTime", data=completed_only, order=level_order)
plt.title("Completion Time Distribution per Level")
plt.ylabel("Completion Time (seconds)")
plt.xticks(rotation=45)
plt.tight_layout()
plt.show()

expected_times = {
    "Basic Tutorial": 60,
    "Ally Tutorial": 80,
    "First Level": 180,
    "Second Level": 240
}
avg_times = completed_only.groupby("level_name")["completionTime"].mean().to_dict()

df_all["time_range"] = df_all["completionTime"].apply(label_time_bins)

time_range_pivot = (
    df_all[df_all["completed"] == True]
    .groupby(["level_name", "time_range"]).size()
    .unstack(fill_value=0)
    .reindex(level_order)
    .reindex(columns=time_range_order)
)

ax = time_range_pivot.plot(kind='bar', stacked=True, figsize=(14, 9), colormap='viridis')
plt.title("Completion Time Buckets per Level (Stacked)")
plt.xlabel("Level")
plt.ylabel("Number of Players")
plt.xticks(rotation=45)
plt.legend(title="Time Range")

for i, level in enumerate(level_order):
    avg = avg_times.get(level, 0)
    exp = expected_times[level]
    total = time_range_pivot.loc[level].sum()
    ax.text(i, total + 1.6, f"Avg: {avg:.1f}s", ha='center', fontsize=9, color='black')
    ax.text(i, total + 2.6, f"Exp: {exp}s", ha='center', fontsize=9, color='gray')

plt.tight_layout()
plt.show()

features = df_all.dropna(subset=["deaths", "retries", "completionTime"])[['deaths', 'retries', 'completionTime']]
kmeans = KMeans(n_clusters=3, random_state=42).fit(features)
df_all.loc[features.index, 'player_type'] = kmeans.labels_

plt.figure(figsize=(10, 6))
sns.scatterplot(data=df_all.loc[features.index], x="retries", y="completionTime", hue="player_type", palette="Set2")
plt.title("Player Archetypes - Completion Time vs Retries")
plt.xlabel("Retries")
plt.ylabel("Completion Time (s)")
plt.legend(title="Player Type")
plt.tight_layout()
plt.show()

heatmap_data = df_all[df_all['completed']].pivot_table(index='retries', columns='level_name', values='completionTime', aggfunc='mean')
plt.figure(figsize=(12, 8))
sns.heatmap(heatmap_data, cmap='coolwarm', annot=True, fmt=".1f", linewidths=.5)
plt.title("Average Completion Time by Retries per Level")
plt.xlabel("Level")
plt.ylabel("Retries")
plt.tight_layout()
plt.show()

completed = df_all[df_all["completed"] & df_all["completionTime"].notna()]
completed["expected"] = completed["level_name"].map(expected_times).astype(float)
outliers = completed[completed["completionTime"] > 2 * completed["expected"]]

if not outliers.empty:
    table_data = outliers[["player_id", "level_name", "completionTime", "expected"]].copy()
    table_data["completionTime"] = table_data["completionTime"].round(2)
    table_data["expected"] = table_data["expected"].round(2)
    table_data.columns = ["Player ID", "Level", "Completion Time", "Expected Time"]

    fig, ax = plt.subplots(figsize=(12, len(table_data) * 0.3 + 1))
    ax.axis('off')
    ax.axis('tight')

    table = ax.table(
        cellText=table_data.values,
        colLabels=table_data.columns,
        cellLoc='center',
        loc='center'
    )
    table.auto_set_font_size(False)
    table.set_fontsize(9)
    table.scale(1, 1.5)

    plt.title("Outliers: Players Taking More Than 2x Expected Time", fontsize=14, pad=20)
    plt.tight_layout()
    plt.show()
