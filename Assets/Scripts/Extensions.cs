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
}
