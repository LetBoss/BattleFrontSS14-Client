using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using NetSerializer;

namespace Robust.Shared.Serialization;

internal sealed class NetBitArraySerializer : IDynamicTypeSerializer, ITypeSerializer
{
	public bool Handles(Type type)
	{
		return type == typeof(BitArray);
	}

	public IEnumerable<Type> GetSubtypes(Type type)
	{
		return new global::_003C_003Ez__ReadOnlyArray<Type>(new Type[2]
		{
			typeof(int[]),
			typeof(int)
		});
	}

	public void GenerateWriterMethod(Serializer serializer, Type type, ILGenerator il)
	{
		MethodInfo method = typeof(NetBitArraySerializer).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic);
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Ldarg_2);
		il.EmitCall(OpCodes.Call, method, null);
		il.Emit(OpCodes.Ret);
	}

	public void GenerateReaderMethod(Serializer serializer, Type type, ILGenerator il)
	{
		MethodInfo method = typeof(NetBitArraySerializer).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
		il.Emit(OpCodes.Ldarg_0);
		il.Emit(OpCodes.Ldarg_1);
		il.Emit(OpCodes.Ldarg_2);
		il.EmitCall(OpCodes.Call, method, null);
		il.Emit(OpCodes.Ret);
	}

	private static void Write(Serializer serializer, Stream stream, BitArray value)
	{
		int[] array = new int[31 + value.Length >> 5];
		value.CopyTo(array, 0);
		serializer.SerializeDirect<int>(stream, 0);
		serializer.SerializeDirect<int[]>(stream, array);
		serializer.SerializeDirect<int>(stream, value.Length);
	}

	private static void Read(Serializer serializer, Stream stream, out BitArray value)
	{
		Unsafe.SkipInit(out int num);
		serializer.DeserializeDirect<int>(stream, ref num);
		Unsafe.SkipInit(out int[] values);
		serializer.DeserializeDirect<int[]>(stream, ref values);
		Unsafe.SkipInit(out int length);
		serializer.DeserializeDirect<int>(stream, ref length);
		value = new BitArray(values)
		{
			Length = length
		};
	}
}
