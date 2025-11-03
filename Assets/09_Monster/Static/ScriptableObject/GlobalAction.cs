using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAction 
{
    public static float GetDisttance(in Vector2 _vSelf , in Vector2 _vTarget )
    {
        return Vector2.Distance(_vTarget , _vSelf);
    }

    public static float GetDisttance(in Vector3 _vSelf, in Vector3 _vTarget)
    {
        return Vector3.Distance(_vTarget, _vSelf);
    }

    public static float GetDirection(in Vector3 _vForward, in Vector3 _vSelf, in Vector3 _vTarget)
    {
        Vector3 vDir = (_vTarget - _vSelf).normalized;
        return Vector3.Angle(_vForward, vDir);
    }

}
