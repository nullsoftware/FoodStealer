using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class MathUtil
{
    public static bool Approximately(float a, float b, float tolerance = 0.1f)
    {
        return (Mathf.Abs(a - b) < tolerance);
    }
}