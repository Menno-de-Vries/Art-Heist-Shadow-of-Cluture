using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class CameraController : MonoBehaviour
{
    [Header("Camera Instellingen")]
    [Tooltip("De snelheid waarmee de speler kan roteren")][Range(1, 500)]                            public float MouseSensitivity = 100f;
    [SerializeField][Tooltip("De transform van de speler zijn lichaam")]                              private Transform _playerBody;
    private float _xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Zorgt ervoor dat je je camera kan bewegen om zo rond te kunnen kijken als speler via mouse input
    /// en dan de input toepast op de camera en speler
    /// </summary>
    void Update()
    {
        float[] _MouseAxis = new float[2];
        float _MouseSensCalculation = MouseSensitivity * Time.deltaTime;

        _MouseAxis[0] = Input.GetAxis("Mouse X") * _MouseSensCalculation;
        _MouseAxis[1] = Input.GetAxis("Mouse Y") * _MouseSensCalculation;

        _xRotation -= _MouseAxis[1];
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _playerBody.Rotate(Vector3.up * _MouseAxis[0]);
    }
}
