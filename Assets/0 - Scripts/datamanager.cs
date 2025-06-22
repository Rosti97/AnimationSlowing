using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class datamanager : MonoBehaviour
{
    private List<string> gameData = new List<string>();
    // private List<string> gameDataSecondHalf = new List<string>();

    // private List<int> roundSuccessHits = new List<int>();
    private int roundSuccessHitCounter = 0;
    private List<float> roundRTs = new List<float>();

    private string filePath = "/data/";

    private string id;
    private StringBuilder _dataCSVBuilder = new StringBuilder();
    private string _filePath = Application.dataPath + "/data/"; 
    //private string dataBase64;

    // [DllImport("__Internal")]
    // private static extern void receiveGameData(string data);
    // [DllImport("__Internal")]
    // private static extern void receiveMidGameData(string data);
    // // [DllImport("__Internal")]
    // // private static extern void receiveBackupData(string id, string data);
    // [DllImport("__Internal")]
    // private static extern void gameEnd();

    private void Start()
    {
        // generate random 4-digit id
        // 1st digit: 1-9, rest: 0-9
        id = UnityEngine.Random.Range(1, 10).ToString() +
                UnityEngine.Random.Range(0, 10).ToString() +
                UnityEngine.Random.Range(0, 10).ToString() +
                UnityEngine.Random.Range(0, 10).ToString();

        // Create the CSV header
        string header = "id, version, round, trial, timestamp, game_timestamp, mouse_x, mouse_y, color, position, animation_duration, rt_start_timestamp, rt_end_timestamp, rt, hit";
        _dataCSVBuilder.AppendLine(header);

        _filePath = $"{_filePath}{id}_gameData.csv";

        // File.WriteAllText(_filePath, _dataCSVBuilder.ToString());
    }

    public void AddTrialToData(int round, int trial, float mouse_x, float mouse_y, string shape, string position, float animation_duration, float start_RT, float end_RT, float RT, int status)
    {
        string row = $"{id}, BL, {round}, {trial}, {DateTime.Now:HH:mm:ss.fff}, {Time.time}, {mouse_x:F3}, {mouse_y:F3}, {shape}, {position}, {animation_duration}, {start_RT}, {end_RT}, {RT}, {status}";

        _dataCSVBuilder.AppendLine(row);

        // stat stuff for UI
        if (status == 1) // if successful hit
        {
            // roundSuccessHits.Add(1);
            roundSuccessHitCounter++;
        }
        roundRTs.Add(RT);

    }

    // called with game end event
    public void SaveGameData()
    {
        try
        {
            File.AppendAllText(_filePath, _dataCSVBuilder.ToString());

            _dataCSVBuilder.Clear();
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving game data: " + e.Message);
        }
    }

    public string GetID()
    {
        return id;
    }

    public int GetRoundSuccessHits()
    {
        return roundSuccessHitCounter;
    }

    public List<float> GetRoundRTs()
    {
        return roundRTs;
    }

    public void ResetRoundStats()
    {
        roundSuccessHitCounter = 0;
        roundRTs.Clear();
    }

}
