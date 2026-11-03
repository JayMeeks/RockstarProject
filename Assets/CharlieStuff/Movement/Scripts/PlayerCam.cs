using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float sensX = 0.15f;
    public float sensY = 0.15f;

    [Header("References")]
    public Transform orientation;
    public Transform camHolder;

    private float xRotation;
    private float yRotation;

    private Camera cam;
    private PlayerControls controls;

    private Coroutine fovRoutine;
    private Coroutine tiltRoutine;

    private void Awake()
    {
        controls = new PlayerControls();
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensX;
        float mouseY = lookInput.y * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void ChangeFOV(float endValue)
    {
        if (fovRoutine != null)
            StopCoroutine(fovRoutine);

        fovRoutine = StartCoroutine(ChangeFOVRoutine(endValue));
    }

    private IEnumerator ChangeFOVRoutine(float targetFOV)
    {
        float startFOV = cam.fieldOfView;
        float timer = 0f;
        const float duration = 0.25f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, timer / duration);
            yield return null;
        }

        cam.fieldOfView = targetFOV;
    }

    public void Tilt(float zTilt)
    {
        if (tiltRoutine != null)
            StopCoroutine(tiltRoutine);

        tiltRoutine = StartCoroutine(TiltRoutine(zTilt));
    }

    private IEnumerator TiltRoutine(float targetTilt)
    {
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetTilt);

        float timer = 0f;
        const float duration = 0.25f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(
                startRotation, targetRotation, timer / duration
            );

            yield return null;
        }

        transform.localRotation = targetRotation;
    }
}
