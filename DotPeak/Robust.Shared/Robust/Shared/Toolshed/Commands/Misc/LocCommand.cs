// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.Misc.LocCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Toolshed.Commands.Misc;

[ToolshedCommand]
internal sealed class LocCommand : ToolshedCommand
{
  [CommandImplementation("tryloc")]
  public string? TryLocalize([PipedArgument] string str)
  {
    string str1;
    this.Loc.TryGetString(str, out str1);
    return str1;
  }

  [CommandImplementation("loc")]
  public string Localize([PipedArgument] string str) => this.Loc.GetString(str);
}
