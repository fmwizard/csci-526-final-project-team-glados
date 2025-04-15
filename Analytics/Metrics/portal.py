import os
import json
import pandas as pd
import seaborn as sns
import matplotlib.pyplot as plt
import matplotlib.image as mpimg

# Level name map
level_name_map = {
    -1: "Basic Tutorial",
    0: "Ally Tutorial",
    1: "First Level",
    2: "Second Level"
}

level_order = ["Basic Tutorial", "Ally Tutorial", "First Level", "Second Level"]

def parse_level_data(directory: str):
    rows = []
    for filename in os.listdir(directory):
        if not filename.startswith("level_") or not filename.endswith(".json"):
            continue

        level_num = int(filename.split("_")[1].split(".")[0])
        file_path = os.path.join(directory, filename)
        with open(file_path, 'r') as f:
            data = json.load(f)
        for session_key, session_values in data.items():
            for attempt_key, attempt_values in session_values.items():
                attempt_num = int(attempt_key.split("_")[1])
                portal_usage = attempt_values.get("portal_usage", {})
                for _, usage in portal_usage.items():
                    row = {
                        "session": session_key,
                        "level": level_num,
                        "level_name": level_name_map[level_num],
                        "attempt": attempt_num,
                        "fromX": usage["fromX"],
                        "fromY": usage["fromY"],
                        "objectType": usage["objectType"],
                        "timestamp": usage["timestamp"],
                        "toX": usage["toX"],
                        "toY": usage["toY"],
                        "velocity": int(usage["velocity"])
                    }
                    rows.append(row)
    df = pd.DataFrame(rows)
    df['level_name'] = pd.Categorical(df['level_name'], categories=level_order, ordered=True)
    df.reset_index(drop=True, inplace=True)
    return df

def process_data(df: pd.DataFrame):
    usage_index = []
    for i, row in df.iterrows():
        if i > 0 and (row["fromX"], row["fromY"], row["toX"], row["toY"]) == (df.iloc[i-1]["fromX"], df.iloc[i-1]["fromY"], df.iloc[i-1]["toX"], df.iloc[i-1]["toY"]):
            usage_index.append(usage_index[-1])
        else:
            usage_index.append(i)
    df['usage_index'] = usage_index

    for _, group in df.groupby('usage_index'):
        if (group['velocity'] >= 10).any():
            df.loc[group.index, 'acceleration'] = 'Accelerated'
        else:
            df.loc[group.index, 'acceleration'] = 'Normal'

    return df

def plot_portal_usage(df: pd.DataFrame):
    usage_counts = df.groupby(['level_name', 'objectType'])['usage_index'].nunique().reset_index()
    pivot = usage_counts.pivot(index='level_name', columns='objectType', values='usage_index').fillna(0)

    pivot.plot(kind='bar', figsize=(12, 6), colormap='Set2')
    plt.xlabel('Level')
    plt.ylabel('Unique Portal Usages')
    plt.title('Portal Usage by Object Type per Level')
    plt.xticks(rotation=0)
    plt.legend(title='Object Type')
    plt.tight_layout()
    plt.show()

def plot_teleportation_types_by_usage(df: pd.DataFrame):
    teleportation_counts = df.groupby(['level_name', 'objectType'])['usage_index'].nunique().unstack(fill_value=0)
    teleportation_percent = teleportation_counts.div(teleportation_counts.sum(axis=1), axis=0) * 100

    bright_colors = sns.color_palette("bright", len(teleportation_percent.columns))
    ax = teleportation_percent.plot(kind='bar', stacked=True, figsize=(12, 6), color=bright_colors)

    plt.xlabel('Level')
    plt.ylabel('Percentage of Portal Usage')
    plt.title('Proportional Portal Usage by Object Type per Level')
    plt.xticks(rotation=0)
    plt.legend(title="Object Type", loc='upper right')

    for i, level in enumerate(teleportation_percent.index):
        bottom = 0
        for object_type in teleportation_percent.columns:
            height = teleportation_percent.loc[level, object_type]
            if height > 2:
                ax.text(i, bottom + height / 2, f"{height:.1f}%", ha='center', va='center', fontsize=9)
            bottom += height

    plt.tight_layout()
    plt.show()

def plot_teleportation_types_by_level_with_acceleration(df: pd.DataFrame):
    teleportation_counts = df.groupby(['level_name', 'acceleration'])['usage_index'].nunique().unstack(fill_value=0)

    bright_colors = sns.color_palette("bright", len(teleportation_counts.columns))
    ax = teleportation_counts.plot(kind='bar', stacked=True, figsize=(12, 6), color=bright_colors)

    plt.xlabel('Level')
    plt.ylabel('Teleportation Count')
    plt.title('Teleportation Count (Normal vs Accelerated) per Level')
    plt.xticks(rotation=0)
    plt.legend(title="Teleportation Type", loc='upper right')

    for i, level in enumerate(teleportation_counts.index):
        bottom = 0
        for accel_type in teleportation_counts.columns:
            height = teleportation_counts.loc[level, accel_type]
            if height > 0:
                ax.text(i, bottom + height / 2, f"{int(height)}", ha='center', va='center', fontsize=10)
            bottom += height

    plt.tight_layout()
    plt.show()

def plot_portal_heatmap(df: pd.DataFrame, level: int, img_path: str, extent: list):
    df_level = df[df["level"] == level]
    img = mpimg.imread(img_path)

    fig, ax = plt.subplots(figsize=(14, 6))
    ax.imshow(img, extent=extent, aspect='auto')

    sns.kdeplot(
        x=df_level['fromX'],
        y=df_level['fromY'],
        ax=ax,
        cmap="Blues",
        fill=True,
        bw_adjust=0.5,
        thresh=0.2,
        levels=15
    )

    cbar = plt.colorbar(ax.collections[0], ax=ax, shrink=0.75, pad=0.02)
    cbar.set_label("Portal Entry Density")

    ax.set_title(f"Portal Entry Heatmap – {level_name_map[level]}")
    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])
    plt.tight_layout()
    plt.show()

if __name__ == "__main__":
    data_directory = "Analytics/Beta_Data/Beta_Details"
    df = parse_level_data(data_directory)
    df = process_data(df)

    plot_portal_usage(df)
    plot_teleportation_types_by_usage(df)
    plot_teleportation_types_by_level_with_acceleration(df)

    plot_portal_heatmap(df, -1, 'Analytics/Metrics/LevelDesignSS/tutorial_screenshot.png', extent=[-11, 97, -6, 7])
    plot_portal_heatmap(df, 0, 'Analytics/Metrics/LevelDesignSS/allyTutorial_screenshot.png', extent=[-11, 87, -6, 5])
    plot_portal_heatmap(df, 1, 'Analytics/Metrics/LevelDesignSS/lvl1_screenshot.png', extent=[-11, 91, -6, 7])
    plot_portal_heatmap(df, 2, 'Analytics/Metrics/LevelDesignSS/lvl2_screenshot.png', extent=[-18, 56, -6, 18])