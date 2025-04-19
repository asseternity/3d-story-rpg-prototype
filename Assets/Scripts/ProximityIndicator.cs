using System.Collections;
using UnityEngine;

public class ProximityIndicator : MonoBehaviour
{
    [Tooltip("Reference to the player's transform")]
    public Transform player;

    [Tooltip("Distance within which the indicator appears")]
    public float detectionRadius = 5f;

    void Update()
    {
        if (player == null)
            return;

        // Determine the distance from the player.
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            if (Input.GetButtonDown("Interact"))
            {
                Debug.Log("Player wants to interact with an NPC");
            }
        }
    }
}
