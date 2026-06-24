// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.AtvModeCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Atmos.EntitySystems;
using Content.Shared.Atmos;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Commands;

internal sealed class AtvModeCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _entitySystemManager;

  public virtual string Command => "atvmode";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length < 1)
    {
      shell.WriteLine(base.Help);
    }
    else
    {
      AtmosDebugOverlayMode result;
      if (!Enum.TryParse<AtmosDebugOverlayMode>(args[0], out result))
      {
        shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error-invalid"));
      }
      else
      {
        int x = 0;
        float num1 = 0.0f;
        float num2 = 207.855988f;
        if (result == AtmosDebugOverlayMode.GasMoles)
        {
          if (args.Length != 2)
          {
            shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error-target-gas"));
            return;
          }
          if (!AtmosCommandUtils.TryParseGasID(args[1], out x))
          {
            shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error-out-of-range"));
            return;
          }
        }
        else
        {
          if (args.Length != 1)
          {
            shell.WriteLine(this.LocalizationManager.GetString($"cmd-{base.Command}-error-info"));
            return;
          }
          if (result == AtmosDebugOverlayMode.Temperature)
          {
            num1 = 373.15f;
            num2 = -160f;
          }
        }
        AtmosDebugOverlaySystem entitySystem = this._entitySystemManager.GetEntitySystem<AtmosDebugOverlaySystem>();
        entitySystem.CfgMode = result;
        entitySystem.CfgSpecificGas = x;
        entitySystem.CfgBase = num1;
        entitySystem.CfgScale = num2;
      }
    }
  }
}
