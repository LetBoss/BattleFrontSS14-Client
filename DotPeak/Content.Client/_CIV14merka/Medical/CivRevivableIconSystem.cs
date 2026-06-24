// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Medical.CivRevivableIconSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Medical;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Medical;

public sealed class CivRevivableIconSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _players;
  private static readonly ProtoId<HealthIconPrototype> IconId = ProtoId<HealthIconPrototype>.op_Implicit("HealthIconCivRevivable");

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivDeathTimeComponent, GetStatusIconsEvent>(new EntityEventRefHandler<CivDeathTimeComponent, GetStatusIconsEvent>((object) this, __methodptr(OnGetStatusIcons)), (Type[]) null, (Type[]) null);
  }

  private void OnGetStatusIcons(Entity<CivDeathTimeComponent> ent, ref GetStatusIconsEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._players).LocalEntity;
    CivTeamMemberComponent teamMemberComponent1;
    CivTeamMemberComponent teamMemberComponent2;
    MobStateComponent mobStateComponent;
    CivRevivedTrackerComponent trackerComponent;
    HealthIconPrototype healthIconPrototype;
    if (!localEntity.HasValue || !this.TryComp<CivTeamMemberComponent>(localEntity, ref teamMemberComponent1) || teamMemberComponent1.TeamId <= 0 || !this.TryComp<CivTeamMemberComponent>(Entity<CivDeathTimeComponent>.op_Implicit(ent), ref teamMemberComponent2) || teamMemberComponent1.TeamId != teamMemberComponent2.TeamId || !this.TryComp<MobStateComponent>(Entity<CivDeathTimeComponent>.op_Implicit(ent), ref mobStateComponent) || mobStateComponent.CurrentState != MobState.Dead || ent.Comp.DeathTime == TimeSpan.Zero || this._timing.CurTime - ent.Comp.DeathTime > TimeSpan.FromMinutes(4.0) || this.TryComp<CivRevivedTrackerComponent>(Entity<CivDeathTimeComponent>.op_Implicit(ent), ref trackerComponent) && trackerComponent.RevivedCount >= trackerComponent.MaxRevives || !this._prototype.TryIndex<HealthIconPrototype>(CivRevivableIconSystem.IconId, ref healthIconPrototype))
      return;
    args.StatusIcons.Add((StatusIconData) healthIconPrototype);
  }
}
