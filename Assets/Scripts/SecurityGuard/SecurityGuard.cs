using UnityEngine;
using UnityEngine.AI;

public class SecurityGuard : MonoBehaviour
{
    enum GuardState
    {
        Patrolling,
        Pausing,
        Chasing
    }
    
    [Header("Beweging Instellingen")]
    
    [Tooltip("Lijst van waypoints waar de bewaker naartoe beweegt")]                public Transform[] Waypoints;
    [Tooltip("Duur van de pauze op een waypoint")]                                  public float PauseDuration = 2f;
    
    [Header("Kijk Rond Instellingen")]
    [Tooltip("Hoek waarin de bewaker rondkijkt tijdens de pauze")]                  public float LookAroundAngle = 90f;
    [Tooltip("Snelheid waarmee de bewaker rondkijkt")]                              public float LookAroundSpeed = 120f;
    
    [Header("Detectie Instellingen")]
    [Tooltip("Het zichtbereik van de bewaker")]                                     public float ViewRange = 10f;
    [Tooltip("Het zichtbereik van de bewaker")]                                     public float ViewAngle = 45f;
    [Tooltip("De hoofd van de bewaker die rond kijkt")]                             public Transform LookTransform;
    [Tooltip("De speler die de bewaker moet detecteren en achtervolgen")]           public Transform Player;
    
    [Header("Spotlight Instellingen")]
    [Tooltip("De zaklamp die de bewakers hebben")]                                  public Light GuardSpotlight;
    
    private NavMeshAgent _agent;
    private int _waypointIndex;
    private float _pauseTimer;
    private GuardState _currentState;
    private float _currentLookAroundAngle;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        SetState(GuardState.Patrolling);
    }

    void Update()
    {
        if (_currentState != GuardState.Chasing && CanSeePlayer())
        {
            SetState(GuardState.Chasing);
            return;
        }
        
        switch (_currentState)
        {
            case GuardState.Patrolling:
                Patrol();
                break;
            case GuardState.Pausing:
                PauseAndLookAround();
                break;
            case GuardState.Chasing:
                ChasePlayer();
                break;
        }
        
        // Pas de richting van de spotlight aan om overeen te komen met de kijkrichting van de bewaker
        GuardSpotlight.transform.forward = transform.forward;
        
        GuardSpotlight.spotAngle = ViewAngle;
        GuardSpotlight.range = ViewRange;
        GuardSpotlight.intensity = CalculateIntensity(ViewRange, ViewAngle);
    }
    
    /// <summary>
    /// Bereken de lichtintensiteit van de spotlight op basis van het zichtbereik en de kijkhoek van de bewaker.
    /// Deze methode neemt het bereik en de hoek als parameters en gebruikt een formule waarbij de intensiteit 
    /// afneemt naarmate de range en angle toenemen. De berekende intensiteit wordt beperkt tot een bereik van 0 tot 100.
    /// </summary>
    /// <param name="_Range">Het zichtbereik van de spotlight.</param>
    /// <param name="_Angle">De kijkhoek van de spotlight.</param>
    /// <returns>De berekende lichtintensiteit, beperkt tussen 0 en 500.</returns>
    float CalculateIntensity(float _Range, float _Angle)
    {
        // Aangepaste berekeningsformule voor hogere intensiteit
        float _Intensity = 20000 / (_Range * _Angle); // Aanpassen van de constante waarde

        // Verhoog de maximumgrens indien nodig
        return Mathf.Clamp(_Intensity, 0, 500); // Aanpassen van de maximumgrens
    }
    
    /// <summary>
    /// Handelt de patrouille logica van de bewaker. Beweegt naar het volgende waypoint en controleert of de speler zichtbaar is.
    /// </summary>
    void Patrol()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            if (CanSeePlayer())
                SetState(GuardState.Chasing);
            
            else if (_pauseTimer < PauseDuration)
                _pauseTimer += Time.deltaTime;
            
            else
                SetState(GuardState.Pausing);
        }
    }

    /// <summary>
    /// Stelt de huidige staat van de bewaker in en initialiseert elke noodzakelijke logica voor die staat.
    /// </summary>
    /// <param name="_NewState">De nieuwe staat om in te stellen.</param>
    void SetState(GuardState _NewState)
    {
        if (_currentState == _NewState) return; // Voorkomt onnodige staatsherhaling
        
        _currentState = _NewState;
        switch (_NewState)
        {
            case GuardState.Patrolling:
                _pauseTimer = 0f;
                MoveToNextWaypoint();
                break;
            case GuardState.Pausing:
                _currentLookAroundAngle = 0f;
                break;
            case GuardState.Chasing:
                _agent.speed *= 1.5f;
                break;
        }
    }

    /// <summary>
    /// Beweegt de bewaker naar een willekeurig waypoint in de lijst.
    /// </summary>
    void MoveToNextWaypoint()
    {
        if (Waypoints.Length == 0)
            return;

        // Kies een willekeurig waypoint dat verschilt van het huidige waypoint
        int _NewWaypointIndex;
        
        do
        {
            _NewWaypointIndex = Random.Range(0, Waypoints.Length);
        } while (_NewWaypointIndex == _waypointIndex);
        
        _waypointIndex = _NewWaypointIndex;
        _agent.destination = Waypoints[_waypointIndex].position;
        
        if (_currentState != GuardState.Patrolling)
            SetState(GuardState.Patrolling);
    }

    /// <summary>
    /// Laat de bewaker pauzeren en rondkijken op zijn huidige locatie.
    /// </summary>
    void PauseAndLookAround()
    {
        if (_currentLookAroundAngle < LookAroundAngle)
        {
            float _RotationAmount = LookAroundSpeed * Time.deltaTime;
            Vector3 _EulerAngles = LookTransform.eulerAngles;
            
            LookTransform.rotation = Quaternion.Euler(_EulerAngles.x, _EulerAngles.y + _RotationAmount, _EulerAngles.z);
            _currentLookAroundAngle += _RotationAmount;
        }
        else
        {
            _currentLookAroundAngle = 0f;
            LookTransform.rotation = transform.rotation;
            SetState(GuardState.Patrolling);
        }
    }

    /// <summary>
    /// Controleert of de speler zichtbaar is voor de bewaker binnen een bepaald gezichtsveld en bereik.
    /// </summary>
    /// <returns>True als de speler zichtbaar is, anders false.</returns>
    bool CanSeePlayer()
    {
        Vector3 _DirectionToPlayer = Player.position - transform.position;
        float _Angle = Vector3.Angle(transform.forward, _DirectionToPlayer);
        if (_DirectionToPlayer.magnitude < ViewRange && _Angle < ViewAngle)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, _DirectionToPlayer.normalized, out hit, ViewRange))
                if (hit.transform == Player)
                    return true;
        }
        return false;
    }

    /// <summary>
    /// Zet de bewaker in achtervolgingsmodus en stelt de bestemming in op de huidige positie van de speler.
    /// </summary>
    void ChasePlayer()
    {
        _agent.destination = Player.position;
    }
}
