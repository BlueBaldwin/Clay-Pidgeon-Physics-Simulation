using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vec2
{
    public float x = 0f;
    public float y = 0f;
    public Vec2(){
        x = 0f; y = 0f;
    }

    public Vec2( float a_x, float a_y ){
        x = a_x; y = a_y;
    }

    public Vec2( Vector2 a_pos ){
        x = a_pos.x; y = a_pos.y;
    }

    //convert from our Vec2 custom data structure to Unity's Vector3 data structure
    public Vector3 ToVector3(){
        return new Vector3(x, y, 0f);
    }
    //convert from our Vec2 format to Unity Vector2 format
    public Vector2 ToVector2(){
        return new Vector2(x, y);
    }

    //overload the add (+) operator
    public static Vec2 operator +( Vec2 a, Vec2 b){
        return new Vec2( a.x + b.x, a.y + b.y);
    }

    public static Vec2 operator -( Vec2 a){
        return new Vec2( -a.x, -a.y);
    }

    public static Vec2 operator -( Vec2 a, Vec2 b){
        return new Vec2( a.x - b.x, a.y - b.y);
    }

    public static Vec2 operator *( Vec2 a, Vec2 b)
    {
        return new Vec2( a.x * b.x, a.y * b.y);
    }

    public static Vec2 operator *( Vec2 a, float b)
    {
        return new Vec2( a.x * b, a.y * b);
    }

    public float Magnitude(){
        return Mathf.Sqrt(x * x + y * y);
    }

    public float MagnitudeSquared(){
        return x*x+y*y;
    }

    public float DotProduct( Vec2 a_b )
    {
        return x * a_b.x + y * a_b.y;
    }

    public static float DotProduct( Vec2 a, Vec2 b )
    {
        return a.x * b.x + a.y * b.y;
    }



    /// Normalize 
    ///     This function transforms the vector into a unit length vector
    ///     A Unit vector has a length of 1.0
    ///     It is possible to use scientific notation within C#
    ///     Return the original length of the vector before normalisation
    public float Normalize()
    {
        float fMag = Magnitude();
        float fInvNormal = (fMag != 0f) ? 1f/fMag : 1.00e+12f;
        x *= fInvNormal;
        y *= fInvNormal;
        return fMag;

    }

    public Vec2 Perpendicular()
    {
        return new Vec2( -y, x );
    }

    public void Rotate( float angle )
    {
        float fX = x;
        x = fX * Mathf.Cos(angle) - y * Mathf.Sin(angle);
        y = fX * Mathf.Sin(angle) + y * Mathf.Cos(angle);
    }

}
