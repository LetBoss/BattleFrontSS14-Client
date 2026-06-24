// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.NetBitArraySerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

#nullable enable
namespace Robust.Shared.Serialization;

internal sealed class NetBitArraySerializer : IDynamicTypeSerializer, ITypeSerializer
{
  public bool Handles(Type type) => type == typeof (BitArray);

  public IEnumerable<Type> GetSubtypes(Type type)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerable<Type>) new \u003C\u003Ez__ReadOnlyArray<Type>(new Type[2]
    {
      typeof (int[]),
      typeof (int)
    });
  }

  public void GenerateWriterMethod(Serializer serializer, Type type, ILGenerator il)
  {
    MethodInfo method = typeof (NetBitArraySerializer).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic);
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Ldarg_2);
    il.EmitCall(OpCodes.Call, method, (Type[]) null);
    il.Emit(OpCodes.Ret);
  }

  public void GenerateReaderMethod(Serializer serializer, Type type, ILGenerator il)
  {
    MethodInfo method = typeof (NetBitArraySerializer).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Ldarg_2);
    il.EmitCall(OpCodes.Call, method, (Type[]) null);
    il.Emit(OpCodes.Ret);
  }

  private static void Write(Serializer serializer, Stream stream, BitArray value)
  {
    int[] numArray = new int[31 /*0x1F*/ + value.Length >> 5];
    value.CopyTo((Array) numArray, 0);
    serializer.SerializeDirect<int>(stream, 0);
    serializer.SerializeDirect<int[]>(stream, numArray);
    serializer.SerializeDirect<int>(stream, value.Length);
  }

  private static void Read(Serializer serializer, Stream stream, out BitArray value)
  {
    int num1;
    serializer.DeserializeDirect<int>(stream, ref num1);
    int[] values;
    serializer.DeserializeDirect<int[]>(stream, ref values);
    int num2;
    serializer.DeserializeDirect<int>(stream, ref num2);
    value = new BitArray(values) { Length = num2 };
  }
}
