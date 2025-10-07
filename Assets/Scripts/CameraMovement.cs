using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 10.0f; // Velocidad de movimiento
    public float rotationSpeed = 100.0f; // Velocidad de rotación
    public float zoomSpeed = 10.0f; // Velocidad de zoom

    private void Update()
    {
        // Movimiento con WASD o teclas de flechas (movimiento en X y Z)
        float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(horizontal, 0, vertical);

        // Movimiento de subir y bajar con Q y E (movimiento en Y)
        if (Input.GetKey(KeyCode.Q)) // Si se presiona la tecla Q
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime); // Baja la cámara
        }
        if (Input.GetKey(KeyCode.E)) // Si se presiona la tecla E
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime); // Sube la cámara
        }

            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * mouseX, Space.World);
            transform.Rotate(Vector3.left * mouseY, Space.Self);
        

        // Zoom con la rueda del ratón
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoom, 20, 100); // Ajuste del zoom
    }
}
