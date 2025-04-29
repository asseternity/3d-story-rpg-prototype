using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Articy.Articy_Tutorial;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour, IArticyFlowPlayerCallbacks
{
    // Battle manager
    public BattleManager battleManager;

    // Assigning very next dialogue or battles
    public string nextThingID;
    public string nextThingType;

    // Assigning the current queue
    public string[] currentQueue;

    // Assigning the queue of next dialogues or battles
    public string[] nextThingQueue;
    List<Queue.QueueEntry> pendingQueue;

    [Header("UI")]
    // Reference to Dialog UI
    [SerializeField]
    GameObject dialogueWidget;

    // Reference to dialogue text
    [SerializeField]
    Text dialogueText;

    // Reference to speaker
    [SerializeField]
    Text dialogueSpeaker;

    // Refernce to sprite
    [SerializeField]
    Image dialogueSprite;

    [SerializeField]
    RectTransform branchLayoutPanel;

    [SerializeField]
    GameObject branchPrefab;

    [SerializeField]
    GameObject closePrefab;

    // To check if we are currently showing the dialog ui interface
    public bool DialogueActive { get; set; }

    private ArticyFlowPlayer flowPlayer;

    void Start()
    {
        flowPlayer = GetComponent<ArticyFlowPlayer>();
    }

    public void StartQueue(Queue queue)
    {
        pendingQueue = new List<Queue.QueueEntry>(queue.entries);
        DispatchNextEntry();
    }

    public void StartQueueFromList(List<Queue.QueueEntry> queue)
    {
        pendingQueue = queue;
        DispatchNextEntry();
    }

    public void DispatchNextEntry()
    {
        if (pendingQueue == null || pendingQueue.Count == 0)
        {
            ShowCloseButton();
            return;
        }

        switch (pendingQueue[0].entryType)
        {
            case Queue.QueueEntry.EntryType.Dialogue:
                CommenceDialogue(pendingQueue[0].dialogue.GetObject());
                break;
            case Queue.QueueEntry.EntryType.Battle:
                CommenceBattle(pendingQueue[0].battle);
                break;
            default:
                ShowCloseButton();
                return;
        }
    }

    private void CommenceDialogue(IArticyObject aObject)
    {
        pendingQueue.RemoveAt(0);
        DialogueActive = true;
        dialogueWidget.SetActive(DialogueActive);
        flowPlayer.StartOn = aObject;
    }

    private void CommenceBattle(BattleData battleData)
    {
        pendingQueue.RemoveAt(0);
        DialogueActive = false;
        dialogueWidget.SetActive(DialogueActive);
        battleManager.FindBattleByID(battleData, pendingQueue);
    }

    /// <summary>
    /// Called by ArticyFlowPlayer whenever dialogue text appears.
    /// </summary>
    public void OnFlowPlayerPaused(IFlowObject aObject)
    {
        //Clear data
        dialogueText.text = string.Empty;
        dialogueSpeaker.text = string.Empty;

        // If we paused on an object that has a "Text" property fetch this text and present it
        var objectWithText = aObject as IObjectWithLocalizableText;
        if (objectWithText != null)
        {
            dialogueText.text = objectWithText.Text;
        }

        // If we paused on an object with a speaker name
        var objectWithSpeaker = aObject as IObjectWithSpeaker;
        if (objectWithSpeaker != null)
        {
            // If the object has a "Speaker" property, fetch the reference
            // and ensure it is really set to an "Entity" object to get its "DisplayName"
            var speakerEntity = objectWithSpeaker.Speaker as Entity;
            if (speakerEntity != null)
            {
                dialogueSpeaker.text = speakerEntity.DisplayName;
                // Fetch the sprite from the entity's asset or preview image
                if (speakerEntity.PreviewImage != null)
                {
                    dialogueSprite.sprite = speakerEntity.PreviewImage.Asset.LoadAssetAsSprite();
                    dialogueSprite.gameObject.SetActive(true); // Ensure the image is visible
                }
                else
                {
                    dialogueSprite.gameObject.SetActive(false); // Hide the image if no sprite is available
                }
            }
        }
    }

    /// <summary>
    /// Called by ArticyFlowPlayer when branch choices update.
    /// When there are no more dialogue-branches, we move on.
    /// </summary>
    public void OnBranchesUpdated(IList<Branch> aBranches)
    {
        ClearAllBranches();
        bool dialogIsFinished = true;
        foreach (var b in aBranches)
            if (b.Target is IDialogueFragment)
                dialogIsFinished = false;
        if (!dialogIsFinished)
        {
            // still got choices → show them
            foreach (var b in aBranches)
            {
                var btn = Instantiate(branchPrefab, branchLayoutPanel);
                btn.GetComponent<BranchChoice>().AssignBranch(flowPlayer, b);
            }
        }
        else
        {
            // no more dialogue here → go do the next queued item
            DispatchNextEntry();
        }
    }

    public void CloseDialogueBox()
    {
        DialogueActive = false;
        dialogueWidget.SetActive(DialogueActive);
        flowPlayer.FinishCurrentPausedObject();
    }

    private void ShowCloseButton()
    {
        ClearAllBranches();
        var btn = Instantiate(closePrefab, branchLayoutPanel);
        btn.GetComponent<Button>().onClick.AddListener(CloseDialogueBox);
    }

    private void ClearAllBranches()
    {
        foreach (Transform c in branchLayoutPanel)
            Destroy(c.gameObject);
    }

    // public void StartSection(Queue queue)
    // {
    //     // reset data
    //     nextThingType = "nothing";
    //     nextThingID = "";
    //     nextThingQueue = null;
    //     currentQueue = null;
    //     // this should call either StartSectionFromDialogue or StartSectionFromBattle
    //     // depending on the first entry in the queue
    //     if (queue.entries.Count > 0)
    //     {
    //         var firstQueueEntry = queue.entries[0];
    //         switch (firstQueueEntry.entryType)
    //         {
    //             case Queue.QueueEntry.EntryType.Dialogue:
    //                 // save both types of string array
    //                 currentQueue = queue.IDs;
    //                 nextThingQueue = new string[queue.entries.Count - 1];
    //                 Array.Copy(queue.IDs, 1, nextThingQueue, 0, queue.entries.Count - 1);
    //                 // start the dialogue
    //                 StartSectionFromDialogue(firstQueueEntry.dialogue.GetObject(), nextThingQueue);
    //                 break;
    //             case Queue.QueueEntry.EntryType.Battle:
    //                 // pop the top of the array
    //                 nextThingQueue = new string[queue.entries.Count - 1];
    //                 Array.Copy(queue.IDs, 1, nextThingQueue, 0, queue.entries.Count - 1);
    //                 // start the battle
    //                 StartSectionFromBattle(firstQueueEntry.battle.battleID, nextThingQueue);
    //                 break;
    //             default:
    //                 Debug.LogError("Unknown entry type in queue: " + firstQueueEntry.entryType);
    //                 break;
    //         }
    //     }
    // }

    // public void StartSectionFromDialogue(IArticyObject aObject, string[] queue)
    // {
    //     // clear data
    //     nextThingType = "nothing";
    //     nextThingID = "";

    //     DialogueActive = true;
    //     dialogueWidget.SetActive(DialogueActive);
    //     flowPlayer.StartOn = aObject;
    //     flowPlayer.Play();
    // }

    // public void CloseDialogueBox()
    // {
    //     DialogueActive = false;
    //     dialogueWidget.SetActive(DialogueActive);
    //     // Last object might have an output pin containing scripts, so we execute them by telling flowPlayer to finish current paused object
    //     flowPlayer.FinishCurrentPausedObject();
    // }

    // // This is called every time the flow player reaches an object of interest
    // public void OnFlowPlayerPaused(IFlowObject aObject)
    // {
    //     //Clear data
    //     dialogueText.text = string.Empty;
    //     dialogueSpeaker.text = string.Empty;

    //     // If we paused on an object that has a "Text" property fetch this text and present it
    //     var objectWithText = aObject as IObjectWithLocalizableText;
    //     if (objectWithText != null)
    //     {
    //         dialogueText.text = objectWithText.Text;
    //     }

    //     // If we paused on an object with a speaker name
    //     var objectWithSpeaker = aObject as IObjectWithSpeaker;
    //     if (objectWithSpeaker != null)
    //     {
    //         // If the object has a "Speaker" property, fetch the reference
    //         // and ensure it is really set to an "Entity" object to get its "DisplayName"
    //         var speakerEntity = objectWithSpeaker.Speaker as Entity;
    //         if (speakerEntity != null)
    //         {
    //             dialogueSpeaker.text = speakerEntity.DisplayName;
    //             // Fetch the sprite from the entity's asset or preview image
    //             if (speakerEntity.PreviewImage != null)
    //             {
    //                 dialogueSprite.sprite = speakerEntity.PreviewImage.Asset.LoadAssetAsSprite();
    //                 dialogueSprite.gameObject.SetActive(true); // Ensure the image is visible
    //             }
    //             else
    //             {
    //                 dialogueSprite.gameObject.SetActive(false); // Hide the image if no sprite is available
    //             }
    //         }
    //     }
    // }

    // // Called every time the flow player encounters multiple branches,
    // // or is paused on a node and wants to tell us how to continue
    // public void OnBranchesUpdated(IList<Branch> aBranches)
    // {
    //     // Destroy buttons from previous use, will create new ones here
    //     ClearAllBranches();

    //     // Here we get passed a list of all branches following the current node
    //     // So we check if any branch leads to a DialogueFragment target
    //     // If so, the dialogue is not yet finished
    //     bool dialogIsFinished = true;
    //     foreach (var branch in aBranches)
    //     {
    //         if (branch.Target is IDialogueFragment)
    //         {
    //             dialogIsFinished = false;
    //         }
    //     }
    //     if (!dialogIsFinished)
    //     {
    //         foreach (var branch in aBranches)
    //         {
    //             GameObject btn = Instantiate(branchPrefab, branchLayoutPanel);
    //             btn.GetComponent<BranchChoice>().AssignBranch(flowPlayer, branch);
    //         }
    //         return;
    //     }
    //     else
    //     {
    //         // next thing's tag would be: "dialogue/battle_dialogueID/battleID"
    //         // so now, parse the nextThing string
    //         string[] nextThingParts = currentQueue[0].Split('_');
    //         if (nextThingParts[0] == "dialogue")
    //         {
    //             nextThingType = "dialogue";
    //             nextThingID = nextThingParts[1];
    //             nextThingQueue = new string[currentQueue.Length - 1];
    //             Array.Copy(currentQueue, 1, nextThingQueue, 0, currentQueue.Length - 1);
    //             var nextDialogue = ArticyDatabase.GetObject(nextThingID) as DialogueFragment;
    //             if (nextDialogue != null)
    //             {
    //                 // start the next dialogue
    //                 StartSectionFromDialogue(nextDialogue, nextThingQueue);
    //             }
    //             else
    //             {
    //                 Debug.LogError("Next dialogue not found: " + nextThingID);
    //             }
    //         }
    //         else if (nextThingParts[0] == "battle")
    //         {
    //             nextThingType = "battle";
    //             nextThingID = nextThingParts[1];
    //             nextThingQueue = new string[currentQueue.Length - 1];
    //             Array.Copy(currentQueue, 1, nextThingQueue, 0, currentQueue.Length - 1);
    //             // how to queue up the next thing after the battle?
    //             StartSectionFromBattle(nextThingID, nextThingQueue);
    //         }
    //         else
    //         {
    //             nextThingType = "nothing";
    //             nextThingID = "";
    //             // there is no more dialogue
    //             GameObject btn = Instantiate(closePrefab, branchLayoutPanel);
    //             var btnComp = btn.GetComponent<Button>();
    //             btnComp.onClick.AddListener(CloseDialogueBox);
    //         }
    //     }
    // }

    // private void ClearAllBranches()
    // {
    //     foreach (Transform child in branchLayoutPanel)
    //     {
    //         Destroy(child.gameObject);
    //     }
    // }

    // public void StartSectionFromBattle(string battleID, string[] queue)
    // {
    //     // clear data
    //     nextThingType = "nothing";
    //     nextThingID = "";

    //     // hide dialogue box
    //     DialogueActive = false;
    //     dialogueWidget.SetActive(DialogueActive);

    //     // pop the top of the queue
    //     nextThingQueue = new string[queue.Length - 1];
    //     Array.Copy(queue, 1, nextThingQueue, 0, queue.Length - 1);

    //     // trigger battle
    //     battleManager.FindBattleByID(battleID, nextThingQueue);
    // }

    // // testing area
    // public BattleData testBattleData;
    // public string[] testQueue = { "nothing" };

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         StartSectionFromBattle(testBattleData.battleID, testQueue);
    //     }
    // }
}
