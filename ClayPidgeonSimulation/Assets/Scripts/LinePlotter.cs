using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LinePlotter : MonoBehaviour
{
    private List<Vec2> points;

    public float x1 = 0f;
    public float y1 = 0f;
    public float m = 1f;
    public float c = 0f;
    public int   xPow = 1;

    private float calcualteY( float y1, float x, float x1, float m, float c){
        return m * Mathf.Pow((x-x1),xPow) + c + y1;
    }

    private void PlotPoints(){
        //As we are using this in Editor it is possible to 
        //get here prior to Start Being called
        if( points == null )
        {
            points = new List<Vec2>();
        }

        points.Clear();
        float x = -10f;
        //create a number of points to go on our line
        for( float xPos = x; xPos < 10f; xPos += 0.5f)
        {
            points.Add( new Vec2(xPos, calcualteY(y1, xPos, x1, m, c)) );
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vec2>();
        Vec2 a = new Vec2(-7,9);
        Vec2 b = new Vec2(3,14);
        Vec2 c = a + b;
        Debug.Log( "X: " + c.x.ToString() + " Y: " + c.y.ToString() );
        PlotPoints();
    }

    private void OnDrawGizmos()
    {
        //if we have some points in our list
        if (points != null)
        {
            //set the gizmo colour
            Gizmos.color = Color.red;
            //for each point in the list draw a line from it to the next point in the list
            for( int i = 0; i < points.Count-1; ++i)
            {
                Gizmos.DrawLine( points[i].ToVector3(), points[i+1].ToVector3());
            }
        }
    }

    //on Validate is called when an editor change happens
    void OnValidate()
    {
        PlotPoints();
        Debug.Log("Re-populating gizmo positions");
    }
}