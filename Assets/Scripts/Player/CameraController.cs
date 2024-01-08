using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class CameraController : MonoBehaviour
{
    [Header("Camera Instellingen")]
    [Tooltip("De snelheid waarmee de speler kan roteren")][Range(1, 500)]                             public float MouseSensitivity = 100f;
    [SerializeField][Tooltip("De transform van de speler zijn lichaam")]                              private Transform _playerBody;
    private float _xRotation = 0f;

    [Header("Object Interactie")]
    [Tooltip("De transform van de speler zijn lichaam")]                                              public float PickupDistance = 5f;
    private GameObject _currentObject = null;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        CameraRotate();
        ObjectInteraction();
    }

    /// <summary>
    /// Zorgt ervoor dat je je camera kan bewegen om zo rond te kunnen kijken als speler via mouse input
    /// en dan de input toepast op de camera en speler
    /// </summary>
    private void CameraRotate()
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
    
    /// <summary>
    /// Handelt de interactie met objecten af. Wanneer de speler op de 'E' toets drukt,
    /// voert deze methode een raycast uit vanuit de huidige positie van de camera in de voorwaartse richting.
    /// Als een object binnen het bereik van 'PickupDistance' wordt geraakt en het heeft de tag 'PickupObject',
    /// wordt dit object als het huidige interactie-object ingesteld en onzichtbaar gemaakt in de spelwereld.
    /// </summary>
    void ObjectInteraction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(_currentObject == null)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position, transform.forward, out hit, PickupDistance))
                {
                    if(hit.collider.gameObject.tag == "PickupObject")
                    {
                        _currentObject = hit.collider.gameObject;
                        _currentObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Om te checken of het juiste object is gestolen als je wilt winnen als check
    /// </summary>
    /// <param name="requiredObject"></param> het object dat je moet stelen
    /// <returns></returns>
    public bool HasRequiredObject(GameObject requiredObject)
    {
        return _currentObject == requiredObject;
    }

}
