using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageController : MonoBehaviour
{
    public TrailRenderer trail;
    private Collider2D myCollider;
    private Rigidbody2D body;
    bool picked;

    #region Unity Functions
    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if(picked == false)
        {
            if (body.velocity.x > 1 || body.velocity.x < -1 || body.velocity.y > 1 || body.velocity.y < -1)
            {
                trail.emitting = true;
            }
            else
            {
                trail.emitting = false;
            }
        }
        else
        {
            trail.emitting = false;
        }
        
    }
    #endregion

    #region Methods
    public void Caught()
    {
        picked = true;
        body.velocity = Vector2.zero;
        myCollider.enabled = false;
        body.isKinematic = true;
    }

    public void Shot(Vector2 direction, float force)
    {
        picked = false;
        transform.parent    = null;        
        body.isKinematic    = false;
        //print("Forca: " + force);
        body.AddForce(direction * force, ForceMode2D.Impulse);
        myCollider.enabled = true;
    }
    public void Free()
    {
    }
    #endregion
}
