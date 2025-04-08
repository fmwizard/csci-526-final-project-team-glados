import os
import json
from collections import defaultdict
import matplotlib.pyplot as plt
import pandas as pd

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
    df.reset_index(drop=True, inplace=True)
    return df


def process_data(df: pd.DataFrame):
    usage_index = []
    for i, row in df.iterrows():
        if i > 0 and (row["fromX"], row["fromY"], row["toX"], row["toY"]) == (df.iloc[i-1]["fromX"], df.iloc[i-1]["fromY"], df.iloc[i-1]["toX"], df.iloc[i-1]["toY"]):
            usage_index.append(usage_index[-1])  # Use the same index for repeated portal usage
        else:
            usage_index.append(i)  # New unique portal usage index
    df['usage_index'] = usage_index

    # Iterate over the grouped usage_index to mark rows as 'Accelerated' if velocity >= 10 in any row of the same usage_index
    for _, group in df.groupby('usage_index'):
        if (group['velocity'] >= 10).any():
            df.loc[group.index, 'acceleration'] = 'Accelerated'
        else:
            df.loc[group.index, 'acceleration'] = 'Normal'

    return df


def plot_portal_usage(df: pd.DataFrame):
    # Count unique usage_index per level
    unique_usage_count = df.groupby('level')['usage_index'].nunique()
    
    # Plotting the bar chart
    plt.figure(figsize=(10, 6))
    unique_usage_count.plot(kind='bar')
    
    # Set the labels and title
    plt.xlabel('Level')
    plt.ylabel('Teleportation Count')
    plt.title('Teleportation Count')
    
    # Display the plot
    plt.xticks(rotation=0)  # Make x-axis labels horizontal
    plt.tight_layout()
    plt.show()


def plot_teleportation_types_by_usage(df):
    # Group by level and objectType, then count unique usage_index per objectType within each level
    teleportation_counts = df.groupby(['level', 'objectType'])['usage_index'].nunique().unstack(fill_value=0)
    # print(teleportation_counts.head())
    
    # Plotting the bar chart
    ax = teleportation_counts.plot(kind='bar', figsize=(12, 6), color=['blue', 'orange', 'green'])
    plt.xlabel('Level')
    plt.ylabel('Teleportation Count')
    plt.title('Teleportation Count of Object Types')
    ax.legend(teleportation_counts.columns, title="Object Type")
    plt.xticks(rotation=0)  # Make x-axis labels horizontal
    plt.tight_layout()
    plt.show()


def plot_teleportation_types_by_level_with_acceleration(df):
    # Count normal and accelerated teleportations per level
    teleportation_counts = df.groupby(['level', 'acceleration'])['usage_index'].nunique().unstack(fill_value=0)
    # print(teleportation_counts.head())
    
    # Plotting the stacked bar chart
    ax = teleportation_counts.plot(kind='bar', stacked=True, figsize=(12, 6), color=['skyblue', 'orange'])
    plt.xlabel('Level')
    plt.ylabel('Teleportation Count')
    plt.title('Teleportation Count (Normal vs Accelerated)')
    ax.legend(teleportation_counts.columns, title="Teleportation Type")
    
    # Display the plot
    plt.xticks(rotation=0)  # Make x-axis labels horizontal
    plt.tight_layout()
    plt.show()


if __name__ == "__main__":
    data_directory = "../Analytics/Beta_Details"
    df = parse_level_data(data_directory)
    df = process_data(df)

    # print(df.head())
    plot_portal_usage(df)
    plot_teleportation_types_by_usage(df)
    plot_teleportation_types_by_level_with_acceleration(df)

