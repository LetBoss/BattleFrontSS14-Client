// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.NetSafeFloatSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Serialization;

internal sealed class NetSafeFloatSerializer : IStaticTypeSerializer, ITypeSerializer
{
  public bool Handles(Type type)
  {
    return type == typeof (float) || type == typeof (double) || type == typeof (Half);
  }

  public IEnumerable<Type> GetSubtypes(Type type) => (IEnumerable<Type>) Array.Empty<Type>();

  public MethodInfo GetStaticWriter(Type type)
  {
    return typeof (Primitives).GetMethod("WritePrimitive", BindingFlags.Static | BindingFlags.Public, new Type[2]
    {
      typeof (Stream),
      type
    });
  }

  public MethodInfo GetStaticReader(Type type)
  {
    return typeof (SafePrimitives).GetMethod("ReadPrimitive", BindingFlags.Static | BindingFlags.Public, new Type[2]
    {
      typeof (Stream),
      type.MakeByRefType()
    });
  }
}
