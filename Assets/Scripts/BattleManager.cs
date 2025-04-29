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
    public Animator playerAnimator;
    private GameObject battleEnvInstance;
    public List<BattleParticipant> allParticipants;
    public List<BattleParticipant> enemies;
    public BattleParticipant player;
    public List<BattleParticipant> friendlies;
    public GameObject player3D;
    public GameObject battleUI;
    public GameObject actionsPanel;
    public GameObject backPanel;
    public GameObject spellsPanel;
    public StoryManager storyManager;
    public List<Queue.QueueEntry> postBattleQueue;
    public BattleData currentBattleData;
    private GameObject playerBattleModel;
    private GameObject[] enemyBattleModels;
    private BattleParticipant[] selectedTargets = new BattleParticipant[0];

    private string selectedMoveType;
    private int selectedMoveTargetCount;
    private BattleMove selectedMove;
    private int enemiesAttacked = 0; // counter for the number of enemies attacked

    public void FindBattleByID(BattleData battleData, List<Queue.QueueEntry> queue)
    {
        // clear data
        allParticipants = new List<BattleParticipant>();
        enemies = new List<BattleParticipant>();
        friendlies = new List<BattleParticipant>();
        player = null;
        currentBattleData = null;

        // replace the queue
        postBattleQueue = new List<Queue.QueueEntry>(queue);

        // update the battle data
        currentBattleData = ScriptableObject.CreateInstance<BattleData>();
        currentBattleData.battleID = battleData.battleID;
        currentBattleData.battleEnvironmentPrefab = battleData.battleEnvironmentPrefab;
        currentBattleData.participants = new List<BattleParticipant>(battleData.participants);
        StartBattle(battleData); // Start the battle with the provided data
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
        battleEnvInstance = Instantiate(
            battleData.battleEnvironmentPrefab,
            battleLocation,
            Quaternion.identity
        );

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
            playerBattleModel = spawnedPlayer; // Save the reference to the spawned player model
            playerBattleModel.name = player.participantName; // Set the name of the spawned object
            playerAnimator = playerBattleModel.GetComponent<Animator>(); // Get the animator component
            HealthBarUpdater(player, playerBattleModel); // Update the health bar for the player
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
        enemyBattleModels = new GameObject[enemies.Count];
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
                enemyBattleModels[i] = spawnedEnemy; // Save the reference to the spawned enemy model
                spawnedEnemy.name = enemies[i].participantName; // Set the name of the spawned object
                HealthBarUpdater(enemies[i], spawnedEnemy); // Update the health bar for the enemy
            }
            else
            {
                Debug.LogError("Prefab not found for enemy: " + enemies[i].participantName);
            }
        }
        // create battle UI
        battleUI.SetActive(true); // Activate the battle UI
        // unblock the buttons
        ToggleButtons(true);
    }

    public void HealthBarUpdater(BattleParticipant unit, GameObject unitModel)
    {
        // find the health bar
        Image fillBar = unitModel.transform.Find("Canvas/FillBar").GetComponent<Image>();
        if (fillBar != null)
        {
            fillBar.fillAmount = (float)unit.HP / (float)unit.maxHP; // Update the health bar fill amount
        }
        else
        {
            Debug.LogError("FillBar not found in the enemy model hierarchy.");
        }
    }

    public bool BattleStatusUpdater()
    {
        bool battleOver = false;
        List<BattleParticipant> dead = new List<BattleParticipant>();
        // check if enemies are dead and remove them if so
        foreach (BattleParticipant fighter in enemies)
        {
            if (fighter.HP <= 0)
            {
                allParticipants.Remove(fighter);
                dead.Add(fighter); // Add the dead enemy to the list
                enemyBattleModels = Array.FindAll(
                    enemyBattleModels,
                    model => model.name != fighter.participantName
                );
                Destroy(GameObject.Find(fighter.participantName)); // Destroy the game object
            }
        }
        // remove dead enemies from the list
        foreach (BattleParticipant fighter in dead)
        {
            enemies.Remove(fighter); // Remove the dead enemy from the enemies list
        }

        // check if player is dead and end the game if so
        if (player.HP <= 0)
        {
            Debug.Log("Game Over! Player is dead.");
            return true;
        }

        // check if battle is over and revert to the story manager with the rest of the queue
        if (enemies.Count == 0)
        {
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
            Destroy(GameObject.Find(battleEnvInstance.name));

            // destroy the player
            Destroy(GameObject.Find(player.participantName)); // Destroy the player object

            storyManager.StartQueueFromList(postBattleQueue); // Start the next queue in the story manager
            battleOver = true; // Set battle over flag to true
        }
        return battleOver; // Return the battle over status
    }

    public void ToggleButtons(bool on)
    {
        // unblock the ui buttons
        GameObject[] buttons = new GameObject[6];
        buttons[0] = GameObject.Find("AttackButton");
        buttons[1] = GameObject.Find("SpellButton");
        buttons[2] = GameObject.Find("ItemButton");
        buttons[3] = GameObject.Find("SkipButton");
        buttons[4] = GameObject.Find("CharacterButton");
        buttons[5] = GameObject.Find("MenuButton");

        if (!on)
        {
            foreach (GameObject button in buttons)
            {
                if (button != null)
                {
                    button.GetComponent<Button>().interactable = false;
                }
                else
                {
                    Debug.LogError("Button not found.");
                }
            }
        }
        else
        {
            foreach (GameObject button in buttons)
            {
                if (button != null)
                {
                    button.GetComponent<Button>().interactable = true;
                }
                else
                {
                    Debug.LogError("Button not found.");
                }
            }
        }
    }

    public void ToActionPanel()
    {
        // 1) Remove any outline
        foreach (var enemyGo in enemyBattleModels)
        {
            var outline = enemyGo.GetComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineWidth = 0;
            }
        }

        // 2) Remove TargetableEnemy so we fully exit targeting mode
        foreach (var enemyGo in enemyBattleModels)
        {
            var ts = enemyGo.GetComponent<TargetableEnemy>();
            if (ts != null)
                Destroy(ts);
        }

        // 3) Reset all our selection state
        selectedMoveType = null;
        selectedTargets = new BattleParticipant[0];
        selectedMoveTargetCount = 0;
        selectedMove = null;

        // 4) Swap your UI panels
        actionsPanel.SetActive(true);
        backPanel.SetActive(false);
        spellsPanel.SetActive(false);
    }

    public void ToBackPanel()
    {
        // hide other panels and show the back panel
        actionsPanel.SetActive(false); // Hide the action panel
        backPanel.SetActive(true); // Show the back panel
        spellsPanel.SetActive(false); // Hide the spells panel
    }

    public void ToSpellsPanel()
    {
        // hide other panels and show the spells panel
        actionsPanel.SetActive(false); // Hide the action panel
        backPanel.SetActive(false); // Hide the back panel
        spellsPanel.SetActive(true); // Show the spells panel
        // change the buttons of the spells panel to moves available to the player
        Button[] spellButtons = new Button[5];
        spellButtons[0] = GameObject.Find("SpellButton1").GetComponent<Button>();
        spellButtons[1] = GameObject.Find("SpellButton2").GetComponent<Button>();
        spellButtons[2] = GameObject.Find("SpellButton3").GetComponent<Button>();
        spellButtons[3] = GameObject.Find("SpellButton4").GetComponent<Button>();
        spellButtons[4] = GameObject.Find("SpellButton5").GetComponent<Button>();
        for (int i = 0; i < player.moves.Count; i++)
        {
            var move = player.moves[i]; // <-- copy into local
            var button = spellButtons[i];
            button.interactable = true;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SpellSelected(move)); // uses local `move`
            button.GetComponentInChildren<Text>().text = move.moveName;
        }
        if (player.moves.Count < spellButtons.Length)
        {
            for (int j = player.moves.Count; j < spellButtons.Length; j++)
            {
                var button = spellButtons[j];
                button.interactable = false;
                button.onClick.RemoveAllListeners();
                button.GetComponentInChildren<Text>().text = ""; // Set the text to "Empty" for empty buttons
            }
        }
    }

    public void OnActionButtonClicked(string actionType)
    {
        // reset the selected move target count to 0
        selectedMoveTargetCount = 0;
        // reset the selected targets to null
        selectedTargets = new BattleParticipant[0];
        // reset the selected move type to null
        selectedMoveType = null;

        switch (actionType)
        {
            case "Attack":
                selectedMoveType = "Attack"; // Set the selected move type to attack
                selectedMoveTargetCount = 1; // Set the number of targets for
                TargetsSelector(); // Call the target selector function
                ToBackPanel();
                break;
            case "Spell":
                selectedMoveType = "Spell"; // Set the selected move type to spell
                ToSpellsPanel();
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

    public void TargetsSelector()
    {
        foreach (GameObject enemy in enemyBattleModels)
        {
            // Check if the TargetableEnemy component already exists
            if (enemy.GetComponent<TargetableEnemy>() == null)
            {
                TargetableEnemy targetScript = enemy.AddComponent<TargetableEnemy>();
                BattleParticipant targetParticipant = enemies.Find(p =>
                    p.participantName == enemy.name
                );
                targetScript.Initialize(this, targetParticipant);
            }
        }
    }

    public void ToggleTarget(BattleParticipant target)
    {
        // Check if the target is already selected
        bool isTargetSelected = Array.Exists(selectedTargets, t => t == target);

        if (isTargetSelected)
        {
            // Deselect the target
            selectedTargets = Array.FindAll(selectedTargets, t => t != target);

            // Remove the outline from the target
            GameObject targetModel = GameObject.Find(target.participantName);
            if (targetModel != null)
            {
                Outline outline = targetModel.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.OutlineWidth = 0; // Remove the outline effect
                }
            }
            else
            {
                Debug.LogError("Target model not found: " + target.participantName);
            }
        }
        else
        {
            // Select the target
            Array.Resize(ref selectedTargets, selectedTargets.Length + 1);
            selectedTargets[selectedTargets.Length - 1] = target;

            // add the outline to the target
            GameObject targetModel = GameObject.Find(target.participantName);
            if (targetModel != null)
            {
                Outline outline = targetModel.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.OutlineWidth = 5f; // Add the outline effect
                }
            }
            else
            {
                Debug.LogError("Target model not found: " + target.participantName);
            }
        }

        // Check if all targets are selected
        AreAllTargetsSelected(selectedTargets);
    }

    public void AreAllTargetsSelected(BattleParticipant[] targets)
    {
        if (targets.Length == selectedMoveTargetCount)
        {
            switch (selectedMoveType)
            {
                case "Attack":
                    foreach (BattleParticipant target in targets)
                    {
                        // remote the outline from the target
                        GameObject targetModel = GameObject.Find(target.participantName);
                        if (targetModel != null)
                        {
                            Outline outline = targetModel.GetComponent<Outline>();
                            if (outline != null)
                            {
                                outline.OutlineWidth = 0; // Remove the outline effect
                            }
                        }
                        else
                        {
                            Debug.LogError("Target model not found: " + target.participantName);
                        }
                        BattleParticipant[] singleTargetArray = new BattleParticipant[1];
                        singleTargetArray[0] = target; // Create an array with the single target
                        StartCoroutine(PlayerAttackRoutine(singleTargetArray, selectedMove)); // Start the attack animation
                    }
                    break;
                case "Spell":
                    foreach (BattleParticipant target in targets)
                    {
                        // remote the outline from the target
                        GameObject targetModel = GameObject.Find(target.participantName);
                        if (targetModel != null)
                        {
                            Outline outline = targetModel.GetComponent<Outline>();
                            if (outline != null)
                            {
                                outline.OutlineWidth = 0; // Remove the outline effect
                            }
                        }
                        else
                        {
                            Debug.LogError("Target model not found: " + target.participantName);
                        }
                    }
                    StartCoroutine(PlayerAttackRoutine(targets, selectedMove)); // Start the attack animation
                    break;
                default:
                    Debug.LogError("Unknown move type: " + selectedMoveType);
                    break;
            }
        }
    }

    public void SpellSelected(BattleMove spell)
    {
        // Check if the spell is valid and can be cast
        if (spell != null && player.MP >= spell.MPcost)
        {
            // Deduct the MP cost from the player
            player.MP -= spell.MPcost;

            // Call the target selector function to select targets for the spell
            selectedMoveType = "Spell"; // Set the selected move type to spell
            selectedMoveTargetCount = spell.numberOfTargets; // Set the number of targets for the spell
            selectedMove = spell; // Set the selected move to the spell
            TargetsSelector();
            ToBackPanel(); // Show the back panel
        }
        else
        {
            Debug.LogError("Invalid spell or insufficient MP: " + spell.moveName);
        }
    }

    private IEnumerator PlayerAttackRoutine(BattleParticipant[] targets, BattleMove move)
    {
        // move the player towards the target
        Vector3 startPosition = playerBattleModel.transform.position;
        // pick a point a little in front of the target
        Vector3 destinationPosition = GameObject
            .Find(targets[0].participantName)
            .transform.position;
        if (targets.Length > 1)
        {
            // pick a point in the middle of the targets
            destinationPosition = Vector3.zero;
            foreach (BattleParticipant target in targets)
            {
                destinationPosition += GameObject.Find(target.participantName).transform.position;
            }
            destinationPosition /= targets.Length; // Average the positions of the targets
        }
        // move the player towards the target
        Vector3 hitOffset = (destinationPosition - startPosition).normalized * 1.5f;
        Vector3 attackPosition = destinationPosition - hitOffset;
        float moveDuration = 0.5f; // Duration of the move
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            playerBattleModel.transform.position = Vector3.Lerp(startPosition, attackPosition, t);
            yield return null;
        }

        // animation
        playerAnimator.SetBool("isAttacking", true);
        float clipLength = 0.8f;
        yield return new WaitForSeconds(clipLength);
        playerAnimator.SetBool("isAttacking", false);

        // move back
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            playerBattleModel.transform.position = Vector3.Lerp(attackPosition, startPosition, t);
            yield return null;
        }
        playerBattleModel.transform.position = startPosition; // ensure exact

        // logic
        foreach (BattleParticipant target in targets)
        {
            if (move == null)
            {
                target.HP -= player.DMG; // Apply damage to the target's HP
            }
            else
            {
                target.HP -= move.DMG; // Apply damage to the target's HP
            }
            HealthBarUpdater(target, GameObject.Find(target.participantName)); // Update the health bar for the target
        }
        ToActionPanel();
        ToggleButtons(false);
        bool battleOver = BattleStatusUpdater();
        if (!battleOver)
        {
            // If the battle is not over, proceed to the enemy's turn
            EnemiesTurn();
        }
    }

    public void EnemiesTurn()
    {
        StartCoroutine(EnemyAttackRoutine(enemies[0])); // Start the enemy attack animation
    }

    private IEnumerator EnemyAttackRoutine(BattleParticipant enemy)
    {
        // get the enemy's battle model
        GameObject enemyModel = GameObject.Find(enemy.participantName);
        if (enemyModel == null)
        {
            Debug.LogError("Enemy model not found: " + enemy.participantName);
            yield break; // Exit the coroutine if the model is not found
        }
        // move the enemy towards the player
        Vector3 startPosition = enemyModel.transform.position;
        Transform targetTransform = playerBattleModel.transform;
        Vector3 hitOffset = (targetTransform.position - startPosition).normalized * 1.5f;
        Vector3 attackPosition = targetTransform.position - hitOffset;
        float moveDuration = 0.5f; // Duration of the move
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            enemyModel.transform.position = Vector3.Lerp(startPosition, attackPosition, t);
            yield return null;
        }
        // move back
        elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            enemyModel.transform.position = Vector3.Lerp(attackPosition, startPosition, t);
            yield return null;
        }
        enemyModel.transform.position = startPosition; // ensure exact
        // logic
        player.HP -= enemy.DMG;
        HealthBarUpdater(player, playerBattleModel); // Update the health bar for the player
        enemiesAttacked++; // Increment the attack counter
        bool battleOver = BattleStatusUpdater();
        if (enemiesAttacked != enemies.Count)
        {
            StartCoroutine(EnemyAttackRoutine(enemies[enemiesAttacked])); // Start the next enemy attack
        }
        else if (!battleOver)
        {
            // If the battle is not over and all enemies have gone, proceed to the player's turn
            enemiesAttacked = 0; // Reset the attack counter
            ToggleButtons(true);
        }
    }
}
