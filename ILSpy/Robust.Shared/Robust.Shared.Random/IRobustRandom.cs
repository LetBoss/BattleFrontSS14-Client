using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Collections;
using Robust.Shared.Maths;

namespace Robust.Shared.Random;

public interface IRobustRandom
{
	[Obsolete("Do not access the underlying implementation")]
	System.Random GetRandom();

	void SetSeed(int seed);

	float NextFloat();

	float NextFloat(float minValue, float maxValue)
	{
		return NextFloat() * (maxValue - minValue) + minValue;
	}

	float NextFloat(float maxValue)
	{
		return NextFloat() * maxValue;
	}

	int Next();

	int Next(int maxValue);

	int Next(int minValue, int maxValue);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	byte NextByte()
	{
		return NextByte(byte.MaxValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	byte NextByte(byte maxValue)
	{
		return NextByte(0, maxValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	byte NextByte(byte minValue, byte maxValue)
	{
		return (byte)Next(minValue, maxValue);
	}

	double NextDouble();

	double Next(double maxValue)
	{
		return NextDouble() * maxValue;
	}

	double NextDouble(double minValue, double maxValue)
	{
		return NextDouble() * (maxValue - minValue) + minValue;
	}

	TimeSpan Next(TimeSpan maxTime);

	TimeSpan Next(TimeSpan minTime, TimeSpan maxTime);

	void NextBytes(byte[] buffer);

	Angle NextAngle()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Angle.op_Implicit(NextFloat() * ((float)Math.PI * 2f));
	}

	Angle NextAngle(Angle maxValue)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return Angle.op_Implicit((double)NextFloat() * Angle.op_Implicit(maxValue));
	}

	Angle NextAngle(Angle minValue, Angle maxValue)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return Angle.op_Implicit((double)NextFloat() * Angle.op_Implicit(maxValue - minValue)) + minValue;
	}

	Vector2 NextVector2(float maxMagnitude = 1f)
	{
		return NextVector2(0f, maxMagnitude);
	}

	Vector2 NextVector2(float minMagnitude, float maxMagnitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Angle val = NextAngle();
		Vector2 vector = new Vector2(NextFloat(minMagnitude, maxMagnitude), 0f);
		return ((Angle)(ref val)).RotateVec(ref vector);
	}

	Vector2 NextVector2Box(float minX, float minY, float maxX, float maxY)
	{
		return new Vector2(NextFloat(minX, maxX), NextFloat(minY, maxY));
	}

	Vector2 NextVector2Box(float maxAbsX = 1f, float maxAbsY = 1f)
	{
		return NextVector2Box(0f - maxAbsX, 0f - maxAbsY, maxAbsX, maxAbsY);
	}

	void Shuffle<T>(IList<T> list)
	{
		if (list is T[] array)
		{
			Shuffle(array);
			return;
		}
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int num2 = Next(num + 1);
			int index = num2;
			int index2 = num;
			T value = list[num];
			T value2 = list[num2];
			list[index] = value;
			list[index2] = value2;
		}
	}

	void Shuffle<T>(Span<T> list)
	{
		int num = list.Length;
		while (num > 1)
		{
			num--;
			int index = Next(num + 1);
			ref T reference = ref list[index];
			ref T reference2 = ref list[num];
			T val = list[num];
			T val2 = list[index];
			reference = val;
			reference2 = val2;
		}
	}

	void Shuffle<T>(ValueList<T> list)
	{
		Shuffle(list.Span);
	}
}
