// QueueSO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Articy.Articy_Tutorial;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "Queue", menuName = "ScriptableObjects/Queue", order = 4)]
public class Queue : ScriptableObject
{
    [Tooltip("Define an ordered sequence of Dialogue or Battle entries.")]
    public List<QueueEntry> entries = new List<QueueEntry>();

    [Serializable]
    public class QueueEntry
    {
        public enum EntryType
        {
            Dialogue,
            Battle,
            None
        }

        public EntryType entryType;
        public ArticyRef dialogue;
        public BattleData battle;

        /// <summary>
        /// Returns a tag like "dialogue_DialogueID" or "battle_BattleID",
        /// used by StoryManager to enqueue next sections.
        /// </summary>
        public string ID
        {
            get
            {
                switch (entryType)
                {
                    case EntryType.Dialogue:
                        if (dialogue != null)
                            return $"dialogue_{dialogue.GetObject().Id}";
                        break;
                    case EntryType.Battle:
                        if (battle != null)
                            return $"battle_{battle.battleID}";
                        Debug.LogWarning("QueueEntry: BattleData is null");
                        break;
                }
                return "nothing_";
            }
        }
    }

    /// <summary>
    /// Returns the array of IDs to feed into StoryManager's StartSection methods.
    /// </summary>
    public string[] IDs
    {
        get { return entries.Select(e => e.ID).ToArray(); }
    }
}
