using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptController2 : MonoBehaviour
{
    public float fVisionRadius = 10f;
    private List<Projectile> projectiles;

    public ProjectileLauncher launcher;

    private float coolDownTime = 1.5f;
    private float coolDownCounter = 0f;
    // Start is called before the first frame update
    void Start()
    {
        projectiles = new List<Projectile>();
    }

    // Update is called once per frame
    void Update()
    {
        coolDownCounter -= Time.deltaTime * 0.1f;
        
        projectiles.Clear();
        //Use Unity's FindObjectsOfTag function to find all the projectiles within the scene
        GameObject[] allActiveProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
        Vec3 position = new Vec3(transform.position);
        float radSqrd = fVisionRadius * fVisionRadius;
        foreach (GameObject projectile in allActiveProjectiles)
        {
            //Get the distance to this projectile
            Vec3 vecToProjectile = new Vec3(projectile.transform.position) - position;
            float distanceToProjectile = vecToProjectile.MagnitudeSquared(); //use Mag squared to avoid sqrt calc. 
            if (distanceToProjectile < radSqrd)
            {
                Projectile p = projectile.GetComponent<Projectile>();
                projectiles.Add(p);
            }
        }

        //Now that we have all projectiles in the detection range
        //calculate the time to intercept for all of the projectiles in range
        //store these in a tuple with the index of the projectile in the projectiles array and time to intercept
        List<(int id, float time)> interceptTimes = new List<(int id, float time)>();
        
        for( int i = 0; i < projectiles.Count; ++i )
        {
            interceptTimes.Add((i, findTimeToIntercept(launcher.launchVelocity * Mathf.Cos(launcher.launchAngle * Mathf.Deg2Rad), 
                projectiles[i].Position, projectiles[i].Velocity)));
        }
        //get the index in the tuple with the shortest time to intercept
        int index = -1;
        float fiTime = float.MaxValue;
        foreach( var intercep in interceptTimes )
        {
            if( intercep.time < fiTime ){
                fiTime = intercep.time;
                index = intercep.id;
            }
        }
        if( index != -1 )
        {
            if( coolDownCounter < 0f )
            {
                //calculate the position of the projectile at this time interval
                Projectile p = projectiles[index];
                //get future position using s = ut + 1/2at^2
                Vec3 predictedPos = p.Position + (p.Velocity * fiTime + p.Acceleration * 0.5f * fiTime * fiTime);
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = predictedPos.ToVector3();
                sphere.transform.localScale = new Vector3( 0.2f, 0.2f, 0.2f );

                Vec3 dirToPos = predictedPos - position;
                float distToTarget = dirToPos.Normalize();
                
                //launcher vector velocity 
                Vec3 v3Vel = new Vec3(0f, launcher.launchVelocity * Mathf.Sin(launcher.launchAngle * Mathf.Deg2Rad),
                                      launcher.launchVelocity * Mathf.Cos(launcher.launchAngle * Mathf.Deg2Rad));
                Transform tx = launcher.transform;
               tx.rotation = Quaternion.LookRotation(dirToPos.ToVector3());
                v3Vel = new Vec3(tx.transform.TransformDirection(v3Vel.ToVector3()));
                
                Vec3 impactPos = position + ( v3Vel * fiTime + p.Acceleration * 0.5f * fiTime * fiTime);

                GameObject isphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                isphere.transform.position = impactPos.ToVector3();
                isphere.transform.localScale = new Vector3( 0.2f, 0.2f, 0.2f );

               
                launcher.FireProjectile(dirToPos, 2 );//fiTime
                coolDownCounter = coolDownTime;
            }
        }

    }

    float findTimeToIntercept( float launcherVelocity, Vec3 projectilePos, Vec3 projectileVel )
    {
        // law of cosines formula c^2 = a^2 + b^2 - 2ab*cos(phi);
        // re-arrange to look like quadratic
        //  x = (lv - pv)t^2 + (2 ab* cos(phi))t - a^2;  
        // for quadratic x = ax^2 + bx + c
        //  a = (lv - pv)^2
        //  b = (2ab * A.B)
        //  c = a^2

        //get direction to projectile for A.B dot product (want direction from projectile towards gun)
        Vec3 directionShooterToProjectile = new Vec3(transform.position) - projectilePos;
        float distanceToProjectileSquared = directionShooterToProjectile.MagnitudeSquared();
        //As the projectile is only accelerating in the Y we can remove this part of the veclocity as it will not 
        //creates a complexity in the equation we can avoid by ignoring it and making this a problem that only exists in the 
        //X/Z Plane.
        Vec3 horizontalProjectileVelocity = new Vec3( projectileVel.x, 0f, projectileVel.z);

        //for the quadratic 
        float c = -(distanceToProjectileSquared);
         //abcos(phi) from the formula can be calculated as the dot product
        //of the projectile velocity and the direction to projectile
        // A.B = |A||B|cos(phi), |A| = a, |B| = b 
        float b = 2 * Vec3.DotProduct(directionShooterToProjectile, horizontalProjectileVelocity);
        //Fun fact the dot product of a vector with itself provides you with the lenght of the vector squared.
        float a = launcherVelocity * launcherVelocity - horizontalProjectileVelocity.DotProduct(horizontalProjectileVelocity);

        float timeToIntercept = UseQuadraticFormula(a, b, c);
        return timeToIntercept;
    }

    float UseQuadraticFormula( float a, float b, float c)
    {
        //if A is nearly 0 then the formula doesn't really hold true
        if( 0.0001f > Mathf.Abs(a) )
        {
            return 0f;
        }

        float bb = b * b;
        float ac = a * c;
        float b4ac = Mathf.Sqrt(bb - 4f * ac);
        float t1 = (-b + b4ac)/ (2f * a);
        float t2 = (-b - b4ac)/ (2f * a);
        float t = Mathf.Max(t1,t2); //only return the highest value as one of these may be negative
        return t;

    }
}
