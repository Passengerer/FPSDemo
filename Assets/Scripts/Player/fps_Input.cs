using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fps_Input {

    /// <summary>
    /// 将轴作为按键，按了返回true
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static bool AxisToButton(string axis)
    {
        if (Mathf.Abs(Input.GetAxis(axis)) == 0)
            return false;
        return true;
    }
}
