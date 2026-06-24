using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using NetSerializer;
using Robust.Shared.Maths;

namespace Robust.Shared.Serialization;

internal sealed class NetUnsafeFloatSerializer : IStaticTypeSerializer, ITypeSerializer
{
	public bool Handles(Type type)
	{
		if (!(type == typeof(UnsafeFloat)) && !(type == typeof(UnsafeDouble)))
		{
			return type == typeof(UnsafeHalf);
		}
		return true;
	}

	public IEnumerable<Type> GetSubtypes(Type type)
	{
		return Array.Empty<Type>();
	}

	public MethodInfo GetStaticWriter(Type type)
	{
		return typeof(NetUnsafeFloatSerializer).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
		{
			typeof(Stream),
			type
		});
	}

	public MethodInfo GetStaticReader(Type type)
	{
		return typeof(NetUnsafeFloatSerializer).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
		{
			typeof(Stream),
			type.MakeByRefType()
		});
	}

	private static void Write(Stream stream, UnsafeFloat value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Primitives.WritePrimitive(stream, UnsafeFloat.op_Implicit(value));
	}

	private static void Read(Stream stream, out UnsafeFloat value)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out float num);
		Primitives.ReadPrimitive(stream, ref num);
		value = UnsafeFloat.op_Implicit(num);
	}

	private static void Write(Stream stream, UnsafeDouble value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Primitives.WritePrimitive(stream, UnsafeDouble.op_Implicit(value));
	}

	private static void Read(Stream stream, out UnsafeDouble value)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out double num);
		Primitives.ReadPrimitive(stream, ref num);
		value = UnsafeDouble.op_Implicit(num);
	}

	private static void Write(Stream stream, UnsafeHalf value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Primitives.WritePrimitive(stream, UnsafeHalf.op_Implicit(value));
	}

	private static void Read(Stream stream, out UnsafeHalf value)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Half half);
		Primitives.ReadPrimitive(stream, ref half);
		value = UnsafeHalf.op_Implicit(half);
	}
}
