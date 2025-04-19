using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Assign the player's transform in the Inspector.
    public float distance = 15f; // Horizontal distance from the player.
    public float height = 10f; // Height above the player.
    public float angle = 45f; // Rotation around the Y-axis for a diagonal view.

    void LateUpdate()
    {
        // Calculate an offset that gives a fixed isometric angle.
        Vector3 offset = Quaternion.Euler(0, angle, 0) * new Vector3(0, height, -distance);
        transform.position = player.position + offset;
        // Make sure the camera looks at the player's current position.
        transform.LookAt(player.position);
    }
}
