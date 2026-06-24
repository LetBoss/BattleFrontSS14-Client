// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.ShowHealthIconsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Medical.HUD;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Overlays;

public sealed class ShowHealthIconsSystem : EquipmentHudSystem<ShowHealthIconsComponent>
{
  [Dependency]
  private IPrototypeManager _prototypeMan;
  [Dependency]
  private CMHealthIconsSystem _healthIcons;
  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<string> DamageContainers = new HashSet<string>();

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DamageableComponent, GetStatusIconsEvent>(new EntityEventRefHandler<DamageableComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIconsEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ShowHealthIconsComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ShowHealthIconsComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
  }

  protected override void UpdateInternal(
    RefreshEquipmentHudEvent<ShowHealthIconsComponent> component)
  {
    base.UpdateInternal(component);
    foreach (ProtoId<DamageContainerPrototype> protoId in component.Components.SelectMany<ShowHealthIconsComponent, ProtoId<DamageContainerPrototype>>((Func<ShowHealthIconsComponent, IEnumerable<ProtoId<DamageContainerPrototype>>>) (x => (IEnumerable<ProtoId<DamageContainerPrototype>>) x.DamageContainers)))
      this.DamageContainers.Add(ProtoId<DamageContainerPrototype>.op_Implicit(protoId));
  }

  protected override void DeactivateInternal()
  {
    base.DeactivateInternal();
    this.DamageContainers.Clear();
  }

  private void OnHandleState(
    Entity<ShowHealthIconsComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.RefreshOverlay();
  }

  private void OnGetStatusIconsEvent(
    Entity<DamageableComponent> entity,
    ref GetStatusIconsEvent args)
  {
    if (!this.IsActive)
      return;
    IReadOnlyList<StatusIconData> icons = this._healthIcons.GetIcons(entity);
    args.StatusIcons.AddRange((IEnumerable<StatusIconData>) icons);
  }

  private IReadOnlyList<HealthIconPrototype> DecideHealthIcons(Entity<DamageableComponent> entity)
  {
    DamageableComponent comp = entity.Comp;
    if (comp.DamageContainerID.HasValue)
    {
      HashSet<string> damageContainers = this.DamageContainers;
      ProtoId<DamageContainerPrototype>? damageContainerId1 = comp.DamageContainerID;
      string str = damageContainerId1.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerId1.GetValueOrDefault()) : (string) null;
      if (damageContainers.Contains(str))
      {
        List<HealthIconPrototype> healthIconPrototypeList = new List<HealthIconPrototype>();
        ProtoId<DamageContainerPrototype>? damageContainerId2 = (ProtoId<DamageContainerPrototype>?) comp?.DamageContainerID;
        ProtoId<DamageContainerPrototype>? nullable = ProtoId<DamageContainerPrototype>.op_Implicit("Biological");
        MobStateComponent mobStateComponent;
        if ((damageContainerId2.HasValue == nullable.HasValue ? (damageContainerId2.HasValue ? (ProtoId<DamageContainerPrototype>.op_Equality(damageContainerId2.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0 && this.TryComp<MobStateComponent>(Entity<DamageableComponent>.op_Implicit(entity), ref mobStateComponent))
        {
          HealthIconPrototype healthIconPrototype1;
          if (this.HasComp<RottingComponent>(Entity<DamageableComponent>.op_Implicit(entity)) && this._prototypeMan.TryIndex<HealthIconPrototype>(comp.RottingIcon, ref healthIconPrototype1))
          {
            healthIconPrototypeList.Add(healthIconPrototype1);
          }
          else
          {
            ProtoId<HealthIconPrototype> protoId;
            HealthIconPrototype healthIconPrototype2;
            if (comp.HealthIcons.TryGetValue(mobStateComponent.CurrentState, out protoId) && this._prototypeMan.TryIndex<HealthIconPrototype>(protoId, ref healthIconPrototype2))
              healthIconPrototypeList.Add(healthIconPrototype2);
          }
        }
        return (IReadOnlyList<HealthIconPrototype>) healthIconPrototypeList;
      }
    }
    return (IReadOnlyList<HealthIconPrototype>) Array.Empty<HealthIconPrototype>();
  }
}
