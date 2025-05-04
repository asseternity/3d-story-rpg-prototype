using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    // This will:
    // 1) keep track of the current date and progress the date
    // 2) populate the game world with Activities based on the date
    // 3) keep track of Activity progress levels

    // An activity is:
    // - A list of Queues
    // - And a levels int

    // Calendar and world populating system
    public int day = 1;
    public int week = 1;
    public int month = 1;
    public int year = 1000;
    public int daysPerWeek = 7;
    public int daysPerMonth = 30;
    public int monthsPerYear = 12;
    private int totalDaysPassed = 0;
    private string[] daysOfWeek = new string[]
    {
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday"
    };
    public List<GameObject> activityStarters;

    public void AdvanceDay()
    {
        day++;
        totalDaysPassed++;

        if (day > daysPerMonth)
        {
            day = 1;
            month++;

            if (month > monthsPerYear)
            {
                month = 1;
                year++;
            }
        }

        string dayOfWeek = daysOfWeek[totalDaysPassed % daysOfWeek.Length];
        if (dayOfWeek == "Monday" && totalDaysPassed != 0)
        {
            week++;
        }

        foreach (GameObject AS in activityStarters)
        {
            ActivityStarter AS_script = AS.GetComponent<ActivityStarter>();
            if (AS_script.daysAvailable.Contains(day))
            {
                GameObject parentObject = AS.gameObject.transform.parent?.gameObject;
                parentObject.SetActive(true);
            }
            else
            {
                GameObject parentObject = AS.gameObject.transform.parent?.gameObject;
                parentObject.SetActive(false);
            }
        }
    }

    // Activity tracking system
    [Serializable]
    private struct ProgressEntry
    {
        public Activity activity;
        public int currentStage;
    }

    private List<ProgressEntry> progressedActivities = new List<ProgressEntry>();
    public StoryManager storyManager;

    public void StartActivity(Activity activityToStart)
    {
        // find or add an entry
        int index = progressedActivities.FindIndex(e => e.activity == activityToStart);
        if (index < 0)
        {
            progressedActivities.Add(
                new ProgressEntry { activity = activityToStart, currentStage = 0 }
            );
            index = progressedActivities.Count - 1;
        }

        // completion check
        var entry = progressedActivities[index];
        if (entry.currentStage >= entry.activity.stages.Count)
        {
            Debug.LogWarning($"Activity '{activityToStart.name}' is already complete.");
            return;
        }

        // start the next Queue
        var nextQueue = entry.activity.stages[entry.currentStage];
        storyManager.StartQueue(nextQueue);

        // advance the stage in our list
        entry.currentStage++;
        progressedActivities[index] = entry; // write back
    }
}
