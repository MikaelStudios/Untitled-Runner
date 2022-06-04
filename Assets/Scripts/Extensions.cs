using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void Debug(string Log)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log(Log);
#endif
    }


    /// <summary>
    ///  Copy the exact position of another transform will no smoothing
    /// </summary>
    /// <param name="t"></param>
    /// <param name="WhatToFollow">The transform to follow</param>
    /// <param name="Smoothing">Add Snoothing to the mix</param>
    public static void Follow(this Transform t, Transform WhatToFollow, bool X = true, bool Y = true, bool Z = true, bool Smoothing = false, float smoothT = .1f)
    {
        if (!Smoothing)
            t.position = new Vector3
            (
                X ? WhatToFollow.position.x : t.position.x,
                Y ? WhatToFollow.position.y : t.position.y,
                Z ? WhatToFollow.position.z : t.position.z
            );
        else
        {
            t.position = Vector3.Lerp
                (
                t.position,
                new Vector3(
                X ? WhatToFollow.position.x : t.position.x,
                Y ? WhatToFollow.position.y : t.position.y,
                Z ? WhatToFollow.position.z : t.position.z),
                smoothT * Time.deltaTime * 10
                );
        }
    }


    /// <summary>
    ///  Copy the exact position of another vector3 will no smoothing
    /// </summary>
    /// <param name="t"></param>
    /// <param name="WhatToFollow">The transform to follow</param>
    /// <param name="Smoothing">Add Snoothing to the mix</param>
    public static void Follow(this Transform t, Vector3 WhatToFollow, bool X = true, bool Y = true, bool Z = true, bool Smoothing = false, float smoothT = .1f)
    {
        if (!Smoothing)
            t.position = new Vector3
            (
                X ? WhatToFollow.x : t.position.x,
                Y ? WhatToFollow.y : t.position.y,
                Z ? WhatToFollow.z : t.position.z
            );
        else
        {
            t.position = Vector3.Lerp
               (
               t.position,
               new Vector3(
               X ? WhatToFollow.x : t.position.x,
               Y ? WhatToFollow.y : t.position.y,
               Z ? WhatToFollow.z : t.position.z),
               smoothT * Time.deltaTime * 10
               );
        }
    }


    /// <summary>
    /// Smoothly follow a vector3 in any specific axis, To not smooth in an axis leave it as 0
    /// 0 means no smoothing, Above 0 is the smooting time
    /// </summary>
    /// <param name="t"></param>
    /// <param name="WhatToFollow">The vector3 to follow</param>
    public static void SmoothAxisFollow(this Transform t, Vector3 WhatToFollow, float X = 0, float Y = 0, float Z = 0)
    {

        t.position =
           new Vector3
           (
           X > 0 ? Mathf.Lerp(t.position.x, WhatToFollow.x, X * 10 * Time.deltaTime) : WhatToFollow.x,
           Y > 0 ? Mathf.Lerp(t.position.y, WhatToFollow.y, Y * 10 * Time.deltaTime) : WhatToFollow.y,
           Z > 0 ? Mathf.Lerp(t.position.z, WhatToFollow.z, Z * 10 * Time.deltaTime) : WhatToFollow.z
           );
    }

    //public static void GetRandomEnumValue(this Enum e)
    //{
    //    (e)UnityEngine.Random.Range(1, Enum.GetValues(typeof(e)).Length);
    //}

    /// <summary>
    /// Get a single Random element from the array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this T[] obj)
    {
        return obj[Random.Range(0, obj.Length)];
    }

    /// <summary>
    /// Get a single Random element from the list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T GetRandom<T>(this List<T> obj)
    {
        return obj[Random.Range(0, obj.Count)];
    }

    /// <summary>
    /// Is the collider tag equals "Player" for the OnTrigger functions
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsPlayer(this Collider t)
    {
        if (t.CompareTag("Player"))
            return true;
        else
            return false;
    }
    /// <summary>
    /// Is the collision tag equals "Player" for the OnCollision functions
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsPlayer(this Collision t)
    {
        if (t.gameObject.CompareTag("Player"))
            return true;
        else
            return false;
    }
    /// <summary>
    /// This sets the velocity and angualr velocity to vecto3.zero
    /// </summary>
    /// <param name="rb"></param>
    /// 


    public static void ResetVelocity(this Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    //public static void SetTrueExclusive(this bool b)
    //{
    //    //if (!b)
    //        b = true;
    //}
    //public static void SetFalseExclusive(this bool b)
    //{
    //    if (b)
    //        b = false;
    //}
    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }


    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector2 WithX(this Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }

    public static Vector2 WithY(this Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }

    public static Vector3 WithZ(this Vector2 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    // axisDirection - unit vector in direction of an axis (eg, defines a line that passes through zero)
    // point - the point to find nearest on line for
    public static Vector3 NearestPointOnAxis(this Vector3 axisDirection, Vector3 point, bool isNormalized = false)
    {
        if (!isNormalized) axisDirection.Normalize();
        var d = Vector3.Dot(point, axisDirection);
        return axisDirection * d;
    }

    // lineDirection - unit vector in direction of line
    // pointOnLine - a point on the line (allowing us to define an actual line in space)
    // point - the point to find nearest on line for
    public static Vector3 NearestPointOnLine(
        this Vector3 lineDirection, Vector3 point, Vector3 pointOnLine, bool isNormalized = false)
    {
        if (!isNormalized) lineDirection.Normalize();
        var d = Vector3.Dot(point - pointOnLine, lineDirection);
        return pointOnLine + (lineDirection * d);
    }

    /// <summary>
    /// Shuffle the list in place using the Fisher-Yates method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Return a random item from the list.
    /// Sampling with replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RandomItem<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Removes a random item from the list, returning that item.
    /// Sampling without replacement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T RemoveRandom<T>(this IList<T> list)
    {
        if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
        int index = UnityEngine.Random.Range(0, list.Count);
        T item = list[index];
        list.RemoveAt(index);
        return item;
    }
}
