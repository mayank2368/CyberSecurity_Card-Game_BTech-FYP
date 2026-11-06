using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData 
{
    public int Level1HighScore, Level2HighScore, Level2UnlockPoints;
    public bool Level2Unlocked;
    public int userImg;
    public string UserID; 
}

[System.Serializable]
public class SaveDataList
{
    public List<SaveData> allSaves;
}