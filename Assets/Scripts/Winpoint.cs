using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// De WinPoint klasse beheert de wincondities voor een level of spel.
/// Wanneer een speler, met het correcte object, het winpunt bereikt, wordt de winconditie geactiveerd.
/// Deze klasse moet worden gekoppeld aan een gameobject met een trigger collider.
/// </summary>
public class Winpoint : MonoBehaviour
{
    [Header("De win instellingen")]
    [Tooltip("Het object dat de speler moet hebben om te winnen")]             public GameObject RequiredObject;
    [Tooltip("De naam van de win scene")]                                      public string WinScene = "Win";

    private void OnCollisionEnter(Collision _Other)
    {
        if (_Other.gameObject.tag == "Player") // Check of de collider behoort tot de speler
        {
            CameraController _CameraController = _Other.gameObject.GetComponentInChildren<CameraController>();
            if (_CameraController != null && _CameraController.HasRequiredObject(RequiredObject))
            {
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(WinScene);
            }
        }
    }
}
