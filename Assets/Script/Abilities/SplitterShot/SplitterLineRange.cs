using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterLineRange : MonoBehaviour
{
    public LineRenderer circleRenderer;
    private int loop = 4;
    private float size = 0f;

    private void Start() {
        StartCoroutine(Process(Time.deltaTime, loop * 10));
    }

    private void DrawCircle(int steps, float radius) {
        circleRenderer.positionCount = steps;
        float angle = 0f;

        for (int i = 0; i < steps; i++) {
            float xScaled = Mathf.Cos(angle);
            float yScaled = Mathf.Sin(angle);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0);
            circleRenderer.SetPosition(i, currentPosition);

            angle += 2f * Mathf.PI / steps;
            
        }
    }

    private IEnumerator Process(float deltaTime, int looped) {
        for (int i = 0; i < looped; i++) {
            DrawCircle(30, size); 
            size += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void IncreaseSize(int loop) {
        StartCoroutine(Process(Time.deltaTime, loop));
    }
}
