using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;   // Velocidad de movimiento
    public float liftSpeed = 5f;    // Velocidad de subida/bajada
    public float rotationSpeed = 100f;  // Velocidad de rotaci�n

    void Update()
    {
        // Movimiento horizontal con W, A, S, D
        float moveForwardBackward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveLeftRight = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        // Movimiento vertical con Q y E (subir/bajar)
        float moveUpDown = 0f;
        if (Input.GetKey(KeyCode.Q)) moveUpDown = -liftSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) moveUpDown = liftSpeed * Time.deltaTime;

        // Rotaci�n de la c�mara con el rat�n (si prefieres usar el rat�n para mover la c�mara)
        float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotationY = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Mover la c�mara
        Vector3 move = new Vector3(moveLeftRight, moveUpDown, moveForwardBackward);
        transform.Translate(move, Space.World);

        // Rotar la c�mara con el rat�n
        transform.Rotate(rotationY, rotationX, 0);
    }
}
