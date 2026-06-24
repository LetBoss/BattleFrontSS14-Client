// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.Binding.ICommandBindRegistry
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Input.Binding;

[NotContentImplementable]
public interface ICommandBindRegistry
{
  void Register<TOwner>(CommandBinds commandBinds);

  void Register(CommandBinds commandBinds, Type owner);

  IEnumerable<InputCmdHandler> GetHandlers(BoundKeyFunction function);

  void Unregister(Type owner);

  void Unregister<TOwner>();
}
