using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    public Transform target; // The target the camera is following
    public float smoothSpeed = 0.125f; // The speed at which the camera follows the target
    public Vector3 offset; // The offset of the camera from the target

    [Header("Camera Shake Settings")]
    private Vector3 initialPosition; // Initial position of the camera for resetting after shake
    private bool isShaking = false; // Flag to check if camera shake is active
    public float shakeIntensity = 0.1f; // Initial intensity of the camera shake
    public float shakeDuration = 0.5f; // Duration of the camera shake
    public float fadeOutTime = 0.2f; // Time it takes for the shake to fade out
    public float shakeIntensityFactor = 1;

    [Header("Camera Zoom Settings")]
    public float zoomSpeed = 3f; // Speed of camera zoom
    public float minZoom = 0.5f; // Minimum zoom level
    public float maxZoom = 2f; // Maximum zoom level

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Camera follow
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Camera shake
        if (isShaking)
        {
            transform.position = initialPosition + Random.insideUnitSphere * shakeIntensity;
        }

        // Camera zoom
        float scrollWheel = -Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            float newZoom = Mathf.Clamp(transform.localScale.x + (scrollWheel * zoomSpeed), minZoom, maxZoom);
            transform.localScale = new Vector3(newZoom, newZoom, newZoom);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            ShakeCamera();
    }

    // Trigger camera shake
    public void ShakeCamera()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }


    // Trigger camera shake
    public void SoftShake(float length)
    {
        shakeDuration = length;
        shakeIntensity = 0.5f;
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    // Trigger camera shake
    public void HeavyShake(float length)
    {
        shakeDuration = length;
        shakeIntensity = 3f;
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }

    // Coroutine for camera shake
    private IEnumerator Shake()
    {
        isShaking = true;
        float shakeTimer = shakeDuration;

        while (shakeTimer > 0f)
        {
            transform.position = initialPosition + Random.insideUnitSphere * shakeIntensity * shakeIntensityFactor;

            // Gradually decrease shake intensity over time
            shakeIntensity = Mathf.Lerp(0, shakeIntensity, 1 - (shakeTimer / shakeDuration));

            shakeTimer -= Time.deltaTime;
            yield return null;
        }

        // Reset values after shake
        isShaking = false;
        shakeIntensity = 0.1f; // Reset shake intensity
        transform.position = initialPosition; // Reset to initial position after shake
    }
}
