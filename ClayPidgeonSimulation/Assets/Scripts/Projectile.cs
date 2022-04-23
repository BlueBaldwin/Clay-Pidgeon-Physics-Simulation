using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vec3 v3CurrentVelocity;             //Launch Velocity as Vector
    private Vec3 v3Acceleration;                //Vector quantity for acceleration.
    private float fLifeSpan = 0f;               //Lifespan of the gameobject

    public Vec3 Velocity
    {
        get { return v3CurrentVelocity; }
        set { v3CurrentVelocity = value;}
    }
    
    public Vec3 Acceleration
    {
        set{ v3Acceleration = value;}
        get{ return v3Acceleration;}
    }
    
    public float LifeSpan
    {
        set{ fLifeSpan = value;}
        get{ return fLifeSpan; }
    }

    public Vec3 Position
    {
        set{ transform.position = value.ToVector3(); }
        get{return new Vec3(transform.position);}
    } 

    private void FixedUpdate() 
    {
        fLifeSpan -= Time.deltaTime;

        Vec3 currentPos = new Vec3( transform.position);
        //work out current velocity
        v3CurrentVelocity += v3Acceleration * Time.deltaTime;
        //work out displacement
        Vec3 displacement = v3CurrentVelocity * Time.deltaTime;
        currentPos += displacement;
        transform.position = currentPos.ToVector3();

        if( fLifeSpan < 0f )
        {
            Destroy(gameObject);
        }
    }
}
