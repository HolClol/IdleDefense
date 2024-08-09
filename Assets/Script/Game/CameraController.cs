using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera PlayerCamera;
    [SerializeField] float CameraDistance = 15;

    public void CameraRun(float[] value)
    {
        StartCoroutine(ChangeCameraDistance(value[0], value[1]));
    }
    public IEnumerator ChangeCameraDistance(float distance, float delay)
    {
        int loop;
        bool increase = false;
        yield return new WaitForSeconds(delay);

        if (distance > CameraDistance)
        {
            loop = (int)(distance - CameraDistance);
            increase = true;
        }

        else
            loop = (int)(CameraDistance - distance);
        for (int i = 0; i < loop * 10; i++)
        {
            if (increase)
                PlayerCamera.orthographicSize += 0.1f;
            else
                PlayerCamera.orthographicSize -= 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        CameraDistance = distance;
    }
}
