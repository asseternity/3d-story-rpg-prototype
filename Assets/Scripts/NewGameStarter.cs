using System.Collections;
using System.Collections.Generic;
using Articy.Unity;
using UnityEngine;

public class NewGameStarter : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject player3D;
    public BattleManager bm;
    public StoryManager sm;
    public StateController cs;
    public DatabaseActivity db_activity;
    public DatabaseMoves db_moves;
    public Vector3 newGamePosition;
    public ArticyRef introBlock;

    public void NewGame()
    {
        // 1) Player position
        player3D.transform.position = newGamePosition;

        // 2) Calendar
        cs.day = 1;
        cs.month = 0;
        cs.year = 1000;
        cs.totalDaysPassed = 0;

        // 3) Progressed Activities
        cs.progressedActivities.Clear();

        // 4) Battle Stats
        bm.player.participantName = "Player";
        bm.player.maxHP = 10;
        bm.player.HP = 10;
        bm.player.maxMP = 10;
        bm.player.MP = 10;
        bm.player.DMG = 1;

        bm.player.moves.Clear();

        // 5) Articy variables and current block
        var currentRef = introBlock;
        var articyObj = currentRef.GetObject<ArticyObject>();
        sm.SetActiveBlock(articyObj.TechnicalName);

        // 6) Unpause player
        PlayerController pc = player3D.GetComponent<PlayerController>();
        pc.gameStarted = true;

        // 7) Disable the main menu canvas
        mainMenuUI.SetActive(false);
    }
}
