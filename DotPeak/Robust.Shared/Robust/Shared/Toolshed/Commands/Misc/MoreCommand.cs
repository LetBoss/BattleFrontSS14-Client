// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.MoreCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
public sealed class MoreCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public object? More(IInvocationContext ctx) => ctx.ReadVar("more");
}
