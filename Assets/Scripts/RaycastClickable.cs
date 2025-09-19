using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastClickable : MonoBehaviour {
    [System.Serializable]
    public class RaycastClickEvent : UnityEvent<RaycastClickable> { }
    public RaycastClickEvent clickEvent;

    public bool maintainMaterialColor = false;
    public bool requiresItem = false;

    private readonly float HIGHLIGHT_FADE_TIME = 0.15f;
    private float highlightTimer = 0f;

    private Renderer renderer;
    private Color originalColor;
    private Color targetColor;
    private bool highlighted = false;

    // Start is called before the first frame update
    void Awake() {
        renderer = GetComponentInChildren<Renderer>();
        SetColor(renderer.material.color);
    }

    void Update() {
        highlightTimer += Time.deltaTime;
        if (highlighted && highlightTimer > HIGHLIGHT_FADE_TIME) {
            highlighted = false;
            renderer.material.color = originalColor;
            // LeanTween.color(gameObject, originalColor, HIGHLIGHT_FADE_TIME);
        }
    }

    public void SetColor(Color color) {
        originalColor = color;
        targetColor = maintainMaterialColor ?
            new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f) :
            Color.green;

        if (highlighted) {
            renderer.material.color = targetColor;
        } else {
            renderer.material.color = originalColor;
        }
    }

    public bool RequiresItem() {
        return requiresItem;
    }

    public void Highlight() {
        highlightTimer = 0f;
        if (!highlighted) {
            highlighted = true;
            renderer.material.color = targetColor;
            // LeanTween.color(gameObject, targetColor, HIGHLIGHT_FADE_TIME);
        }
    }

    public void Click() {
        clickEvent.Invoke(this);
    }
}
