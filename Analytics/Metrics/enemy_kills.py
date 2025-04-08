import os
import json
import matplotlib.pyplot as plt
import pandas as pd
import matplotlib.image as mpimg


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
                        "attempt": attempt_num, 
                        "posX": kills["posX"],
                        "posY": kills["posY"],
                        "reason": kills["reason"],
                        "timestamp": kills["timestamp"],
                    }
                    rows.append(row)
    df = pd.DataFrame(rows)
    df.reset_index(drop=True, inplace=True)
    return df


def plot_reason_counts(df):
    # Group and count number of kills per level per reason
    reason_counts = df.groupby(['level', 'reason']).size().unstack(fill_value=0)

    # Plotting
    ax = reason_counts.plot(kind='bar', figsize=(12, 6))

    plt.xlabel('Level')
    plt.ylabel('Number of Enemy Kills')
    plt.title('Enemy Kill Reasons per Level')
    plt.xticks(rotation=0)
    plt.legend(title='Reason')
    plt.tight_layout()
    plt.show()


def plot_lvl1_kill_positions(df: pd.DataFrame):
    img = mpimg.imread('./lvl1_screenshot.png')
    fig, ax = plt.subplots(figsize=(20, 5))

    # [-11, 91, -6, 7] is the minX, maxX, minY, maxY of lvl1 screenshot
    ax.imshow(img, extent=[-11, 91, -6, 7], aspect='auto')
    reasons = df['reason'].unique()
    colors = plt.get_cmap('tab10', len(reasons))
    df = df[df['level'] == 1]
    for i, reason in enumerate(reasons):
        sub_df = df[df['reason'] == reason]
        ax.scatter(sub_df['posX'], sub_df['posY'], color=colors(i), label=reason, s=10)
    
    # [-11, 91, -6, 7]
    ax.set_xlim([-11, 91])
    ax.set_ylim([-6, 7])
    ax.set_title('Enemy Kill Positions in Level 1')
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


def plot_lvl2_kill_positions(df: pd.DataFrame):
    img = mpimg.imread('./lvl2_screenshot.png')
    fig, ax = plt.subplots(figsize=(16, 7))

    # [-18, 56, -6, 18] is the minX, maxX, minY, maxY of lvl2 screenshot
    ax.imshow(img, extent=[-18, 56, -6, 18], aspect='auto')
    reasons = df['reason'].unique()
    colors = plt.get_cmap('tab10', len(reasons))
    df = df[df['level'] == 2]
    for i, reason in enumerate(reasons):
        sub_df = df[df['reason'] == reason]
        ax.scatter(sub_df['posX'], sub_df['posY'], color=colors(i), label=reason, s=10)
    
    # [-18, 56, -6, 18]
    ax.set_xlim([-18, 56])
    ax.set_ylim([-6, 18])
    ax.set_title('Enemy Kill Positions in Level 2')
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
    data_directory = "../Analytics/Beta_Details"
    df = parse_data(data_directory)

    # print(df.head())
    plot_reason_counts(df)
    plot_lvl1_kill_positions(df)
    plot_lvl2_kill_positions(df)