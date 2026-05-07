using UnityEngine;
using UnityEngine.EventSystems; // Necesario para detectar toques

public class PedalInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool isPressed = false;

    // Se ejecuta cuando presionas el botón
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    // Se ejecuta cuando sueltas el botón
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}