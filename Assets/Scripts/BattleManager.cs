using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public List<BattleData> allBattles;
    public List<BattleParticipant> allParticipants;
    public List<BattleParticipant> enemies;
    public BattleParticipant player;
    public List<BattleParticipant> friendlies;
    public GameObject player3D;
    public GameObject battleUI;

    public void FindBattleByID(string battleID, string[] queue)
    {
        foreach (BattleData battle in allBattles)
        {
            if (battle.battleID == battleID)
            {
                // Start the battle with the found battle data
                StartBattle(battle, queue);
                return;
            }
        }
        Debug.LogError("Battle with ID " + battleID + " not found.");
    }

    public void StartBattle(BattleData battleData, string[] queue)
    {
        allParticipants = battleData.participants;
        enemies = battleData.participants.FindAll(p => !p.isPlayer && !p.isFriendly);
        player = battleData.participants.Find(p => p.isPlayer);
        friendlies = battleData.participants.FindAll(p => !p.isPlayer && p.isFriendly);

        // take away movement control
        // HOW TO DO: reference main player object here and take away movement control in the script
        PlayerController playerController = player3D.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.isInBattle = true; // Set the battle flag to true
        }
        else
        {
            Debug.LogError("PlayerController not found on player object.");
        }

        // teleport to battle location
        // HOW TO DO: spawn the environment from prefab to a predetermined location
        Vector3 battleLocation = new Vector3(500, 0, 0);
        Instantiate(battleData.battleEnvironmentPrefab, battleLocation, Quaternion.identity);

        // spawn enemies and player from prefabs
        // HOW TO DO: spawn the player at a predetermined location in the environment, then go through enemies
        Vector3 playerSpawnPosition = battleLocation + new Vector3(5, 1.31f, 0);
        GameObject playerPrefab = player.prefabModel;
        if (playerPrefab != null)
        {
            GameObject spawnedPlayer = Instantiate(
                playerPrefab,
                playerSpawnPosition,
                Quaternion.identity
            );
            spawnedPlayer.name = player.participantName; // Set the name of the spawned object
        }
        else
        {
            Debug.LogError("Prefab not found for player: " + player.participantName);
        }

        // move the camera
        Vector3 cameraPosition = new Vector3(
            playerSpawnPosition.x + 3,
            playerSpawnPosition.y + 5,
            playerSpawnPosition.z
        );
        Camera.main.transform.position = cameraPosition;
        Camera.main.transform.LookAt(battleLocation);

        // now spawn the enemies in a line, alternating between left and right
        int[] enemySpawnOrder = new int[5];
        enemySpawnOrder[0] = 0;
        enemySpawnOrder[1] = 2;
        enemySpawnOrder[2] = -2;
        enemySpawnOrder[3] = 4;
        enemySpawnOrder[4] = -4;
        for (int i = 0; i < enemies.Count; i++)
        {
            Vector3 spawnPosition = battleLocation + new Vector3(-5, 1.31f, enemySpawnOrder[i]);
            GameObject enemyPrefab = enemies[i].prefabModel;
            if (enemyPrefab != null)
            {
                GameObject spawnedEnemy = Instantiate(
                    enemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );
                spawnedEnemy.name = enemies[i].participantName; // Set the name of the spawned object
            }
            else
            {
                Debug.LogError("Prefab not found for enemy: " + enemies[i].participantName);
            }
        }
        // create battle UI
        battleUI.SetActive(true); // Activate the battle UI

        // start battle loop: player turn, enemy turn, allies turn if any
        // HOW TO DO:

        // check if units are dead and remove them if so
        // HOW TO DO:

        // check if battle is over and revert to the story manager with the rest of the queue
        // HOW TO DO:
    }

    public void PlayerTurn()
    {
        // player turn logic here
    }

    public void EnemiesTurn()
    {
        foreach (BattleParticipant enemy in enemies)
        {
            // enemy AI logic here
        }
    }
}
