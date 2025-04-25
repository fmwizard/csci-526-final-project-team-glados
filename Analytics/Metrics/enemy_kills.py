import os
import json
import matplotlib.pyplot as plt
import pandas as pd
import matplotlib.image as mpimg

level_name_map = {
    -1: "Basic Tutorial",
    0: "Ally Tutorial",
    1: "First Level",
    2: "Second Level"
}
level_order = ["Basic Tutorial", "Ally Tutorial", "First Level", "Second Level"]

level_screenshots = {
    -1: ("Analytics/Metrics/LevelDesignSS/tutorial_screenshot.png", [-11, 97, -7, 8]),
    0: ("Analytics/Metrics/LevelDesignSS/allyTutorial_screenshot.png", [-11, 86, -6, 5]),
    1: ("Analytics/Metrics/LevelDesignSS/lvl1_screenshot.png", [-11, 91, -6.3, 7.3]),
    2: ("Analytics/Metrics/LevelDesignSS/lvl2_screenshot.png", [-18, 56, -6, 18])
}

def parse_data(directory: str):
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
                enemy_kills = attempt_values.get("enemy_kills", {})
                for _, kills in enemy_kills.items():
                    row = {
                        "session": session_key,
                        "level": level_num,
                        "level_name": level_name_map[level_num],
                        "attempt": attempt_num, 
                        "posX": kills["posX"],
                        "posY": kills["posY"],
                        "reason": kills["reason"],
                        "timestamp": kills["timestamp"],
                    }
                    rows.append(row)
    df = pd.DataFrame(rows)
    df['level_name'] = pd.Categorical(df['level_name'], categories=level_order, ordered=True)
    df.reset_index(drop=True, inplace=True)

    for level_id, (_, extent) in level_screenshots.items():
        ymin = extent[2]
        mask = (df["level"] == level_id) & (df["reason"] == "Fall")
        df.loc[mask, "posY"] = ymin + 1

    return df

def process_data(df: pd.DataFrame) -> pd.DataFrame:
    df = df[~df['reason'].str.contains(r'#1|#2', na=False)]
    df.loc[:, 'reason'] = df['reason'].str.replace('#3', '3 times', regex=False)
    return df

def plot_reason_counts(df):
    reason_counts = df.groupby(['level_name', 'reason']).size().unstack(fill_value=0)
    reasons = df['reason'].unique()
    
    color_map = dict(zip(reasons, plt.get_cmap('tab10').colors[:len(reasons)]))
    bar_colors = [color_map.get(reason, 'gray') for reason in reason_counts.columns]
    
    ax = reason_counts.plot(kind='bar', figsize=(18, 8), color=bar_colors)
    plt.xlabel('Level')
    plt.ylabel('Number of Enemy Kills')
    plt.title('Enemy Kill Reasons per Level')
    plt.xticks(rotation=0)
    plt.legend(
        loc='upper center',
        bbox_to_anchor=(0.5, -0.15),
        ncol=len(reason_counts.columns),
        title='Reason'
    )
    plt.tight_layout()
    plt.show()

def plot_kill_positions(df: pd.DataFrame, level: int, image_path: str, extent: list):
    img = mpimg.imread(image_path)
    fig, ax = plt.subplots(figsize=(24, 10))

    level_name = level_name_map[level]
    df = df[df["level"] == level]
    reasons = df['reason'].unique()
    colors = dict(zip(reasons, plt.get_cmap('tab10').colors[:len(reasons)]))

    ax.imshow(img, extent=extent, aspect='auto')
    for reason in reasons:
        sub_df = df[df['reason'] == reason]
        ax.scatter(sub_df['posX'], sub_df['posY'], color=colors[reason], label=reason, s=10)

    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])
    ax.set_title(f'Enemy Kill Positions – {level_name}')
    ax.set_xlabel('posX')
    ax.set_ylabel('posY')
    ax.legend(
        loc='upper center',
        bbox_to_anchor=(0.5, -0.15),
        ncol=len(reasons),
        title='Reason'
    )
    plt.grid(True)
    plt.tight_layout()
    plt.show()

if __name__ == "__main__":
    data_directory = "Analytics/Beta_Data/Beta_Details"
    df = parse_data(data_directory)
    df = process_data(df)

    plot_reason_counts(df)
    plot_kill_positions(df, -1, level_screenshots[-1][0], extent=level_screenshots[-1][1])
    plot_kill_positions(df, 0, level_screenshots[0][0], extent=level_screenshots[0][1])
    plot_kill_positions(df, 1, level_screenshots[1][0], extent=level_screenshots[1][1])
    plot_kill_positions(df, 2, level_screenshots[2][0], extent=level_screenshots[2][1])