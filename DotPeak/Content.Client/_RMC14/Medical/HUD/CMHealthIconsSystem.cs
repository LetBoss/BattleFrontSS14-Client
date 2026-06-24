// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Medical.HUD.CMHealthIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Connection;
using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared.Damage;
using Content.Shared.Mobs.Systems;
using Content.Shared.StatusIcon;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Medical.HUD;

public sealed class CMHealthIconsSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private RMCUnrevivableSystem _unrevivable;
  private static readonly ProtoId<HealthIconPrototype> BaseDeadIcon = ProtoId<HealthIconPrototype>.op_Implicit("CMHealthIconDead");

  public StatusIconData GetDeadIcon()
  {
    return (StatusIconData) this._prototype.Index<HealthIconPrototype>(CMHealthIconsSystem.BaseDeadIcon);
  }

  public IReadOnlyList<StatusIconData> GetIcons(Entity<DamageableComponent> damageable)
  {
    List<StatusIconData> icons = new List<StatusIconData>();
    RMCHealthIconTypes key = RMCHealthIconTypes.Healthy;
    RMCHealthIconsComponent healthIconsComponent;
    if (!this.TryComp<RMCHealthIconsComponent>(Entity<DamageableComponent>.op_Implicit(damageable), ref healthIconsComponent))
      return (IReadOnlyList<StatusIconData>) icons;
    if (this._mobState.IsDead(Entity<DamageableComponent>.op_Implicit(damageable)))
    {
      int unrevivableStage = this._unrevivable.GetUnrevivableStage(Entity<RMCRevivableComponent>.op_Implicit(damageable.Owner), 4);
      if (this._unrevivable.IsUnrevivable(Entity<DamageableComponent>.op_Implicit(damageable)))
      {
        key = RMCHealthIconTypes.Dead;
      }
      else
      {
        MindCheckComponent mindCheckComponent;
        if (this.TryComp<MindCheckComponent>(Entity<DamageableComponent>.op_Implicit(damageable), ref mindCheckComponent) && !mindCheckComponent.ActiveMindOrGhost)
          key = RMCHealthIconTypes.DeadDNR;
        else if (unrevivableStage <= 1)
          key = RMCHealthIconTypes.DeadDefib;
        else if (unrevivableStage == 2)
          key = RMCHealthIconTypes.DeadClose;
        else if (unrevivableStage == 3)
          key = RMCHealthIconTypes.DeadAlmost;
      }
    }
    ProtoId<HealthIconPrototype> protoId;
    if (healthIconsComponent.Icons.TryGetValue(key, out protoId))
      icons.Add((StatusIconData) this._prototype.Index<HealthIconPrototype>(protoId));
    return (IReadOnlyList<StatusIconData>) icons;
  }
}
