using UnityEngine;

public class SaveService
{
    private const string SaveKey = "PuzzleWorld_Save";

    public SaveData Data { get; private set; }

    public SaveService()
    {
        Load();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            Data = JsonUtility.FromJson<SaveData>(json);
        }

        if (Data == null)
            Data = new SaveData();

        Data.unlockedLevel = Mathf.Max(1, Data.unlockedLevel);
        Data.completedLevels = Mathf.Max(0, Data.completedLevels);
        Data.currentLevel = Mathf.Max(1, Data.currentLevel);
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(Data);

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void SetCurrentLevel(int levelNumber)
    {
        Data.currentLevel = Mathf.Max(1, levelNumber);
        Save();
    }

    public void CompleteLevel(int levelNumber)
    {
        if (levelNumber >= Data.unlockedLevel)
            Data.unlockedLevel = levelNumber + 1;

        Data.completedLevels = Mathf.Max(
            Data.completedLevels,
            levelNumber
        );

        Save();
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber >= 1 &&
               levelNumber <= Data.unlockedLevel;
    }

    public void ResetProgress()
    {
        Data = new SaveData();
        Save();
    }
}
