using UnityEngine;
using UnityEngine.Serialization;

public class FloatEffect : MonoBehaviour
{
    [Header("Float effect instellingen")]
    [Tooltip("Amplitude van de zweefbeweging")]            public float Amplitude = 0.5f;
    [Tooltip("Snelheid van de zweefbeweging")]             public float Frequency = 1f;

    private Vector3 _startPosition;
    private float _tempVal;

    void Start()
    {
        _startPosition = transform.position;
    }

    /// <summary>
    /// Update is verantwoordelijk voor het creëren van het zwevende effect.
    /// Het gebruikt een sinusfunctie om een soepele op-en-neer beweging te genereren. De verticale positie
    /// van het object wordt aangepast op basis van de huidige tijd, amplitude en frequentie, wat resulteert in
    /// een zwevende beweging rond de oorspronkelijke startpositie.
    /// </summary>
    void Update()
    {
        _tempVal = _startPosition.y + Amplitude * Mathf.Sin(Time.time * Frequency);
        transform.position = new Vector3(_startPosition.x, _tempVal, _startPosition.z);
    }

}
