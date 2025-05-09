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
        battleManager.FadeToFindBattleByID(battleData, pendingQueue);
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
}
