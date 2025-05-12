using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public GameObject player3D;
    public BattleManager bm;
    public StoryManager sm;
    public StateController cs;

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
        List<string> moveIDs = new List<string>();
        foreach (var move in bm.player.moves)
        {
            string moveName = move.moveName;
            moveIDs.Add(moveName);
        }
        b.moveIDs = moveIDs;
        saveData.playerBattleStats = b;

        List<ArticyVariable> av = sm.GatherGlobalVariables();
        saveData.articyVariables = av;

        string blockID = sm.GetCurrentBlockId();
        saveData.currentBlockId = blockID;

        string jsonString = JsonUtility.ToJson(saveData, true);
        string path = Path.Combine(Application.persistentDataPath, $"savefile{slot}.json");
        File.WriteAllText(path, jsonString);
    }

    public void LoadGame() { }
}
