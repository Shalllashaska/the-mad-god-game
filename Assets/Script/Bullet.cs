using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public float timeDestroy = 3f;
    public float speed = 3f;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timeDestroy);
        //Invoke("DestroyBullet", timeDestroy);
        rb.velocity = transform.forward * speed;
    }

    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}
