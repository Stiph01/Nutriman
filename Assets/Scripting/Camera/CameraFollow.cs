using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Configuración de Seguimiento")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Límites del Mapa (Límites Externos)")]
    public bool useBounds = false;
    public Vector2 minBounds; // Esquina inferior izquierda del mapa
    public Vector2 maxBounds; // Esquina superior derecha del mapa

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        if (useBounds && cam != null)
        {
            // Calcula la mitad del ancho y alto que abarca la cámara
            float vertExtent = cam.orthographicSize;
            float horizExtent = vertExtent * cam.aspect;

            // Ajusta el límite para que la cámara no muestre fuera del mapa
            float minX = minBounds.x + horizExtent;
            float maxX = maxBounds.x - horizExtent;
            float minY = minBounds.y + vertExtent;
            float maxY = maxBounds.y - vertExtent;

            // Restringe la posición del centro de la cámara
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}