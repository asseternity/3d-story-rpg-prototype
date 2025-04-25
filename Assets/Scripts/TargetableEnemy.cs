using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableEnemy : MonoBehaviour
{
    private BattleManager battleManager;
    public BattleParticipant me;

    public void Initialize(BattleManager bm, BattleParticipant p)
    {
        battleManager = bm;
        me = p;
    }

    public void OnMouseDown()
    {
        battleManager.ToggleTarget(me);
    }
}
