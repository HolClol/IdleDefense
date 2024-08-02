using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;

    void Start() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            if (m_SpriteRenderer != null) {
                m_SpriteRenderer.color = Color.red;
            }
        }
    }

    void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            if (m_SpriteRenderer != null) {
                m_SpriteRenderer.color = Color.white;
            }
        }
    }
}
