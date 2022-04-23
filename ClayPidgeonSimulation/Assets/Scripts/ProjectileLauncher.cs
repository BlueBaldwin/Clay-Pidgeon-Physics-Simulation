using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour {

    //publicly modifiable variables
    public float launchVelocity = 10f; //the launch velocity of the projectile
    public float launchAngle = 30f; //the angle the projectile is fired at
    public float Gravity = -9.8f; //the gravity that effects the projectiles

    public Vec3 v3InitialVelocity = new Vec3(); //Launch Velocity as Vector
    private Vec3 v3Acceleration; //Vector quantity for acceleration.

    private float airTime = 0f; //How long will the projectile be in the air
    private float horizontalDisplacement = 0f; //How far in the horizontal plane will the projectile travel?

    //Variables that relate to drawing the path of the projectile
    private List<Vec3> pathPoints; //List of points along the path of the vector for drawing line of trave
    private int simulationSteps = 30; //Number of points on the path of projectile to

    public GameObject projectile; //Game object to instantiate for our projectile
    public GameObject launchPoint; //game object to use as our launch point

    private float coolDownTimer = 2f;
    public float trapTimer = 2f;

    private int mSeed = 5;
    private int maxTime = 1;

    public bool useUpdate = true;

    void Start() {

        //initialise path vector for drawing
        pathPoints = new List<Vec3>();
        CalculateProjectile();
        calculatePath();
        coolDownTimer = (useUpdate) ? 2f : 0.2f;


    }

    private void CalculateProjectile() {

        launchAngle = transform.parent.eulerAngles.x;
        //work out our vertical offset
        float launchHeight = launchPoint.transform.position.y;
        
        //work out velocity as vector quantity
        //The velocity is claculated here from the perspective of the cannon
        v3InitialVelocity.x = 0f; //set x value to zero
        v3InitialVelocity.z = launchVelocity * Mathf.Cos(launchAngle * Mathf.Deg2Rad);
        v3InitialVelocity.y = launchVelocity * Mathf.Sin(launchAngle * Mathf.Deg2Rad);
        //V3Velocity is in local space facing down the cannons z-axis
        //transform that into a world space direction if this step is ommitted the projectile will always 
        //move down the world's z-axis.
        Vector3 txDirection = launchPoint.transform.TransformDirection(v3InitialVelocity.ToVector3());
        v3InitialVelocity = new Vec3(txDirection);
        //gravity as a vec3
        v3Acceleration = new Vec3(0f, Gravity, 0f);
        //calculate total time in air
        //Use quadratic equation to find air time
        airTime = UseQuadraticFormula(v3Acceleration.y, v3InitialVelocity.y * 2f, launchHeight * 2f);
        //calculate total distance travelled horizontally prior to the projectile hitting the ground
        horizontalDisplacement = airTime * v3InitialVelocity.z;
    }

    float UseQuadraticFormula(float a, float b, float c) {

        //if A is nearly 0 then the formula doesn't really hold true
        if (0.0001f > Mathf.Abs(a)) {
            return 0f;
        }

        float bb = b * b;
        float ac = a * c;
        float b4ac = Mathf.Sqrt(bb - 4f * ac);
        float t1 = (-b + b4ac) / (2f * a);
        float t2 = (-b - b4ac) / (2f * a);
        float t = Mathf.Max(t1, t2); //only return the highest value as one of these may be negative
        return t;
    }

    private void calculatePath() {

        Vec3 launchPos = new Vec3(launchPoint.transform.position);
        pathPoints.Add(launchPos);

        for (int i = 0; i <= simulationSteps; ++i) {
            float simTime = (i / (float) simulationSteps) * airTime;
            //suvat formulat for displacement s = ut + 1/2at^2
            Vec3 displacement = v3InitialVelocity * simTime + v3Acceleration * simTime * simTime * 0.5f;
            Vec3 drawPoint = launchPos + displacement;
            pathPoints.Add(drawPoint);
        }
    }

    void drawPath() {

        for (int i = 0; i < pathPoints.Count - 1; ++i) {
            Debug.DrawLine(pathPoints[i].ToVector3(), pathPoints[i + 1].ToVector3(), Color.green);
        }
    }

    // Update is called once per frame
    private void FixedUpdate() {

        coolDownTimer -= Time.deltaTime;
        
        // resetting trap timer to even out run time costs
        trapTimer -= Time.deltaTime;
        if (trapTimer <= 0) {
            trapTimer -= maxTime;
        }
    }

    private void Update() {

        drawPath();
        // Random Number Generator
        mSeed = LFSR(mSeed);
        float rand = mSeed % 10 + 1;
        
        if (useUpdate) {
            if (projectile != null && trapTimer <= 0) {
                pathPoints = new List<Vec3>();
                CalculateProjectile();
                calculatePath();

                // Instantiate at the launch pont + current rotation
                GameObject p = Instantiate(projectile, launchPoint.transform.position, launchPoint.transform.rotation);
                Projectile cb = p.GetComponent<Projectile>();
                cb.GetComponent<Projectile>().Velocity = v3InitialVelocity;
                cb.GetComponent<Projectile>().Acceleration = v3Acceleration;
                cb.GetComponent<Projectile>().LifeSpan = airTime;
                trapTimer = rand;
            }
        }
    }

    public void FireProjectile(Vec3 direction, float a_lifeSpan) {

        coolDownTimer -= Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(direction.ToVector3());
        
        if (coolDownTimer < 0f) {
            pathPoints.Clear();
            CalculateProjectile();
            calculatePath();
            // Instantiate at the launch point position, with the current rotation
            GameObject cb = Instantiate(projectile, launchPoint.transform.position, launchPoint.transform.rotation);
            Projectile pro = cb.GetComponent<Projectile>();
            pro.Velocity = v3InitialVelocity;
            pro.Acceleration = v3Acceleration;
            pro.LifeSpan = a_lifeSpan;
            coolDownTimer = 0.2f;
        }
    }
    
    // Random number Seed Generator
    private int LFSR(int seed) {
        int lsb = seed & 0x1;
        seed >>= 1;
        if (lsb != 0) {
            seed ^= 0x400A1;
        }

        return seed;

    }

}




