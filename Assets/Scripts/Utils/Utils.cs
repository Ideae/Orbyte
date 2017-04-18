using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
    {
        public static IEnumerable<Transform> Children(this Transform t)
        {
            return t.Cast<Transform>();
        }
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static bool DebugMode => Application.isEditor;
        public static bool Approx(this float a, float b) => Mathf.Approximately(a, b);
    }
