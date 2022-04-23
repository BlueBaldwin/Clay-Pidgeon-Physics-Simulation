using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox  {
    
    // A bounding box is always axis aligned
    private static Vec3 xAxis = new Vec3(1f, 0f, 0f);
    private static Vec3 yAxis = new Vec3(0f, 1f, 0f);
    private static Vec3 zAxis = new Vec3(0f, 0f, 1f);
    
    // As this box may be asked numerous times for it's Min and Max extremes we will calculate these
    // Once and store them in the following six variables to avoid the need to calculate them again.

    private float xMin = 0f;
    private float xMax = 0f;
    private float yMin = 0f;
    private float yMax = 0f;
    private float zMin = 0f;
    private float zMax = 0f;
    
    // Property for the position of the bounding box
    private Vec3 v3Position = new Vec3(0f, 0f, 0f);
    public Vec3 position {
        set {
            v3Position = new Vec3(0f, 0f, 0f);
        }
        get {
            return v3Position;
        }
    }

    private Vec3 v3Extents = new Vec3(0f, 0f, 0f);
    public Vec3 Extents {
        set {
            v3Extents = value;
            calcExtremes();
        }
        get { return v3Extents; }
    }

    private void calcExtremes() {
        xMin = v3Position.x - v3Extents.x;
        xMax = v3Position.x + v3Extents.x;

        yMin = v3Position.y - v3Extents.y;
        yMax = v3Position.y + v3Extents.y;

        zMin = v3Position.z - v3Extents.z;
        zMax = v3Position.z + v3Extents.z;
    }
    
    // To test if an object is inside of the bounds of the bounding box
    public bool containsObject(Vec3 aPos, Vec3 aBounds) {
        
        // bouding box origin is at center of box
        Vec3 halfABounds = aBounds * 0.5f;
        
        if (aPos.x - halfABounds.x < xMax && aPos.x + halfABounds.x > xMin &&
            aPos.y - halfABounds.y < yMax && aPos.y + halfABounds.y > yMin &&
            aPos.z - halfABounds.z < zMax && aPos.z + halfABounds.z > zMin) {
            return true;
        }
        return false;
        
    }
    
    // Visualise the box within Unity

    public void Draw() {
        // To draw he box we need to draw 12 lines in total
        
        // Draw vertical lines of the box
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMin, yMin, zMin), Color.cyan);
        Debug.DrawLine(new Vector3(xMin, yMax, zMax), new Vector3(xMin, yMin, zMax), Color.cyan);
        Debug.DrawLine(new Vector3(xMax, yMax, zMin), new Vector3(xMax, yMin, zMin), Color.cyan);
        Debug.DrawLine(new Vector3(xMax, yMax, zMax), new Vector3(xMax, yMin, zMax), Color.cyan);
        
        // Draw top lines of the box
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMin, yMax, zMax), Color.cyan);
        Debug.DrawLine(new Vector3(xMax, yMax, zMin), new Vector3(xMax, yMax, zMax), Color.cyan);
        Debug.DrawLine(new Vector3(xMin, yMax, zMin), new Vector3(xMax, yMax, zMin), Color.cyan);
        Debug.DrawLine(new Vector3(xMin, yMax, zMax), new Vector3(xMax, yMax, zMax), Color.cyan);
        
        // Draw bottom lines of the box
        Debug.DrawLine(new Vector3(xMin, yMin, zMin), new Vector3(xMin, yMin, zMax), Color.cyan);
        Debug.DrawLine(new Vector3(xMax, yMin, zMin), new Vector3(xMax, yMin, zMax), Color.cyan);
        Debug.DrawLine(new Vector3(xMin, yMin, zMin), new Vector3(xMax, yMin, zMin), Color.cyan);
        Debug.DrawLine(new Vector3(xMin, yMin, zMax), new Vector3(xMax, yMin, zMax), Color.cyan);
    }

    public BoundingBox(Vec3 aOrigin, Vec3 aExtents) {
        v3Position = aOrigin;
        // Set extends using the Extends Property so that Calc Extents is called 
        Extents = aExtents;
    }
}
