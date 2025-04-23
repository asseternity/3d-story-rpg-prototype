using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BattleParticipant",
    menuName = "ScriptableObjects/BattleParticipant",
    order = 1
)]
public class BattleParticipant : ScriptableObject
{
    public string participantName;
    public bool isPlayer;
    public bool isFriendly;
    public int HP;
    public int MP;
    public int AC;
    public int ATK;
    public int DMG;
    public GameObject prefabModel;
}
