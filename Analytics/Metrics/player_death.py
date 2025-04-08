import os
import json
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import matplotlib.colors as mcolors


data_dir = "Analytics/Beta_Data/Beta_Details"
json_files = {
    -1: "level_-1.json",
    0: "level_0.json",
    1: "level_1.json",
    2: "level_2.json"
}

level_name_map = {
    -1: "Basic Tutorial",
    0: "Ally Tutorial",
    1: "First Level",
    2: "Second Level"
}

level_order = ["Basic Tutorial", "Ally Tutorial", "First Level", "Second Level"]

rows = []
for level, file in json_files.items():
    path = os.path.join(data_dir, file)
    with open(path, "r") as f:
        data = json.load(f)
    for player_id, attempts in data.items():
        for attempt_key, metrics in attempts.items():
            death_entries = metrics.get("deathReasons", {})
            for _, death in death_entries.items():
                rows.append({
                    "player_id": player_id,
                    "attempt": attempt_key,
                    "level": level,
                    "level_name": level_name_map[level],
                    "posX": death["posX"],
                    "posY": death["posY"],
                    "timestamp": pd.to_numeric(death["timestamp"], errors="coerce"),
                    "reason": death["reason"]
                })

df_death = pd.DataFrame(rows)
df_death["level_name"] = pd.Categorical(df_death["level_name"], categories=level_order, ordered=True)


all_reasons = df_death["reason"].unique()
color_palette = sns.color_palette("tab10", len(all_reasons))
reason_color_map = dict(zip(all_reasons, color_palette))

fig, axes = plt.subplots(2, 2, figsize=(12, 10))
axes = axes.flatten()

for i, level in enumerate(level_order):
    level_data = df_death[df_death["level_name"] == level]
    reason_counts = level_data["reason"].value_counts()
    
    reasons = reason_counts.index
    colors = [reason_color_map[reason] for reason in reasons]
    
    ax = axes[i]
    ax.pie(reason_counts, labels=reasons, autopct='%1.1f%%', startangle=90, colors=colors)
    ax.set_title(f"Death Reasons – {level}")

plt.suptitle("Death Reason Distribution per Level", fontsize=16)
plt.tight_layout()
plt.subplots_adjust(top=0.9)
plt.show()


fig, axes = plt.subplots(2, 2, figsize=(12, 8))
axes = axes.flatten()

for idx, level in enumerate(level_order):
    level_data = df_death[df_death["level_name"] == level]
    players_per_reason = level_data.groupby("reason")["player_id"].nunique().sort_values(ascending=False)
    axes[idx].bar(players_per_reason.index, players_per_reason.values, color='steelblue', edgecolor='black')
    axes[idx].set_title(f"Players Affected per Death Reason – {level}")
    axes[idx].set_xlabel("Reason")
    axes[idx].set_ylabel("Unique Players")
    axes[idx].tick_params(axis='x', rotation=30)

plt.tight_layout()
plt.show()


player_death_counts = df_death.groupby(["level_name", "player_id"]).size().reset_index(name="death_count")

fig, axes = plt.subplots(2, 2, figsize=(12, 8))
axes = axes.flatten()

for idx, level in enumerate(level_order):
    level_data = player_death_counts[player_death_counts["level_name"] == level]
    if not level_data.empty:
        bins = range(1, level_data["death_count"].max() + 2)
        axes[idx].hist(level_data["death_count"], bins=bins, color='purple', edgecolor='black', align='left', rwidth=0.8)
        axes[idx].set_xticks(range(1, level_data["death_count"].max() + 1))
    axes[idx].set_title(f"Player Death Count - {level}")
    axes[idx].set_xlabel("Deaths per Player")
    axes[idx].set_ylabel("Player Count")

plt.tight_layout()
plt.show()