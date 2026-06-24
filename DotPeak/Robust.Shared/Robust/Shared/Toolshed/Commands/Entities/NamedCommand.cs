// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Entities.NamedCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Entities;

[ToolshedCommand]
internal sealed class NamedCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public IEnumerable<EntityUid> Named([PipedArgument] IEnumerable<EntityUid> input, string regex, [CommandInverted] bool inverted)
  {
    Regex compiled = new Regex($"^{regex}$");
    return input.Where<EntityUid>((Func<EntityUid, bool>) (x => compiled.IsMatch(this.EntName(x)) ^ inverted));
  }
}
