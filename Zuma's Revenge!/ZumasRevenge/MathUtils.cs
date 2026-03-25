using System;
using SexyFramework.Misc;

namespace ZumasRevenge;

public static class MathUtils
{
	public const float EPSILON = 1E-06f;

	public const float JL_PI = (float)Math.PI;

	private static Random mRandomGen = new Random();

	public static int SafeRand()
	{
		return mRandomGen.Next();
	}

	public static int Rand(int range)
	{
		return mRandomGen.Next() % range;
	}

	public static void Seed(int seed)
	{
		mRandomGen = new Random(seed);
	}

	public static int Rand()
	{
		return mRandomGen.Next();
	}

	public static float RadiansToDegrees(float pRads)
	{
		return pRads * 57.29694f;
	}

	public static float DegreesToRadians(float pDegs)
	{
		return pDegs * 0.017452938f;
	}

	public static int Sign(int val)
	{
		if (val >= 0)
		{
			return 1;
		}
		return -1;
	}

	public static float Sign(float val)
	{
		if (!(val < 0f))
		{
			return 1f;
		}
		return -1f;
	}

	public static bool _eq(float n1, float n2, float tolerance)
	{
		return Math.Abs(n1 - n2) <= tolerance;
	}

	public static bool _leq(float n1, float n2, float tolerance)
	{
		if (!_eq(n1, n2, tolerance))
		{
			return n1 < n2;
		}
		return true;
	}

	public static bool _geq(float n1, float n2, float tolerance)
	{
		if (!_eq(n1, n2, tolerance))
		{
			return n1 > n2;
		}
		return true;
	}

	public static bool _eq(float n1, float n2)
	{
		return Math.Abs(n1 - n2) <= float.Epsilon;
	}

	public static bool _leq(float n1, float n2)
	{
		if (!_eq(n1, n2, float.Epsilon))
		{
			return n1 < n2;
		}
		return true;
	}

	public static bool _geq(float n1, float n2)
	{
		if (!_eq(n1, n2, float.Epsilon))
		{
			return n1 > n2;
		}
		return true;
	}

	public static int IntRange(int min_val, int max_val)
	{
		if (min_val == max_val)
		{
			return min_val;
		}
		if (min_val < 0 && max_val < 0)
		{
			return min_val + SafeRand() % (Math.Abs(min_val) - Math.Abs(max_val));
		}
		return min_val + SafeRand() % (max_val - min_val + 1);
	}

	public static float FloatRange(float min_val, float max_val)
	{
		if (min_val == max_val)
		{
			return min_val;
		}
		if (min_val < 0f && max_val < 0f)
		{
			return min_val + (float)(SafeRand() % (int)((Math.Abs(min_val) - Math.Abs(max_val)) * 100000000f + 1f)) / 100000000f;
		}
		return min_val + (float)(SafeRand() % (int)((max_val - min_val) * 100000000f + 1f)) / 100000000f;
	}

	public static void Clamp(ref int value, int min_val, int max_val)
	{
		if (value < min_val)
		{
			value = min_val;
		}
		else if (value > max_val)
		{
			value = max_val;
		}
	}

	public static bool IncrementAndClamp(ref float val, float target, float inc)
	{
		val += inc;
		if (inc > 0f && val >= target)
		{
			val = target;
			return true;
		}
		if (inc < 0f && val <= target)
		{
			val = target;
			return true;
		}
		return false;
	}

	public static int GetClosestPowerOf2Above(int theNum)
	{
		int num;
		for (num = 1; num < theNum; num <<= 1)
		{
		}
		return num;
	}

	public static bool IsPowerOf2(int theNum)
	{
		int num = 0;
		while (theNum > 0)
		{
			num += theNum & 1;
			theNum >>= 1;
		}
		return num == 1;
	}

	public static float Distance(Point p1, Point p2, bool sqrt)
	{
		float num = p2.mX - p1.mX;
		float num2 = p2.mY - p1.mY;
		float num3 = num * num + num2 * num2;
		if (!sqrt)
		{
			return num3;
		}
		return (float)Math.Sqrt(num3);
	}

	public static float Distance(Point p1, Point p2)
	{
		return Distance(p1, p2, sqrt: true);
	}

	public static float Distance(float p1x, float p1y, float p2x, float p2y, bool sqrt)
	{
		float num = p2x - p1x;
		float num2 = p2y - p1y;
		float num3 = num * num + num2 * num2;
		if (!sqrt)
		{
			return num3;
		}
		return (float)Math.Sqrt(num3);
	}

	public static float Distance(float p1x, float p1y, float p2x, float p2y)
	{
		return Distance(p1x, p1y, p2x, p2y, sqrt: true);
	}

	public static bool CirclesIntersect(float x1, float y1, float x2, float y2, float total_radius, ref float seperation)
	{
		float num = x1 - x2;
		float num2 = y1 - y2;
		return (seperation = num * num + num2 * num2) < total_radius * total_radius;
	}

	public static bool CirclesIntersect(float x1, float y1, float x2, float y2, float total_radius)
	{
		float seperation = 0f;
		return CirclesIntersect(x1, y1, x2, y2, total_radius, ref seperation);
	}
}
