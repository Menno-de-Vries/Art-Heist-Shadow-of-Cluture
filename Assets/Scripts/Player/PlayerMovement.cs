using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speler Instellingen")]
    [Tooltip("De snelheid waarmee de speler zich voort beweegt")]                        public float MoveSpeed = 5f;
    [Tooltip("De variabel die de snelheid van lopen vermeervoudigt om te rennen")]       public float RunningMultiplier = 2;
    [Tooltip("De hoogte van de sprong")]                                                 public float JumpHeight = 2f;
    [Tooltip("De sterkte van de zwaartekracht")]                                         public float Gravity = -9.81f;
    [Tooltip("De afstand van raycast om te checken of speler op de grond staat")]        public float GroundCheckDistance = 1.1f;
    
    private const float _jumpVelocityMultiplier = -1.75f;
    private Vector3 _moveInput;
    private Vector3 _moveVelocity;
    private Vector3 _velocity;
    private Rigidbody _rb;
    private bool _IsGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovementCalculation();
        JumpAndGravity();
    }

    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveVelocity * Time.fixedDeltaTime);
        _rb.velocity = new Vector3(_rb.velocity.x, _velocity.y, _rb.velocity.z);
    }
    
    /// <summary>
    /// Dit berekent de beweging van de speler gebaseerd op de input van de speler. Deze functie verwerkt de horizontale
    /// en verticale input via de WASD of pijltjestoetsen en zet deze om in een bewegingsvector.
    /// Als de speler de shift-toets ingedrukt houdt, wordt de bewegingssnelheid verhoogd.
    /// </summary>
    private void MovementCalculation()
    {
        _moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        Vector3 _finaleMoveInput = transform.right * _moveInput.x + transform.forward * _moveInput.z;

        if (Input.GetKey(KeyCode.LeftShift))
            _moveVelocity = _finaleMoveInput.normalized * (MoveSpeed * RunningMultiplier);
        else
            _moveVelocity = _finaleMoveInput.normalized * MoveSpeed;
    }

    /// <summary>
    /// Dit maakt het springen en toepassing van zwaartekracht op de speler mogelijk. Deze functie maakt gebruik van een raycast
    /// om te bepalen of de speler op de grond staat. Als de speler op de grond staat en de spatiebalk wordt ingedrukt,
    /// wordt een springkracht toegepast. Onafhankelijk hiervan wordt een constante zwaartekracht
    /// toegepast op de speler om deze naar beneden te trekken.
    /// </summary>
    private void JumpAndGravity()
    {
        _IsGrounded = Physics.Raycast(transform.position, Vector3.down, GroundCheckDistance);

        if (_IsGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        if (Input.GetKey(KeyCode.Space) && _IsGrounded)
            _velocity.y = Mathf.Sqrt(JumpHeight * _jumpVelocityMultiplier * Gravity);

        _velocity.y += Gravity * Time.deltaTime;
    }
}
