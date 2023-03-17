using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VariableMovement : MonoBehaviour
{
    // Enum to select the axis to move on
    public enum Axis { X, Y, Z }
    public Axis selectedAxis;

    // Variables to control the period and amplitude of the sine wave
    [FormerlySerializedAs("period")] public float speed = 1f;
    public float amplitude = 1f;

    public bool RandomMovementOffset;
    private float offset = 0f;
    
    // Boolean to control whether position wobbling is enabled
    public bool positionWobble = false;

    // Variables to control the speed and scale of the position wobbling
    public float wobbleSpeed = 1f;
    public float wobbleScale = 1f;

    // Store the initial position for local movement
    [SerializeField]private Vector3 initialPosition;
    private Vector3 wobbleOffset;
    // Start is called before the first frame update
    void Awake()
    {
        // If local movement is enabled, store the initial position for reference
        initialPosition = transform.localPosition;
        offset = Random.Range(1f, 1.15f);
    }

    // Update is called once per frame
    void Update()
    {
        // if (transform.parent != null) transform.rotation = Quaternion.Euler(Vector3.zero);
        CalculateWave();
        // Apply position wobbling using Perlin noise if the boolean is enabled
        if (positionWobble)Wobble();
    }

    private void Wobble()
    {
        // Calculate the Perlin noise value based on time and speed
        float perlinValue = Mathf.PerlinNoise(Time.time * offset * wobbleSpeed, 0f);

        // Multiply the Perlin noise value by the wobble scale to get the actual wobble amount
        float wobbleAmount = perlinValue * wobbleScale;

        // Create a wobble offset vector using Perlin noise in all three axes
        wobbleOffset = new Vector3(
            Mathf.PerlinNoise(Time.time * wobbleSpeed, 0f) * wobbleAmount,
            Mathf.PerlinNoise(0f, Time.time * wobbleSpeed) * wobbleAmount,
            Mathf.PerlinNoise(Time.time * wobbleSpeed, Time.time * wobbleSpeed) * wobbleAmount);

        // Apply the wobble offset to the object's position
        transform.position += wobbleOffset;
    }

    public void CalculateWave()
    {
        // Calculate the sine wave value based on time and period
        float sineValue = Mathf.Sin(Time.time * offset * speed);

        // Multiply the sine value by the amplitude to get the actual movement amount
        float movementAmount = sineValue * amplitude;

        // Create a movement vector based on the selected axis and movement amount
        Vector3 movementVector = Vector3.zero;

        switch (selectedAxis)
        {
            case Axis.X:
                movementVector = new Vector3(movementAmount, 0f, 0f);
                break;
            case Axis.Y:
                movementVector = new Vector3(0f, movementAmount, 0f);
                break;
            case Axis.Z:
                movementVector = new Vector3(0f, 0f, movementAmount);
                break;
        }

        // If local movement is enabled, use the initial position as the reference point
        if (transform.parent != null)
        {
            transform.position = transform.parent.position + movementVector;
        }
        else
            transform.position = initialPosition + movementVector;
    }
}
