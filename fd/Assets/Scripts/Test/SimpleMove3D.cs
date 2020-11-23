using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove3D : MonoBehaviour
{
    // Start is called before the first frame update
    float speed = 10f;
    Rigidbody myRigidbody;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        Vector3 position = myRigidbody.position;
        position.x += hori * Time.deltaTime * speed;
        position.z += vert * Time.deltaTime * speed;
        myRigidbody.MovePosition(position);
    }
}
