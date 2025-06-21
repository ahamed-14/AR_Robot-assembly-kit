using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Explodecontroller : MonoBehaviour
{
    // List of all robot parts from the hierarchy
    public Transform Robot_gey; // Parent object
    public Transform left_arm;
    public Transform right_arm;
    public Transform head;
    public Transform legs;
    public Transform armor_part_1;
    public Transform armor_part_2;
    public Transform armor_part_3;
    public Transform armor_part_4;
    public Transform armor_part_5;
    public Transform armor_part_6;
    public Transform armor_part_7;
    public Transform armor_part_8;
    public Transform armor_part_9;

    private Vector3[] originalPositions;
    private Vector3[] explodedPositions;
    private Transform[] interactableObjects;

    private bool isExploded = false;
    public float moveSpeed = 2.0f;

    private Transform selectedObject = null;
    private Vector3 offset;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Initialize array with all interactable parts
        interactableObjects = new Transform[] {
            left_arm, right_arm, head, legs,
            armor_part_1, armor_part_2, armor_part_3,
            armor_part_4, armor_part_5, armor_part_6,
            armor_part_7, armor_part_8, armor_part_9
        };

        originalPositions = new Vector3[interactableObjects.Length];
        explodedPositions = new Vector3[interactableObjects.Length];

        // Store original positions and calculate exploded positions
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            if (interactableObjects[i] != null)
            {
                originalPositions[i] = interactableObjects[i].localPosition;
                explodedPositions[i] = originalPositions[i] + GetExplodedOffset(interactableObjects[i]);
            }
        }
    }

    Vector3 GetExplodedOffset(Transform obj)
    {
        // Reduced explosion distances for better containment
        if (obj == left_arm) return new Vector3(-0.8f, 0.3f, 0);  // Reduced from 1.5, 0.5
        if (obj == right_arm) return new Vector3(0.8f, 0.3f, 0);  // Reduced from 1.5, 0.5
        if (obj == head) return new Vector3(0, 0.23f, 0);          // Reduced from 1.5
        if (obj == legs) return new Vector3(0, -0.8f, 0);         // Reduced from 1.5

        // Spread armor parts in a tighter circular pattern
        if (obj.name.Contains("armor"))
        {
            int index = int.Parse(obj.name.Split('_')[2]);
            float angle = index * Mathf.PI * 2 / 9; // 9 armor parts
            float height = Mathf.Sin(angle) * 0.3f; // Reduced vertical variation
            return new Vector3(Mathf.Cos(angle) * 1.2f, height, Mathf.Sin(angle) * 1.2f); // Reduced from 2.0
        }

        return Vector3.zero;
    }

    public void ToggleExplode()
    {
        StopAllCoroutines();

        for (int i = 0; i < interactableObjects.Length; i++)
        {
            if (interactableObjects[i] != null)
            {
                Vector3 targetPos = isExploded ? originalPositions[i] : explodedPositions[i];
                StartCoroutine(MoveToPosition(interactableObjects[i], targetPos));
            }
        }

        isExploded = !isExploded;
    }

    IEnumerator MoveToPosition(Transform obj, Vector3 targetPosition)
    {
        float timeElapsed = 0;
        Vector3 startPosition = obj.localPosition;

        while (timeElapsed < 1)
        {
            obj.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
            timeElapsed += Time.deltaTime * moveSpeed;
            yield return null;
        }

        obj.localPosition = targetPosition;
    }

    void Update()
    {
        HandleTouchInput();
    }

    void HandleTouchInput()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame && selectedObject == null)
        {
            Ray ray = mainCamera.ScreenPointToRay(touch.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (IsInteractable(hit.transform))
                {
                    selectedObject = hit.transform;
                    offset = selectedObject.position - GetTouchWorldPosition();
                }
            }
        }

        if (touch.press.isPressed && selectedObject != null)
        {
            Vector3 touchPos = GetTouchWorldPosition() + offset;
            selectedObject.position = touchPos;
        }

        if (touch.press.wasReleasedThisFrame && selectedObject != null)
        {
            int index = System.Array.IndexOf(interactableObjects, selectedObject);
            if (index >= 0)
            {
                Vector3 targetPos = isExploded ? explodedPositions[index] : originalPositions[index];
                StartCoroutine(MoveToPosition(selectedObject, targetPos));
            }

            selectedObject = null;
        }
    }

    Vector3 GetTouchWorldPosition()
    {
        Vector3 touchPoint = Touchscreen.current.primaryTouch.position.ReadValue();
        touchPoint.z = 10f; // Adjust based on your scene
        return mainCamera.ScreenToWorldPoint(touchPoint);
    }

    bool IsInteractable(Transform obj)
    {
        return System.Array.IndexOf(interactableObjects, obj) >= 0;
    }
}