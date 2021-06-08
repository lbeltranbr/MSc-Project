using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_behaviour : MonoBehaviour
{
    public float speed = 10f;
    public float scrollSpeed = 10f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update()
    {
         
        if (Input.GetMouseButton(1))
        {
            yaw += speed * Input.GetAxis("Mouse X");
            pitch -= speed * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
        if (Input.GetMouseButton(2))
        {
            //transform.position += new Vector3(-Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, 0.0f);
            transform.position += transform.right * -Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed;
            transform.position += transform.up * -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed;
        }
        

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.position += scrollSpeed * Input.GetAxis("Mouse ScrollWheel")*transform.forward;
        }
    }
}
