using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vec3
{
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    public Vec3(){
        x = 0f; y = 0f; z = 0f;
    }

    public Vec3( float a_x, float a_y, float a_z){
        x = a_x; y = a_y; z = a_z;
    }

    public Vec3( Vec3 a_pos )
    {
        x = a_pos.x; y = a_pos.y; z = a_pos.z;
    }

    public Vec3( Vector3 a_pos )
    {
        x = a_pos.x; y = a_pos.y; z = a_pos.z;
    }

    //convert from our Vec2 custom data structure to Unity's Vector3 data structure
    public Vector3 ToVector3(){
        return new Vector3(x, y, z);
    }

    public override string ToString(){
        return "Vec3: " + x.ToString() + "X " + y.ToString() + "Y " + z.ToString() + "Z";
    }
    
    //overload the add (+) operator
    public static Vec3 operator +( Vec3 a, Vec3 b){
        return new Vec3( a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Vec3 operator -( Vec3 a){
        return new Vec3( -a.x, -a.y, -a.z);
    }

    public static Vec3 operator -( Vec3 a, Vec3 b){
        return new Vec3( a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static Vec3 operator *( Vec3 a, Vec3 b)
    {
        return new Vec3( a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static Vec3 operator *( Vec3 a, float b)
    {
        return new Vec3( a.x * b, a.y * b, a.z * b);
    }

    public static Vec3 operator /( Vec3 a, float b)
    {
        return new Vec3( a.x / b, a.y / b, a.z / b);
    }

    public float Magnitude(){
        return Mathf.Sqrt(x * x + y * y + z * z);
    }

    public float MagnitudeSquared(){
        return x * x + y * y + z * z;
    }

    public float DotProduct( Vec3 a_b )
    {
        return x * a_b.x + y * a_b.y + z * a_b.z;
    }

    public static float DotProduct( Vec3 a, Vec3 b )
    {
        return a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /// Normalize 
    ///     This function transforms the vector into a unit length vector
    ///     A Unit vector has a length of 1.0
    ///     It is possible to use scientific notation within C#
    ///     Return the original length of the vector before normalisation
    public float Normalize()
    {
        float fMag = Magnitude();
        float fInvMag = (fMag != 0f) ? 1f/fMag : 1.00e+12f;
        x *= fInvMag;
        y *= fInvMag;
        z *= fInvMag;
        return fMag;

    }

    public static Vec3 CrossProduct( Vec3 a, Vec3 b)
    {
        return new Vec3( a.y*b.z - a.z*b.y,
                         a.z*b.x - a.x*b.z,
                         a.x*b.y - a.y*b.x);
    }

    public void RotateX( float angle )
    {
        float fY = y;
        y = fY * Mathf.Cos(angle) - z * Mathf.Sin(angle);
        z = fY * Mathf.Sin(angle) + z * Mathf.Cos(angle);
    }

    public void RotateY( float angle )
    {
        float fX = x;
        x = fX * Mathf.Cos(angle) - z * Mathf.Sin(angle);
        z = fX * Mathf.Sin(angle) + z * Mathf.Cos(angle);
    }

    public void RotateZ( float angle )
    {
        float fX = x;
        x = fX * Mathf.Cos(angle) - y * Mathf.Sin(angle);
        y = fX * Mathf.Sin(angle) + y * Mathf.Cos(angle);
    }

}

