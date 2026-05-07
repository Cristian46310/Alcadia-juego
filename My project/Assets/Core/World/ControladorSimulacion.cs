using UnityEngine;

public class ControladorSimulacion : MonoBehaviour
{
    private float cronometro = 0f;
    private bool eventoFinalActivado = false;

    void Update()
    {
        cronometro += Time.deltaTime;

        // A los 100 segundos (1:40 min), empezamos a preparar el final
        if (cronometro >= 100f && !eventoFinalActivado)
        {
            PrepararChoqueFinal();
        }

        // A los 120 segundos, si no ha chocado, forzamos algo
        if (cronometro >= 120f)
        {
            Debug.Log("Fin del tiempo: Forzando evento de cierre.");
        }
    }

    void PrepararChoqueFinal()
    {
        eventoFinalActivado = true;
        // Aquí daremos la orden al Spawner de tráfico para que 
        // cree un camión que bloquee todos los carriles.
        Debug.Log("Apareciendo obstáculo inevitable...");
    }
}