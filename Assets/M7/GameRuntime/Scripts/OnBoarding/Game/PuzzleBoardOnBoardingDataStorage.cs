using UnityEngine;

public class PuzzleBoardOnBoardingDataStorage : MonoBehaviour
{
    public static void Save(string json)
    {
        PlayerPrefs.SetString("UpdatePuzzleBoardOnBoarding", json);
        PlayerPrefs.Save();
    }

    public static string Load()
    {
        return PlayerPrefs.GetString("UpdatePuzzleBoardOnBoarding");
    }
}
