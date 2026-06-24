// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Configuration.ConfigApplyRollbackCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Robust.Shared.Configuration;

internal sealed class ConfigApplyRollbackCommand : IConsoleCommand
{
  [Dependency]
  private readonly IConfigurationManager _cfg;

  public string Command => "config_rollback_apply";

  public string Description => "";

  public string Help => "";

  public void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    this._cfg.ApplyRollback();
  }
}
