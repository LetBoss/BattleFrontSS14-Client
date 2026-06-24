// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.NetFormattedStringSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using NetSerializer;
using Robust.Shared.RichText;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#nullable enable
namespace Robust.Shared.Serialization;

internal sealed class NetFormattedStringSerializer : IStaticTypeSerializer, ITypeSerializer
{
  public bool Handles(Type type) => type == typeof (FormattedString);

  public IEnumerable<Type> GetSubtypes(Type type)
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerable<Type>) new \u003C\u003Ez__ReadOnlySingleElementList<Type>(typeof (string));
  }

  public MethodInfo GetStaticWriter(Type type)
  {
    return typeof (NetFormattedStringSerializer).GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic);
  }

  public MethodInfo GetStaticReader(Type type)
  {
    return typeof (NetFormattedStringSerializer).GetMethod("Read", BindingFlags.Static | BindingFlags.NonPublic);
  }

  private static void Write(Stream stream, FormattedString value)
  {
    Primitives.WritePrimitive(stream, value.Markup);
  }

  private static void Read(Stream stream, out FormattedString value)
  {
    string markup;
    Primitives.ReadPrimitive(stream, ref markup);
    value = FormattedString.FromMarkup(markup);
  }
}
