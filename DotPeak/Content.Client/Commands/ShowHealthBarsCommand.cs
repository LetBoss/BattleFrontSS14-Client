// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.ShowHealthBarsCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Damage.Prototypes;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Commands;

public sealed class ShowHealthBarsCommand : LocalizedEntityCommands
{
  public virtual string Command => "showhealthbars";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    ICommonSession player = shell.Player;
    if (player == null)
    {
      shell.WriteError(((LocalizedCommands) this).Loc.GetString("shell-only-players-can-run-this-command"));
    }
    else
    {
      EntityUid? attachedEntity = player.AttachedEntity;
      if (attachedEntity.HasValue)
      {
        EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
        if (!this.EntityManager.HasComponent<ShowHealthBarsComponent>(valueOrDefault))
        {
          ShowHealthBarsComponent healthBarsComponent1 = new ShowHealthBarsComponent();
          healthBarsComponent1.DamageContainers = ((IEnumerable<string>) args).Select<string, ProtoId<DamageContainerPrototype>>((Func<string, ProtoId<DamageContainerPrototype>>) (arg => new ProtoId<DamageContainerPrototype>(arg))).ToList<ProtoId<DamageContainerPrototype>>();
          healthBarsComponent1.HealthStatusIcon = new ProtoId<HealthIconPrototype>?();
          healthBarsComponent1.NetSyncEnabled = false;
          ShowHealthBarsComponent healthBarsComponent2 = healthBarsComponent1;
          this.EntityManager.AddComponent<ShowHealthBarsComponent>(valueOrDefault, healthBarsComponent2, true, (MetaDataComponent) null);
          shell.WriteLine(((LocalizedCommands) this).Loc.GetString("cmd-showhealthbars-notify-enabled", (nameof (args), (object) string.Join(", ", args))));
        }
        else
        {
          this.EntityManager.RemoveComponentDeferred<ShowHealthBarsComponent>(valueOrDefault);
          shell.WriteLine(((LocalizedCommands) this).Loc.GetString("cmd-showhealthbars-notify-disabled"));
        }
      }
      else
        shell.WriteError(((LocalizedCommands) this).Loc.GetString("shell-must-be-attached-to-entity"));
    }
  }
}
