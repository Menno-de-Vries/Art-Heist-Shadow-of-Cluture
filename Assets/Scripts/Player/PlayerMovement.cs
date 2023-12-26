using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speler Instellingen")]
    public float MoveSpeed = 5f;
    
    private Vector3 _moveInput;
    private Vector3 _moveVelocity;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Zorgt voor een calculatie die de input van de wasd toetsen pakt
    /// waarbij deze in een berekening worden gegooid om de beweging relatief te maken aan hoe je kijkt met je camera
    /// </summary>
    void Update()
    {
        _moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        
        Vector3 _finaleMoveInput = transform.right * _moveInput.x + transform.forward * _moveInput.z;
        _moveVelocity = _finaleMoveInput.normalized * MoveSpeed;
    }

    /// <summary>
    /// Hierbij wordt de calculatie toegepast op de speler met de toebehorende snelheid en vermenigvuldigt bij tijd
    /// </summary>
    void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _moveVelocity * Time.fixedDeltaTime);
    }
}
