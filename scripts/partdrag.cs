using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class PartDragHandler : MonoBehaviour
{
    // AR Components
    private ARRaycastManager _raycastManager;
    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    // Drag Mechanics
    private bool _isDragging = false;
    private Rigidbody _rb;
    private Vector3 _offset;
    private float _dragDistance = 0.1f;

    void Start()
    {
        // Get required components
        _raycastManager = FindObjectOfType<ARRaycastManager>();
        _rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody is configured properly
        if (_rb != null)
        {
            _rb.isKinematic = true; // Start kinematic for precise placement
            _rb.useGravity = false;
        }
        else
        {
            Debug.LogError("Rigidbody missing on " + gameObject.name);
        }
    }

    void Update()
    {
        // Handle touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Convert touch position to ray
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Check if we hit this object
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (hit.transform == transform)
                        {
                            _isDragging = true;

                            // Calculate offset between touch and object position
                            _offset = transform.position - GetARPosition(touch.position);

                            // Visual feedback (optional)
                            GetComponent<Renderer>().material.color = Color.yellow;
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (_isDragging)
                    {
                        // Move object with AR surface tracking
                        Vector3 targetPosition = GetARPosition(touch.position) + _offset;
                        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.2f);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_isDragging)
                    {
                        _isDragging = false;

                        // Reset visual feedback
                        GetComponent<Renderer>().material.color = Color.white;
                    }
                    break;
            }
        }
    }

    // Helper method to get AR surface position
    private Vector3 GetARPosition(Vector2 screenPosition)
    {
        if (_raycastManager.Raycast(screenPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;
            return hitPose.position;
        }

        // Fallback: use current position if no AR plane detected
        return transform.position;
    }

    // Called when part is correctly snapped
    public void OnSnapped()
    {
        // Disable dragging
        enabled = false;

        // Visual confirmation
        GetComponent<Renderer>().material.color = Color.green;

        // Make physics static
        if (_rb != null)
        {
            _rb.isKinematic = true;
        }
    }
}