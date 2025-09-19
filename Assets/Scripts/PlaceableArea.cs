using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaceableArea : MonoBehaviour {
    [System.Serializable]
    public class PlaceClickEvent : UnityEvent<PlaceableArea> { }
    public PlaceClickEvent clickEvent;

    private readonly float HIGHLIGHT_FADE_TIME = 0.15f;
    private float highlightTimer = 0f;

    private Renderer renderer;
    private static readonly Color TRANSPARENT_COLOR = new Color(0f, 0f, 0f, 0f);
    private Color originalColor;
    private bool highlighted = false;

    // Start is called before the first frame update
    void Start() {
        renderer = GetComponent<Renderer>();
        originalColor = renderer.material.color;
        // Set to be transparent.
        renderer.material.color = TRANSPARENT_COLOR;
    }

    void Update() {
        highlightTimer += Time.deltaTime;
        if (highlighted && highlightTimer > HIGHLIGHT_FADE_TIME) {
            highlighted = false;
            LeanTween.color(gameObject, TRANSPARENT_COLOR, HIGHLIGHT_FADE_TIME);
        }
    }

    public void Highlight() {
        highlightTimer = 0f;
        if (!highlighted) {
            highlighted = true;
            LeanTween.color(gameObject, originalColor, HIGHLIGHT_FADE_TIME);
        }
    }

    public void Click() {
        clickEvent.Invoke(this);
    }
}
