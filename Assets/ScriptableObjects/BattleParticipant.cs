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
    public int maxHP;
    public int HP;
    public int maxMP;
    public int MP;
    public int DMG;
    public GameObject prefabModel;
    public List<BattleMove> moves;
}
