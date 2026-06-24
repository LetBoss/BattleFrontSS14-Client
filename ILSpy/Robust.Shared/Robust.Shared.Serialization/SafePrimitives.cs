using System;
using System.IO;
using System.Runtime.CompilerServices;
using NetSerializer;

namespace Robust.Shared.Serialization;

internal static class SafePrimitives
{
	public static void ReadPrimitive(Stream stream, out float value)
	{
		Unsafe.SkipInit(out float num);
		Primitives.ReadPrimitive(stream, ref num);
		value = (float.IsNaN(num) ? 0f : num);
	}

	public static void ReadPrimitive(Stream stream, out double value)
	{
		Unsafe.SkipInit(out double num);
		Primitives.ReadPrimitive(stream, ref num);
		value = (double.IsNaN(num) ? 0.0 : num);
	}

	public static void ReadPrimitive(Stream stream, out Half value)
	{
		Unsafe.SkipInit(out Half half);
		Primitives.ReadPrimitive(stream, ref half);
		value = (Half.IsNaN(half) ? Half.Zero : half);
	}
}
