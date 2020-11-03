using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotSquare : MonoBehaviour
{
    public Projectile bulletPrefab;
    public float forceMax;
    public float forceMin;
    public float delay;
    public bool enabled;

    private float currentTime;    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            ShotMethod();
        }        
    }

    public void ShotMethod()
    {
        if(Time.timeSinceLevelLoad >= currentTime + delay)
        {
            transform.localEulerAngles = Vector3.forward * Random.Range(-50f, -20f);
            Projectile newShot = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            newShot.Shot(Random.Range(forceMin, forceMax), -transform.right);
            currentTime = Time.timeSinceLevelLoad;
        }
    }

    


}
