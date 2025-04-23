using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleData", menuName = "ScriptableObjects/BattleData", order = 2)]
public class BattleData : ScriptableObject
{
    public string battleID;
    public List<BattleParticipant> participants;
    public GameObject battleEnvironmentPrefab;
}
