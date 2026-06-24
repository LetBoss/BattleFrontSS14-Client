// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Values.IntCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Toolshed.Commands.Values;

[ToolshedCommand(Name = "i")]
public sealed class IntCommand : ToolshedCommand
{
  [CommandImplementation(null)]
  public int Impl(int value) => value;
}
