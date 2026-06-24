// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.ZoomCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Movement.Systems;
using Content.Shared.Movement.Components;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System.Numerics;

#nullable enable
namespace Content.Client.Commands;

public sealed class ZoomCommand : LocalizedCommands
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IPlayerManager _playerManager;

  public virtual string Command => "zoom";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    bool flag;
    switch (args.Length)
    {
      case 1:
      case 2:
      case 3:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    if (!flag)
    {
      shell.WriteLine(this.Help);
    }
    else
    {
      float result1;
      if (!float.TryParse(args[0], out result1))
        shell.WriteError(this.LocalizationManager.GetString("cmd-parse-failure-float", ("arg", (object) args[0])));
      else if ((double) result1 > 0.0)
      {
        Vector2 zoom = new Vector2(result1, result1);
        if (args.Length == 2)
        {
          float result2;
          if (!float.TryParse(args[1], out result2))
          {
            shell.WriteError(this.LocalizationManager.GetString("cmd-parse-failure-float", ("arg", (object) args[1])));
            return;
          }
          if ((double) result2 > 0.0)
          {
            zoom.Y = result2;
          }
          else
          {
            shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error"));
            return;
          }
        }
        bool result3 = true;
        if (args.Length == 3 && !bool.TryParse(args[2], out result3))
        {
          shell.WriteError(this.LocalizationManager.GetString("cmd-parse-failure-bool", ("arg", (object) args[2])));
        }
        else
        {
          EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._playerManager).LocalSession?.AttachedEntity;
          ContentEyeComponent content;
          if (this._entityManager.TryGetComponent<ContentEyeComponent>(attachedEntity, ref content))
            this._entityManager.System<ContentEyeSystem>().RequestZoom(attachedEntity.Value, zoom, true, result3, content);
          else
            this._eyeManager.CurrentEye.Zoom = zoom;
        }
      }
      else
        shell.WriteError(this.LocalizationManager.GetString($"cmd-{base.Command}-error"));
    }
  }
}
