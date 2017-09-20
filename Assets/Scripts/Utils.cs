
using UnityEngine;
using System.Collections;

struct IntVec2
{
	public int x;
	public int y;

	public IntVec2(int px, int py)
	{
		x = px;
		y = py;
	}

	public bool IsInvalid()
	{
		return ((x==-1) || (y==-1));
	}
}

public class Utils
{
	public static Vector3 LerpVec3(Vector3 a, Vector3 b, float p)
	{
		return new Vector3(Lerp(a.x,b.x,p), Lerp(a.y,b.y,p), Lerp(a.z,b.z,p));
	}

	public static float Lerp(float a, float b, float p)
	{
		return a + (b-a)*p;
	}
}
