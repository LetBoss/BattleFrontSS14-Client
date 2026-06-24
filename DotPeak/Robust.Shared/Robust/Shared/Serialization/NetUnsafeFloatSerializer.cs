// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.NetUnsafeFloatSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Serialization;

internal sealed class NetUnsafeFloatSerializer : IStaticTypeSerializer, ITypeSerializer
{
  public bool Handles(Type type)
  {
    return type == typeof (UnsafeFloat) || type == typeof (UnsafeDouble) || type == typeof (UnsafeHalf);
  }

  public IEnumerable<Type> GetSubtypes(Type type) => (IEnumerable<Type>) Array.Empty<Type>();

  public MethodInfo GetStaticWriter(Type type)
  {
    return typeof (NetUnsafeFloatSerializer).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
    {
      typeof (Stream),
      type
    });
  }

  public MethodInfo GetStaticReader(Type type)
  {
    return typeof (NetUnsafeFloatSerializer).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic, new Type[2]
    {
      typeof (Stream),
      type.MakeByRefType()
    });
  }

  private static void Write(Stream stream, UnsafeFloat value)
  {
    Primitives.WritePrimitive(stream, UnsafeFloat.op_Implicit(value));
  }

  private static void Read(Stream stream, out UnsafeFloat value)
  {
    float num;
    Primitives.ReadPrimitive(stream, ref num);
    value = UnsafeFloat.op_Implicit(num);
  }

  private static void Write(Stream stream, UnsafeDouble value)
  {
    Primitives.WritePrimitive(stream, UnsafeDouble.op_Implicit(value));
  }

  private static void Read(Stream stream, out UnsafeDouble value)
  {
    double num;
    Primitives.ReadPrimitive(stream, ref num);
    value = UnsafeDouble.op_Implicit(num);
  }

  private static void Write(Stream stream, UnsafeHalf value)
  {
    Primitives.WritePrimitive(stream, UnsafeHalf.op_Implicit(value));
  }

  private static void Read(Stream stream, out UnsafeHalf value)
  {
    Half half;
    Primitives.ReadPrimitive(stream, ref half);
    value = UnsafeHalf.op_Implicit(half);
  }
}
