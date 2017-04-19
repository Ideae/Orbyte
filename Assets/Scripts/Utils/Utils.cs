using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static partial class Utils
{
	public static bool DebugMode => Application.isEditor;

	public static IEnumerable<Transform> Children(this Transform t) => t.Cast<Transform>();

	public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);

	public static bool Approx(this float a, float b) => Mathf.Approximately(a, b);
}