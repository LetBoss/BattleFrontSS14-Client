using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NetSerializer;

namespace Robust.Shared.Serialization;

internal sealed class NetMathSerializer : IStaticTypeSerializer, ITypeSerializer
{
	public bool Handles(Type type)
	{
		if (!(type == typeof(Vector2)) && !(type == typeof(Vector3)) && !(type == typeof(Vector4)) && !(type == typeof(Quaternion)) && !(type == typeof(Matrix4x4)))
		{
			return type == typeof(Matrix3x2);
		}
		return true;
	}

	public IEnumerable<Type> GetSubtypes(Type type)
	{
		return Type.EmptyTypes;
	}

	public MethodInfo GetStaticWriter(Type type)
	{
		return typeof(NetMathSerializer).GetMethod("WriteFloatObject", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
	}

	public MethodInfo GetStaticReader(Type type)
	{
		return typeof(NetMathSerializer).GetMethod("ReadFloatObject", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
	}

	public static void WriteFloatObject<T>(Stream stream, T value) where T : unmanaged
	{
		Span<float> span = MemoryMarshal.Cast<T, float>(new Span<T>(ref value));
		for (int i = 0; i < span.Length; i++)
		{
			float num = span[i];
			Primitives.WritePrimitive(stream, num);
		}
	}

	public static void ReadFloatObject<T>(Stream stream, out T value) where T : unmanaged
	{
		Unsafe.SkipInit<T>(out value);
		Span<float> span = MemoryMarshal.Cast<T, float>(new Span<T>(ref value));
		for (int i = 0; i < span.Length; i++)
		{
			Primitives.ReadPrimitive(stream, ref span[i]);
		}
	}
}
