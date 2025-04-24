using System;
using System.Collections;
using System.Collections.Generic;
using Articy.Articy_Tutorial;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public List<BattleData> allBattles;
    public List<BattleParticipant> allParticipants;
    public List<BattleParticipant> enemies;
    public BattleParticipant player;
    public List<BattleParticipant> friendlies;
    public GameObject player3D;
    public GameObject battleUI;
    public StoryManager storyManager;
    public string[] postBattleQueue;
    public BattleData currentBattleData;
    private bool buttonsActive = false;

    public void FindBattleByID(string battleID, string[] queue)
    {
        // clear data
        allParticipants.Clear();
        enemies.Clear();
        currentBattleData = null;

        // replace the queue
        postBattleQueue = queue;

        foreach (BattleData battle in allBattles)
        {
            if (battle.battleID == battleID)
            {
                // save battle data for later use
                currentBattleData = battle;
                // Start the battle with the found battle data
                StartBattle(battle);
                return;
            }
        }
        Debug.LogError("Battle with ID " + battleID + " not found.");
    }

    public void StartBattle(BattleData battleData)
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
    }

    public void battleStatusUpdater()
    {
        // check if enemies are dead and remove them if so
        foreach (BattleParticipant fighter in enemies)
        {
            if (fighter.HP <= 0)
            {
                allParticipants.Remove(fighter);
                Destroy(GameObject.Find(fighter.participantName)); // Destroy the game object
            }
        }

        // check if player is dead and end the game if so
        if (player.HP <= 0)
        {
            Debug.Log("Game Over! Player is dead.");
            return;
        }

        // check if battle is over and revert to the story manager with the rest of the queue
        if (enemies.Count == 0)
        {
            string nextThing = postBattleQueue[0];
            string nextThingType = nextThing.Split('_')[0];
            string nextThingID = nextThing.Split('_')[1];

            // clear the battle UI
            battleUI.SetActive(false); // Deactivate the battle UI

            // teleport back to the original location
            PlayerController playerController = player3D.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.isInBattle = false; // Set the battle flag to false
            }
            else
            {
                Debug.LogError("PlayerController not found on player object.");
            }

            // destroy the battle environment
            string battleEnvironmentName = currentBattleData.battleEnvironmentPrefab.name;
            Destroy(GameObject.Find(battleEnvironmentName)); // Destroy the battle environment

            // destroy the player
            Destroy(GameObject.Find(player.participantName)); // Destroy the player object

            // play the next thing in the queue
            if (nextThingType == "dialogue")
            {
                var nextDialogue = ArticyDatabase.GetObject(nextThingID) as DialogueFragment;
                if (nextDialogue != null)
                {
                    // cut the queue to remove the first element
                    string[] nextThingQueue = new string[postBattleQueue.Length - 1];
                    Array.Copy(postBattleQueue, 1, nextThingQueue, 0, postBattleQueue.Length - 1);
                    // start the next dialogue
                    storyManager.StartSectionFromDialogue(nextDialogue, nextThingQueue);
                }
                else
                {
                    Debug.LogError("Next dialogue not found: " + nextThingID);
                }
            }
            else if (nextThingType == "battle")
            {
                string[] nextThingQueue = new string[postBattleQueue.Length - 1];
                Array.Copy(postBattleQueue, 1, nextThingQueue, 0, postBattleQueue.Length - 1);
                storyManager.StartSectionFromBattle(nextThingID, nextThingQueue);
            }
            else if (nextThingType == "nothing")
            {
                Debug.LogError("No more things in the queue after battle.");
            }
        }
    }

    public void ToggleButtons()
    {
        // 1. unblock the ui buttons
        GameObject[] buttons = new GameObject[6];
        buttons[0] = GameObject.Find("AttackButton");
        buttons[1] = GameObject.Find("SpellButton");
        buttons[2] = GameObject.Find("ItemButton");
        buttons[3] = GameObject.Find("SkipButton");
        buttons[4] = GameObject.Find("CharacterButton");
        buttons[5] = GameObject.Find("MenuButton");

        foreach (GameObject button in buttons)
        {
            if (button != null)
            {
                if (buttonsActive)
                {
                    button.GetComponent<Button>().interactable = false; // Enable the button
                    buttonsActive = false; // Set the flag to false
                }
                else
                {
                    button.GetComponent<Button>().interactable = true; // Disable the button
                    buttonsActive = true; // Set the flag to true
                }
            }
            else
            {
                Debug.LogError("Button not found: " + button.name);
            }
        }
    }

    // make functions:
    // 2. get player's input for this turn's action
    // 3. if the action is an attack, change the cursor to a crosshair and allow the player to select a target
    // 4. then attack the target and deal damage
    // 5. end the turn automatically, disabling the ui buttons
    // 6. call battleStatusUpdater() to check if the battle is over
    // 7. call the enemies turn function

    public void OnActionButtonClicked(string actionType)
    {
        switch (actionType)
        {
            case "Attack":
                // Change cursor to crosshair and allow player to select a target
                // Implement target selection logic here
                break;
            case "Spell":
                // Implement spell casting logic here
                break;
            case "Item":
                // Implement item usage logic here
                break;
            case "Skip":
                // Skip the turn logic here
                break;
            case "Character":
                // Show character stats or abilities here
                break;
            case "Menu":
                // Open the menu here
                break;
            default:
                Debug.LogError("Unknown action type: " + actionType);
                break;
        }
    }

    public void EnemiesTurn()
    {
        foreach (BattleParticipant enemy in enemies)
        {
            // enemy AI logic here
        }
    }
}
