import os
import json
import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sns
import matplotlib.image as mpimg

data_dir = "Analytics/Analytics/Beta_Details"
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
                    "timestamp": death["timestamp"],
                    "reason": death["reason"]
                })

df_death = pd.DataFrame(rows)

death_reason_counts = df_death.groupby(["level_name", "reason"]).size().unstack(fill_value=0)
ax = death_reason_counts.plot(kind="bar", stacked=True, figsize=(12, 6))
plt.title("Death Reasons by Level")
plt.xlabel("Level")
plt.ylabel("Number of Deaths")
plt.xticks(rotation=45)
plt.tight_layout()
plt.show()

fig, axes = plt.subplots(2, 2, figsize=(12, 10))
levels = df_death["level_name"].unique()
for i, level in enumerate(levels):
    ax = axes[i // 2, i % 2]
    data = df_death[df_death["level_name"] == level]["reason"].value_counts()
    ax.pie(data, labels=data.index, autopct="%1.1f%%", startangle=90)
    ax.set_title(level)
plt.tight_layout()
plt.show()

def plot_death_heatmap(level, img_path, extent):
    data = df_death[df_death["level"] == level]
    img = mpimg.imread(img_path)
    fig, ax = plt.subplots(figsize=(14, 6))
    ax.imshow(img, extent=extent, aspect='auto')
    sns.kdeplot(data['posX'], data['posY'], ax=ax, cmap="Reds", fill=True, thresh=0.05)
    ax.set_title(f"Death Heatmap - {level_name_map[level]}")
    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])
    plt.tight_layout()
    plt.show()

plot_death_heatmap(-1, './images/level_-1.png', extent=[-5, 50, -5, 10])
plot_death_heatmap(0, './images/level_0.png', extent=[-10, 60, -5, 15])
plot_death_heatmap(1, './images/level_1.png', extent=[-11, 91, -6, 7])
plot_death_heatmap(2, './images/level_2.png', extent=[-18, 56, -6, 18])