// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.IoCCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class IoCCommand : ToolshedCommand
{
  [CommandImplementation("registered")]
  public IEnumerable<Type> Registered() => IoCManager.Instance.GetRegisteredTypes();

  [CommandImplementation("get")]
  public object? Get([PipedArgument] Type t) => IoCManager.ResolveType(t);
}
