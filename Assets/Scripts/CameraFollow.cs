using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private Transform target;

    private void Update()
    {
        targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
