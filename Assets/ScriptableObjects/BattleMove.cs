using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleMove", menuName = "ScriptableObjects/BattleMove", order = 3)]
public class BattleMove : ScriptableObject
{
    public string moveName;
    public int numberOfTargets;
    public int MPcost;
    public int ATKmodifier;
    public int DMG;
}
