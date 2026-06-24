// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.NetMathSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Serialization;

internal sealed class NetMathSerializer : IStaticTypeSerializer, ITypeSerializer
{
  public bool Handles(Type type)
  {
    return type == typeof (Vector2) || type == typeof (Vector3) || type == typeof (Vector4) || type == typeof (Quaternion) || type == typeof (Matrix4x4) || type == typeof (Matrix3x2);
  }

  public IEnumerable<Type> GetSubtypes(Type type) => (IEnumerable<Type>) Type.EmptyTypes;

  public MethodInfo GetStaticWriter(Type type)
  {
    return typeof (NetMathSerializer).GetMethod("WriteFloatObject", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
  }

  public MethodInfo GetStaticReader(Type type)
  {
    return typeof (NetMathSerializer).GetMethod("ReadFloatObject", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(type);
  }

  public static void WriteFloatObject<T>(Stream stream, T value) where T : unmanaged
  {
    Span<float> span = MemoryMarshal.Cast<T, float>(new Span<T>(ref value));
    for (int index = 0; index < span.Length; ++index)
    {
      float num = span[index];
      Primitives.WritePrimitive(stream, num);
    }
  }

  public static void ReadFloatObject<T>(Stream stream, out T value) where T : unmanaged
  {
    Unsafe.SkipInit<T>(out value);
    Span<float> span = MemoryMarshal.Cast<T, float>(new Span<T>(ref value));
    for (int index = 0; index < span.Length; ++index)
      Primitives.ReadPrimitive(stream, ref span[index]);
  }
}
