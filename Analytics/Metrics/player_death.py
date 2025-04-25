import os
import json
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import matplotlib.image as mpimg

sns.set(style="whitegrid")

data_dir = "Analytics/Beta_Data/Beta_Details"
json_files = {-1: "level_-1.json", 0: "level_0.json", 1: "level_1.json", 2: "level_2.json"}
level_name_map = {-1: "Basic Tutorial", 0: "Ally Tutorial", 1: "First Level", 2: "Second Level"}
level_order = ["Basic Tutorial", "Ally Tutorial", "First Level", "Second Level"]
level_screenshots = {
    -1: ("Analytics/Metrics/LevelDesignSS/tutorial_screenshot.png", [-11, 97, -7, 8]),
    0: ("Analytics/Metrics/LevelDesignSS/allyTutorial_screenshot.png", [-11, 87, -6, 5]),
    1: ("Analytics/Metrics/LevelDesignSS/lvl1_screenshot.png", [-11, 91, -6.3, 7.3]),
    2: ("Analytics/Metrics/LevelDesignSS/lvl2_screenshot.png", [-18, 56, -6, 18])
}

rows = []
for level, file in json_files.items():
    path = os.path.join(data_dir, file)
    with open(path, "r") as f:
        data = json.load(f)
    for player_id, attempts in data.items():
        for attempt_key, metrics in attempts.items():
            for death in metrics.get("deathReasons", {}).values():
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

for level_id, (_, extent) in level_screenshots.items():
    ymin = extent[2]
    mask = (df_death["level"] == level_id) & (df_death["reason"] == "Fall")
    df_death.loc[mask, "posY"] = ymin + 1

unique_reasons = df_death["reason"].unique()
reason_colors = dict(zip(unique_reasons, sns.color_palette("Set1", len(unique_reasons))))

fig, axes = plt.subplots(2, 2, figsize=(14, 10))
axes = axes.flatten()
for i, level in enumerate(level_order):
    level_data = df_death[df_death["level_name"] == level]
    reason_counts = level_data["reason"].value_counts()
    reasons = reason_counts.index
    colors = [reason_colors[reason] for reason in reasons]
    axes[i].pie(reason_counts, labels=reasons, autopct='%1.1f%%', startangle=90, colors=colors)
    axes[i].set_title(f"Death Reasons – {level}")
plt.suptitle("Death Reason Distribution per Level", fontsize=16)
plt.tight_layout()
plt.subplots_adjust(top=0.9)
plt.show()

fig, axes = plt.subplots(2, 2, figsize=(12, 8))
axes = axes.flatten()
for idx, level in enumerate(level_order):
    level_data = df_death[df_death["level_name"] == level]
    players_per_reason = level_data.groupby("reason")["player_id"].nunique().sort_values(ascending=False)
    reason_bar_colors = [reason_colors[reason] for reason in players_per_reason.index]
    axes[idx].bar(players_per_reason.index, players_per_reason.values, color=reason_bar_colors, edgecolor='black')
    axes[idx].set_title(f"Players Affected per Death Reason – {level}")
    axes[idx].set_xlabel("Reason")
    axes[idx].set_ylabel("Unique Players")
    axes[idx].tick_params(axis='x', rotation=30)
plt.tight_layout()
plt.show()

player_death_counts = df_death.groupby(["level_name", "player_id"]).size().reset_index(name="death_count")
fig, axes = plt.subplots(2, 2, figsize=(14, 9))
axes = axes.flatten()
for i, level in enumerate(level_order):
    data = player_death_counts[player_death_counts["level_name"] == level]
    avg_death = data["death_count"].mean()
    if not data.empty:
        bins = range(1, data["death_count"].max() + 2)
        axes[i].hist(data["death_count"], bins=bins, color='purple', edgecolor='black', align='left', rwidth=0.9)
        axes[i].set_title(f"Player Death Count - {level}")
        axes[i].set_xlabel("Deaths per Player")
        axes[i].set_ylabel("Player Count")
        axes[i].legend([f"Avg: {avg_death:.1f}"], loc="upper right", frameon=True)
plt.tight_layout()
plt.show()

for level, (screenshot_path, extent) in level_screenshots.items():
    fig, ax = plt.subplots(figsize=(24, 12))
    img = mpimg.imread(screenshot_path)
    ax.imshow(img, extent=extent, aspect='auto', alpha=0.8)

    for reason in unique_reasons:
        data = df_death[(df_death["level"] == level) & (df_death["reason"] == reason)]
        ax.scatter(data["posX"], data["posY"], label=reason, s=25, color=reason_colors[reason], alpha=0.7)

    ax.set_title(f"Death Heatmap – {level_name_map[level]}")
    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])
    ax.set_xlabel("posX")
    ax.set_ylabel("posY")
    ax.legend(title="Death Reason", loc='upper right')
    plt.grid(False)
    plt.tight_layout()
    plt.show()