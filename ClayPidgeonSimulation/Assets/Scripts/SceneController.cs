using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {
    private bool TargetDestroyed;
    private void Update() {
        OctreeNode rootNode = new OctreeNode(new Vec3(0f, 20f, 0f), new Vec3(20f, 20f, 20f));
        // Find all projectiles that are launched in the sandbox
        
        GameObject[] allActiveProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
        
        foreach (GameObject projectile in allActiveProjectiles) {
            // add the projectiles to the root node
            rootNode.AddObject(projectile);
        }
        // Draw the Octree
        rootNode.Draw();
        
        // Iterate through each node in the octree and perform collison tests
        // from the rootNode step into each child node and test collisions for each object in the node.
        rootNode.PerformCollisionTest();
    }
}
