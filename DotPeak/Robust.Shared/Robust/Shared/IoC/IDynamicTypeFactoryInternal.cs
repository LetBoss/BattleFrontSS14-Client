// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.IDynamicTypeFactoryInternal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.IoC;

internal interface IDynamicTypeFactoryInternal : IDynamicTypeFactory
{
  object CreateInstanceUnchecked(Type type, bool oneOff = false, bool inject = true);

  object CreateInstanceUnchecked(Type type, object[] args, bool oneOff = false, bool inject = true);

  T CreateInstanceUnchecked<T>(bool oneOff = false, bool inject = true) where T : new();
}
