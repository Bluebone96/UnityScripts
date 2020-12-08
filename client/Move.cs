using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    
    public byte[] op { get; set; }
    public float m_speed = 10f;

    public float moveSpeed = 5f;
    public float rotateSpeed = 30f;
    Vector3 move;
    void Awake() {
        Time.fixedDeltaTime = 1f/60f;
        Debug.Log(Time.fixedDeltaTime);
        op = new byte[10];
    }

    void  FixedUpdate () {

        // move.x = m_speed * (op[0] - op[1]);

        // move.z = m_speed * (op[2] - op[3]);

        // transform.Translate(move * Time.fixedDeltaTime, Space.World);


        // Update is called once per frame

 

        // float h = Input.GetAxis ("Horizontal");

        // float v = Input.GetAxis ("Vertical");

        // transform.Translate (new Vector3(v * Time.deltaTime * moveSpeed, 0, 0));

        // transform.Rotate (new Vector3(0, h * Time.deltaTime * rotateSpeed, 0));

    }


    public void SetOp(byte[] b, int n) {
        for (int i = 0; i < n; ++i) {
            op[i] = b[i];
        }
    }

}
