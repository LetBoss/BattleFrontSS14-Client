// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Generic.Variables.VarsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Collections;
using System.Linq;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Generic.Variables;

[ToolshedCommand]
public sealed class VarsCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public void Vars(IInvocationContext ctx)
  {
    ctx.WriteLine(this.Toolshed.PrettyPrintType((object) ctx.GetVars().Select<string, string>((Func<string, string>) (x => $"{x} = {this.Toolshed.PrettyPrintType(ctx.ReadVar(x), out IEnumerable _)}")), out IEnumerable _));
  }
}
