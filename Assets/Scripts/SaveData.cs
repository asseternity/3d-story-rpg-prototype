using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string currentBlockId;
    public Vector3 playerPosition;
    public CalendarData calendar;
    public PlayerBattleStatsSave playerBattleStats;
    public List<ProgressEntrySave> progressedActivities;
    public List<ArticyVariable> articyVariables;
}

[System.Serializable]
public class PlayerBattleStatsSave
{
    public string participantName;
    public bool isPlayer;
    public bool isFriendly;
    public int maxHP;
    public int HP;
    public int maxMP;
    public int MP;
    public int DMG;
    public List<string> moveNames;
}

[System.Serializable]
public class CalendarData
{
    public int day;
    public int month;
    public int year;
    public int totalDaysPassed;
}

[System.Serializable]
public class ProgressEntrySave
{
    public string activityID;
    public int currentStage;
}

public enum ArticyVariableType
{
    Int,
    Float,
    Bool,
    String
}

[System.Serializable]
public class ArticyVariable
{
    public string name;
    public ArticyVariableType type;

    // one of these will actually be meaningful, depending on `type`
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public string stringValue;
}
