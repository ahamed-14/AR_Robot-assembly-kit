using UnityEngine;
using UnityEngine.UI;

public class RotationController : MonoBehaviour
{
    public GameObject PlanetObject;
    public Vector3 RotationVector;
    public float rotationSpeed = 10f;
    public Button toggleButton;

    private bool isRotating = false;

    private void Start()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleRotation);
        }
    }

    private void Update()
    {
        if (isRotating && PlanetObject != null)
        {
            PlanetObject.transform.Rotate(RotationVector * rotationSpeed * Time.deltaTime);
        }
    }

    public void ToggleRotation()
    {
        isRotating = !isRotating;

        // Optional: Update button text
        Text buttonText = toggleButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = isRotating ? "Stop Rotation" : "Start Rotation";
        }
    }
}