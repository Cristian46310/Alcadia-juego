using UnityEngine;
using TMPro;

public class MotoController : MonoBehaviour
{
    [Header("Conexión con Pedales")]
    public PedalInput pedalAcelerador; 
    public PedalInput pedalFreno;      

    [Header("Ajustes de la Moto")]
    public float fuerzaMotor = 800000f;
    public float fuerzaFrenado = 8f;        // Reducido: frenado menos brusco
    public float velocidadMaxima = 150f; 
    public float resistenciaAire = 0.015f;  // NUEVO: resistencia aerodinámica progresiva
    public float rozamientoRueda = 0.03f;   // NUEVO: rozamiento natural muy bajo

    [Header("Joystick")]
    public FixedJoystick joystick;    
    public float sensibilidadGiro = 100f; 
    public float inclinacionMaxima = 25f; 

    [Header("UI de Velocidad")]
    public TextMeshProUGUI textoVelocidad; 
    
    private Rigidbody rb;
    private float velocidadActualKMH;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = 0f; // CERO damping — la física la controlamos nosotros
    }

    void Update()
    {
        ActualizarUI();
    }

    void FixedUpdate()
    {
        ManejarAceleracion();
        ManejarGiro();
    }

    void ManejarAceleracion()
    {
        velocidadActualKMH = rb.linearVelocity.magnitude * 3.6f;

        // --- 1. ACELERACIÓN ---
        if (pedalAcelerador != null && pedalAcelerador.isPressed)
        {
            if (velocidadActualKMH < velocidadMaxima) 
            {
                float factorPotencia = 1f - (velocidadActualKMH / velocidadMaxima);
                rb.AddForce(transform.forward * fuerzaMotor * factorPotencia * Time.fixedDeltaTime, ForceMode.Force);
            }
        }

        // --- 2. FRENADO ACTIVO ---
        if (pedalFreno != null && pedalFreno.isPressed)
        {
            // Frenado progresivo: más suave al inicio, más fuerte si sigues frenando
            rb.linearVelocity = Vector3.MoveTowards(
                rb.linearVelocity, 
                Vector3.zero, 
                fuerzaFrenado * Time.fixedDeltaTime
            );
        }
        
        // --- 3. INERCIA NATURAL (rueda libre) ---
        else
        {
            AplicarResistenciaFisica();
        }
    }

    void AplicarResistenciaFisica()
    {
        if (rb.linearVelocity.magnitude < 0.01f) return; // Evita cálculos innecesarios

        // Resistencia aerodinámica: crece con el CUADRADO de la velocidad (física real)
        // A baja velocidad casi no frena, a alta velocidad frena más notablemente
        float velocidadMS = rb.linearVelocity.magnitude;
        float fuerzaAire = resistenciaAire * velocidadMS * velocidadMS;

        // Rozamiento de rueda: constante y muy pequeño (simula asfalto liso)
        float fuerzaRozamiento = rozamientoRueda;

        // Fuerza total de resistencia opuesta al movimiento
        Vector3 direccionOpuesta = -rb.linearVelocity.normalized;
        rb.AddForce(direccionOpuesta * (fuerzaAire + fuerzaRozamiento), ForceMode.Force);
    }

    void ManejarGiro()
    {
        float direccion = joystick.Horizontal; 

        if (rb.linearVelocity.magnitude > 0.5f)
        {
            // La sensibilidad del giro baja a alta velocidad (más realista)
            float factorVelocidad = Mathf.Clamp01(1f - (velocidadActualKMH / (velocidadMaxima * 1.5f)));
            float sensibilidadReal = Mathf.Lerp(sensibilidadGiro * 0.5f, sensibilidadGiro, factorVelocidad);

            float rotacionY = direccion * sensibilidadReal * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, rotacionY, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);

            // Inclinación visual suavizada
            float inclinacionZ = -direccion * inclinacionMaxima;
            Quaternion targetRot = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, inclinacionZ);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.fixedDeltaTime * 10f);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }
    }

    void ActualizarUI()
    {
        if (textoVelocidad != null)
        {
            float kmh = rb.linearVelocity.magnitude * 3.6f;
            textoVelocidad.text = kmh.ToString("F0") + " KM/H";
            textoVelocidad.color = kmh > 100 ? Color.red : Color.white;
        }
    }
}