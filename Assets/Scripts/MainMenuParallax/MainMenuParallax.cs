using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuParallax : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public Transform layer;   // La capa
        public float intensity;   // Cuánto se mueve
        [HideInInspector] public Vector3 startPos; // Posición original
    }

    public ParallaxLayer[] layers;
    public float smoothSpeed = 5f;

    void Start()
    {
        // Guardamos la posición inicial de cada capa
        foreach (var l in layers)
        {
            if (l.layer != null)
                l.startPos = l.layer.localPosition;
        }
    }

    void Update()
    {
        // Normalizamos mouse a -1..1
        Vector2 mouse = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 normalized = (mouse - screenCenter) / screenCenter;
        normalized = Vector2.ClampMagnitude(normalized, 1f);

        foreach (var l in layers)
        {
            if (l.layer == null) continue;

            // Offset respecto a la posición inicial
            Vector3 offset = new Vector3(normalized.x * l.intensity, normalized.y * l.intensity, 0);

            // Nueva posición = posición inicial + offset
            Vector3 targetPos = l.startPos + offset;

            // Suavizamos
            l.layer.localPosition = Vector3.Lerp(l.layer.localPosition, targetPos, Time.deltaTime * smoothSpeed);
        }
    }
}
