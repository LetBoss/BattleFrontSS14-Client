// Decompiled with JetBrains decompiler
// Type: Robust.Shared.IoC.IDynamicTypeFactory
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.IoC;

[NotContentImplementable]
public interface IDynamicTypeFactory
{
  object CreateInstance(Type type, bool oneOff = false, bool inject = true);

  object CreateInstance(Type type, object[] args, bool oneOff = false, bool inject = true);

  T CreateInstance<T>(bool oneOff = false, bool inject = true) where T : new();
}
