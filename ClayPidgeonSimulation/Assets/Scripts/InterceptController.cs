using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptController : MonoBehaviour {
	
	
	public static float projectileSpeed = 0f;
	public float fVisionRadius = 10f;
	public float speed = 0f;
	public ProjectileLauncher launcher;
	public static bool shotFired = false;
	
	private List<Projectile> projectiles;
	
	
	// Start is called before the first frame update
	void Start() {
		projectiles = new List<Projectile>();
	}

	// Update is called once per frame
	void Update() {
		projectiles.Clear();

		// Use Unity's FindObjectWithTag function to find all the targets in the scene
		GameObject[] allActiveTargets = GameObject.FindGameObjectsWithTag("Projectile");
		Vec3 position = new Vec3(transform.position);
		float radSqrd = fVisionRadius * fVisionRadius;

		foreach (GameObject target in allActiveTargets) {
			// Get the distance to this target
			Vec3 vecToTarget = new Vec3(target.transform.position) - position;
			float distanceToTarget = vecToTarget.MagnitudeSquared(); // Use mag squared to avoid sqrt calc for speed

			if (distanceToTarget < radSqrd) {
				Projectile p = target.GetComponent<Projectile>();
				projectiles.Add(p);
			} 
		}

		// Calculate time to each target intercept within range
		List<(int id, float time)> interceptTimes = new List<(int id, float time)>();
		
		for (int i = 0; i < projectiles.Count; i++) {
			interceptTimes.Add((i, findTimeToIntercept(launcher.launchVelocity * Mathf.Cos(launcher.launchAngle * Mathf.Deg2Rad), 
				projectiles[i].Position, projectiles[i].Velocity)));
		}

		int index = -1;
		float fiTime = float.MaxValue;
		
			foreach (var intercept in interceptTimes) {
				if (intercept.time > 0f && intercept.time < fiTime) {
					fiTime = intercept.time;
					index = intercept.id;
				}
			}

			if (index != -1) {
			Debug.Log("Closest interecept time is: " + fiTime);
			// Calculate the position of the projectile at this time interval
			Projectile p = projectiles[index];
			// Get future position using s = ut + 1/2at^2
			Vec3 predictedPos = p.Position + (p.Velocity * fiTime + p.Acceleration * 0.5f * fiTime * fiTime);

			Vec3 dirToPos = predictedPos - position;
			float disToTarget = dirToPos.Normalize();

			if (predictedPos.y > 0) {
				launcher.FireProjectile(dirToPos, 2);
				shotFired = true;
			}
		}
	}

	float findTimeToIntercept(float launcherVelocity, Vec3 projectilePos, Vec3 projectileVel) {

		// Law of cosines formula c^2 = a^2 + b^2 - 2ab*cos(phi);
		// Re-arranged to look like a quagratic formula.
		// x = (lv - pv) t^2 + (2 ab*cos(phi)) t - a^2);
		// For quadratic x = ax^2 + bx + c
		// a = (lv - pv)^2
		// b = (2ab * A.B)
		// c = a^2

		// Get direction to projectile for A.B dot product (we want the direction from the projectile towards the gun)
		Vec3 directionShooterToProjectile = new Vec3(transform.position) - projectilePos;
		float distanceToProjectileSquared = directionShooterToProjectile.MagnitudeSquared();
		// As the projectile is only accelerating in the Y axis we can remove this part of the velocity as it will 		
		// only create complexityin the equation we can avoid by ignoring it and making this a problem that only exists 
		// in the X/Z plane.
		Vec3 horizontalProjectileVelocity = new Vec3(projectileVel.x, 0f, projectileVel.z);

		// For the quadratic
		float c = -(distanceToProjectileSquared);
		// abcos(phi), [A] = a, [B] = b
		float b = 2 * Vec3.DotProduct(directionShooterToProjectile, horizontalProjectileVelocity);
		// Fun fact to remember; The dot product of a vector with itself provides you with the length of the vector squared.
		float a = launcherVelocity * launcherVelocity - horizontalProjectileVelocity.DotProduct(horizontalProjectileVelocity);

		float timeToIntercept = UseQuadraticFormula(a, b, c);
		speed = Speed( distanceToProjectileSquared, timeToIntercept);
		projectileSpeed = speed;
		return timeToIntercept;
	}
	
	// Speed calculations - s = d/t
	public float Speed(float distanceToProjectileSquared, float findTimeToIntercept) {
		return distanceToProjectileSquared / findTimeToIntercept;
	}
	
	float UseQuadraticFormula(float a, float b, float c) {
		// If A is nearly 0 then the formula doesn't really hold true
		if (0.0001f > Mathf.Abs(a)) {
			return 0f;
		}
		float bb = b * b;
		float ac = a * c;
		float b4ac = bb - 4f * ac;
		if (b4ac < 0f) {
			return - 1f;
		}
		b4ac = Mathf.Sqrt(b4ac);
		
		float t1 = (-b + b4ac) / (2f * a);
		float t2 = (-b - b4ac) / (2f * a);
		float t = Mathf.Max(t1, t2); // Only return the highest value as one of these may be negative
		return t;
	}
}
