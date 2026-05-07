using UnityEngine;

public class EfectoVelocidad : MonoBehaviour
{
    public Rigidbody rbMoto;
    private Camera cam;

    [Header("Ajustes de FOV")]
    public float fovMinimo = 60f;
    public float fovMaximo = 90f;
    public float velocidadReferencia = 150f; // Velocidad a la que el FOV llega al máximo

    [Header("Vibración (Viento)")]
    public float intensidadVibracion = 0.05f;
    private Vector3 posicionOriginal;

    void Start()
    {
        cam = GetComponent<Camera>();
        posicionOriginal = transform.localPosition;
    }

    void Update()
    {
        // Obtenemos la velocidad actual en KM/H
        float velocidadKMH = rbMoto.linearVelocity.magnitude * 3.6f;

        // 1. Efecto de Estiramiento (FOV)
        // Calculamos cuánto FOV sumar basado en la velocidad
        float ratioVelocidad = velocidadKMH / velocidadReferencia;
        cam.fieldOfView = Mathf.Lerp(fovMinimo, fovMaximo, ratioVelocidad);

        // 2. Efecto de Vibración por Viento
        if (velocidadKMH > 40f) // Solo vibra si vamos a más de 40 km/h
        {
            float fuerzaSacudida = (velocidadKMH / velocidadReferencia) * intensidadVibracion;
            transform.localPosition = posicionOriginal + Random.insideUnitSphere * fuerzaSacudida;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, posicionOriginal, Time.deltaTime * 5f);
        }
    }
}