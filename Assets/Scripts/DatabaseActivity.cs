using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseActivity : MonoBehaviour
{
    public List<Activity> allActivities;
    private Dictionary<string, Activity> activityByID;

    private void Awake()
    {
        activityByID = new Dictionary<string, Activity>();
        foreach (var activity in allActivities)
        {
            activityByID[activity.id] = activity;
        }
    }

    public Activity GetByID(string id)
    {
        activityByID.TryGetValue(id, out var activity);
        return activity;
    }
}
