// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.SaveConfig
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Configuration;
using Robust.Shared.IoC;

#nullable enable
namespace Robust.Shared.Console.Commands;

public sealed class SaveConfig : LocalizedCommands
{
  [Dependency]
  private readonly IConfigurationManager _cfg;

  public override string Command => "saveconfig";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    this._cfg.SaveToFile();
  }
}
