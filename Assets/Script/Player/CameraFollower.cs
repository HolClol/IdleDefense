using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;
    public Transform Target;

    private Vector3 velocity = Vector3.zero;

    private void Start() {
        Vector3 targetPos = Target.position + offset;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, damping);
    }
}
