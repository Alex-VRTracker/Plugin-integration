using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour {

    public float startSpeed = 0.1f;
    public float acceleration = 0.005f;

    private float speed;
    private float middlePos;


    // Use this for initialization
    void Start()
    {
        speed = startSpeed;
        middlePos = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (this.transform.position.y >= middlePos)
        {
            speed -= acceleration;
        }
        else
        {
            speed += acceleration;
        }
        Vector3 direction = new Vector3(0f, speed, 0f);
        this.transform.Translate(direction);
    }
}
