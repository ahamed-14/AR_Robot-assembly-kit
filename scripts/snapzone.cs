using UnityEngine;

public class SnapZone : MonoBehaviour
{
    [Header("Settings")]
    public string expectedPartTag; // Set this in Inspector (e.g., "Arm")
    public float snapDistance = 0.05f; // How close parts need to be to snap
    public Color debugColor = Color.green; // Editor visualization

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(expectedPartTag))
        {
            float distance = Vector3.Distance(other.transform.position, transform.position);
            if (distance <= snapDistance)
            {
                // Snap the part into place
                other.transform.position = transform.position;
                other.transform.rotation = transform.rotation;

                // Notify the part it's snapped
                other.GetComponent<PartDragHandler>()?.OnSnapped();
            }
        }
    }

    // Visualize the snap zone in Scene View (editor-only)
    void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    }
}