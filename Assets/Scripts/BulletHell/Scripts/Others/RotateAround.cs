using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour 
{
    public Transform center;
    public Vector3 axis = Vector3.forward;
    public float radius = 1.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 80.0f;

    Vector3 desiredPosition;

    void Start () 
    {
        transform.position = (transform.position - center.position).normalized * radius + center.position;
    }

    void Update () 
    {
        transform.RotateAround (center.position, axis, rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - center.position).normalized * radius + center.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
