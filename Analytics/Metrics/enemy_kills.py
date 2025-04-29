import os
import json
import matplotlib.pyplot as plt
import pandas as pd
import matplotlib.image as mpimg
import matplotlib.lines as mlines

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
plot_colors = [
    "#7f7f7f",
    "#2ca02c",  
    "#bcbd22",  
    "#17becf",  
    "#ce9340",  
    "#f123aa",  
    "#1f77b4",  
    "#ff7f0e",  
    "#d62728",  
    "#926e38"  
]

plot_markers = [
    '<',
    'v',  
    'D',
    'o',  
    '*',    
    'X',  
    '^',    
    'P',  
    '2',  
    's',  
]
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
    
    for level_id, (_, extent) in level_screenshots.items():
        ymin = extent[2]
        mask = (df["level"] == level_id) & (df["reason"] == "Box")
        df.loc[mask, "posY"] += 0.5
    
    for level_id, (_, extent) in level_screenshots.items():
        ymin = extent[2]
        mask = (df["level"] == level_id) & (df["reason"] == "Acclerated Box")
        df.loc[mask, "posY"] += 0.8
        
    for level_id, (_, extent) in level_screenshots.items():
        ymin = extent[2]
        mask = (df["level"] == level_id) & (df["reason"] == "Player #3")
        df.loc[mask, "posY"] += 0.3
        
    return df

def process_data(df: pd.DataFrame) -> pd.DataFrame:
    df = df[~df['reason'].str.contains(r'#1|#2', na=False)]
    df.loc[:, 'reason'] = df['reason'].str.replace('#3', '3 times', regex=False)
    return df

def plot_reason_counts(df, color_map):
    reason_counts = df.groupby(['level_name', 'reason']).size().unstack(fill_value=0)
    total_reason_counts = df['reason'].value_counts().to_dict()
    reasons = reason_counts.columns
    bar_colors = [color_map.get(reason, 'gray') for reason in reasons]
    fig, ax = plt.subplots(figsize=(18, 10))
    reason_counts.plot(kind='bar', ax=ax, color=bar_colors)
    ax.set_xlabel('Level', fontsize=16)
    ax.set_ylabel('Number of Enemy Kills', fontsize=16)
    ax.set_title('Enemy Kill Reasons per Level', fontsize=20)
    ax.tick_params(axis='x', labelsize=14)
    ax.tick_params(axis='y', labelsize=14)
    plt.xticks(rotation=0, ha='right', )

    for container in ax.containers:
        labels_list = ax.bar_label(
            container,
            labels=[int(v) if v > 0 else '' for v in container.datavalues],
            label_type='edge',
            fontsize=12,
            padding=3
        )
        
        for label in labels_list:
            label.set_horizontalalignment('center')
        
    legend_labels = [f"{reason} ({total_reason_counts.get(reason, 0)})" for reason in reasons]
    ax.legend(
        labels=legend_labels,
        loc='upper center',
        bbox_to_anchor=(0.5, -0.1),
        ncol=min(len(reasons), 5),
        title='Reason (Count)',
        fontsize=12,       
        title_fontsize=14   
    )
    plt.tight_layout()
    plt.show()
    
def create_reason_mappings(df: pd.DataFrame):
    unique_reasons = df['reason'].unique()
    color_map = {}
    marker_map = {}
    for idx, reason in enumerate(unique_reasons):
        color = plot_colors[idx % len(plot_colors)]  
        marker = plot_markers[idx % len(plot_markers)]
        color_map[reason] = color
        marker_map[reason] = marker
    return color_map, marker_map

def plot_kill_positions(df: pd.DataFrame, level: int, image_path: str, extent: list, color_map, marker_map):
    img = mpimg.imread(image_path)
    fig, ax = plt.subplots(figsize=(24, 10))

    level_name = level_name_map[level]
    df_level = df[df["level"] == level]
    reasons = df_level['reason'].unique()
    ax.imshow(img, extent=extent, aspect='auto', alpha=0.7)  # optional slight background fade

    reason_counts = df_level['reason'].value_counts().to_dict()

    for reason in reasons:
        sub_df = df_level[df_level['reason'] == reason]
        ax.scatter(
            sub_df['posX'], 
            sub_df['posY'], 
            color=color_map.get(reason, 'black'), 
            marker=marker_map.get(reason, 'o'),
            label=f"{reason} ({reason_counts.get(reason, 0)})",
            s=180,                
            edgecolors='black',   
            linewidths=1,
            alpha=0.7
        )

    ax.set_xlim(extent[0], extent[1])
    ax.set_ylim(extent[2], extent[3])

    ax.set_title(f'Enemy Kill Positions – {level_name}', fontsize=22)
    ax.set_xlabel('posX', fontsize=18)
    ax.set_ylabel('posY', fontsize=18)
    ax.tick_params(axis='x', labelsize=16)
    ax.tick_params(axis='y', labelsize=16)

    legend_elements = [
        mlines.Line2D([], [], 
                    color=color_map.get(r, 'black'), 
                    marker=marker_map.get(r, 'o'), 
                    linestyle='None',
                    markersize=18,    
                    label=f"{r} ({reason_counts.get(r, 0)})", 
                    markeredgecolor='black', 
                    alpha=0.8) 
        for r in reasons
    ]

    ax.legend(
        handles=legend_elements,
        loc='upper center',
        bbox_to_anchor=(0.5, -0.1),
        ncol=min(len(reasons), 5),
        title='Reason (Count)',
        title_fontsize=18,   
        fontsize=16,           
        framealpha=0.8,
        borderpad=1.05,
        labelspacing=0.8,
        handletextpad=0.5        
    )

    plt.grid(True, linestyle='--', linewidth=0.5)
    plt.tight_layout()
    plt.show()

if __name__ == "__main__":
    data_directory = "Analytics/Beta_Data/Beta_Details"
    df = parse_data(data_directory)
    df = process_data(df)

    color_map, marker_map = create_reason_mappings(df)

    plot_reason_counts(df, color_map=color_map)
    plot_kill_positions(df, -1, level_screenshots[-1][0], extent=level_screenshots[-1][1], color_map=color_map, marker_map=marker_map)
    plot_kill_positions(df, 0, level_screenshots[0][0], extent=level_screenshots[0][1], color_map=color_map, marker_map=marker_map)
    plot_kill_positions(df, 1, level_screenshots[1][0], extent=level_screenshots[1][1], color_map=color_map, marker_map=marker_map)
    plot_kill_positions(df, 2, level_screenshots[2][0], extent=level_screenshots[2][1], color_map=color_map, marker_map=marker_map)