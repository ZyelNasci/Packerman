using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D body;
    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }    

    public void Shot(float force, Vector3 direction)
    {
        body.AddTorque(force);
        body.AddForce(direction * force, ForceMode2D.Impulse);
    }
}
