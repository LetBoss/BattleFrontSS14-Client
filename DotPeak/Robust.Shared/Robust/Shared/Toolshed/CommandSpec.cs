// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.CommandSpec
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.Toolshed.TypeParsers;

#nullable enable
namespace Robust.Shared.Toolshed;

public readonly record struct CommandSpec(ToolshedCommand Cmd, string? SubCommand) : 
  IAsType<ToolshedCommand>
{
  public ToolshedCommand AsType() => this.Cmd;

  public CompletionOption AsCompletion()
  {
    return new CompletionOption(this.FullName(), this.Cmd.Description(this.SubCommand));
  }

  public string FullName()
  {
    return this.SubCommand != null ? $"{this.Cmd.Name}:{this.SubCommand}" : this.Cmd.Name;
  }

  public string DescLocStr() => this.Cmd.DescriptionLocKey(this.SubCommand);

  public override string ToString() => this.FullName();
}
