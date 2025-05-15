using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseMoves : MonoBehaviour
{
    public List<BattleMove> allMoves;
    private Dictionary<string, BattleMove> moveByName;

    private void Awake()
    {
        moveByName = new Dictionary<string, BattleMove>();
        foreach (var move in allMoves)
        {
            moveByName[move.moveName] = move;
        }
    }

    public BattleMove GetByName(string name)
    {
        moveByName.TryGetValue(name, out var move);
        return move;
    }
}
