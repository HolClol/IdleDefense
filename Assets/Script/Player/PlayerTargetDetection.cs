using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetDetection : MonoBehaviour
{
    public GameObject PlayerChar;
    public PlayerController playerController;
    public LineRenderer circleRenderer;
    public int loop = 8;

    private void Start() {
        StartCoroutine(Process(Time.deltaTime, loop * 4));
        transform.position = PlayerChar.transform.position;
    }

    // private void Update() {
    //     transform.position = PlayerChar.transform.position;
    // }

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
        float size = 0f;
        for (int i = 0; i < looped; i++) {
            DrawCircle(50, size); 
            size += 0.25f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            playerController.EnemyInZone.RemoveAll(GameObject => GameObject == null);
            playerController.EnemyInZone.Add(trigger.gameObject);
        }
                
    }

    private void OnTriggerExit2D(Collider2D trigger) {
        if (trigger.gameObject.CompareTag("Enemy")) {
            playerController.EnemyInZone.RemoveAll(GameObject => GameObject == null);
            playerController.EnemyInZone.Remove(trigger.gameObject);
        }
                
    }
}
