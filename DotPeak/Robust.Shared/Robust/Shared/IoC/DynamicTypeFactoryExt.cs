// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.DynamicTypeFactoryExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.IoC;

public static class DynamicTypeFactoryExt
{
  public static T CreateInstance<T>(
    this IDynamicTypeFactory dynamicTypeFactory,
    Type type,
    bool oneOff = false,
    bool inject = true)
  {
    return (T) dynamicTypeFactory.CreateInstance(type, oneOff, inject);
  }

  public static T CreateInstance<T>(
    this IDynamicTypeFactory dynamicTypeFactory,
    Type type,
    object[] args,
    bool oneOff = false,
    bool inject = true)
  {
    return (T) dynamicTypeFactory.CreateInstance(type, args, oneOff, inject);
  }

  internal static T CreateInstanceUnchecked<T>(
    this IDynamicTypeFactoryInternal dynamicTypeFactory,
    Type type,
    bool oneOff = false,
    bool inject = true)
  {
    return (T) dynamicTypeFactory.CreateInstanceUnchecked(type, oneOff, inject);
  }

  internal static T CreateInstanceUnchecked<T>(
    this IDynamicTypeFactoryInternal dynamicTypeFactory,
    Type type,
    object[] args,
    bool oneOff = false,
    bool inject = true)
  {
    return (T) dynamicTypeFactory.CreateInstanceUnchecked(type, args, oneOff, inject);
  }
}
