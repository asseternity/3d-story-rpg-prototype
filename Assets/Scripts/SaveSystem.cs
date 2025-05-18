using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public GameObject player3D;
    public BattleManager bm;
    public StoryManager sm;
    public StateController cs;
    public DatabaseActivity db_activity;
    public DatabaseMoves db_moves;

    public void SaveGame(int slot)
    {
        SaveData saveData = new SaveData();
        saveData.playerPosition = player3D.transform.position;

        CalendarData c = new CalendarData();
        c.day = cs.day;
        c.month = cs.month;
        c.year = cs.year;
        c.totalDaysPassed = cs.totalDaysPassed;
        saveData.calendar = c;

        List<ProgressEntrySave> s = new List<ProgressEntrySave>();
        foreach (var pe in cs.progressedActivities)
        {
            string activityName = pe.activity.name;
            int activityStage = pe.currentStage;
            ProgressEntrySave entry = new ProgressEntrySave();
            entry.activityID = activityName;
            entry.currentStage = activityStage;
            s.Add(entry);
        }
        saveData.progressedActivities = s;

        PlayerBattleStatsSave b = new PlayerBattleStatsSave();
        b.participantName = bm.player.participantName;
        b.isPlayer = true;
        b.isFriendly = false;
        b.maxHP = bm.player.maxHP;
        b.HP = bm.player.HP;
        b.maxMP = bm.player.maxMP;
        b.MP = bm.player.MP;
        b.DMG = bm.player.DMG;
        List<string> moveNames = new List<string>();
        foreach (var move in bm.player.moves)
        {
            string moveName = move.moveName;
            moveNames.Add(moveName);
        }
        b.moveNames = moveNames;
        saveData.playerBattleStats = b;

        List<ArticyVariable> av = sm.GatherGlobalVariables();
        saveData.articyVariables = av;

        string blockID = sm.GetCurrentBlockId();
        if (blockID != null)
        {
            saveData.currentBlockId = blockID;
        }
        else
        {
            saveData.currentBlockId = "";
        }

        string jsonString = JsonUtility.ToJson(saveData, true);
        string path = Path.Combine(Application.persistentDataPath, $"savefile{slot}.json");
        File.WriteAllText(path, jsonString);
    }

    public void LoadGame(int slot)
    {
        string path = Path.Combine(Application.persistentDataPath, $"savefile{slot}.json");

        if (!File.Exists(path))
        {
            Debug.LogError($"No save file found at {path}");
            return;
        }

        string jsonString = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(jsonString);

        // 1) Player position
        player3D.transform.position = data.playerPosition;

        // 2) Calendar
        cs.day = data.calendar.day;
        cs.month = data.calendar.month;
        cs.year = data.calendar.year;
        cs.totalDaysPassed = data.calendar.totalDaysPassed;

        // 3) Progressed Activities
        cs.progressedActivities.Clear();
        foreach (var entry in data.progressedActivities)
        {
            // A way to map activityID -> real Activity object
            var foundActivity = db_activity.GetByID(entry.activityID);
            cs.progressedActivities.Add(
                new StateController.ProgressEntry
                {
                    activity = foundActivity,
                    currentStage = entry.currentStage,
                }
            );
        }

        // 4) Battle Stats
        var pb = data.playerBattleStats;
        bm.player.participantName = pb.participantName;
        bm.player.maxHP = pb.maxHP;
        bm.player.HP = pb.HP;
        bm.player.maxMP = pb.maxMP;
        bm.player.MP = pb.MP;
        bm.player.DMG = pb.DMG;

        bm.player.moves.Clear();
        foreach (var moveName in pb.moveNames)
        {
            // A way to map moveID -> real Move object
            var foundMove = db_moves.GetByName(moveName);
            if (foundMove != null)
            {
                bm.player.moves.Add(foundMove);
            }
        }

        // 5) Articy variables and current block
        sm.SetGlobalVariables(data.articyVariables);
        sm.SetActiveBlock(data.currentBlockId);

        // 6) Unpause player
        PlayerController pc = player3D.GetComponent<PlayerController>();
        pc.paused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
