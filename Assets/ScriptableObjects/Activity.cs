// QueueSO.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Articy.Articy_Tutorial;
using Articy.Unity;
using Articy.Unity.Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "Activity", menuName = "ScriptableObjects/Activity", order = 4)]
public class Activity : ScriptableObject
{
    [Tooltip("Define an ordered sequence of Queues for this Activity.")]
    public List<Queue> stages = new List<Queue>();
}
