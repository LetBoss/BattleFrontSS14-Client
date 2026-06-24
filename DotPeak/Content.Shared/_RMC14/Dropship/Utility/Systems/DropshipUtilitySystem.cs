// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dropship.Utility.Systems.DropshipUtilitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Interaction;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Dropship.Utility.Systems;

public sealed class DropshipUtilitySystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedDropshipSystem _dropship;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDropshipWeaponSystem _dropshipWeapon;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, DropshipTargetChangedEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, DropshipTargetChangedEvent>(this.OnTargetChange));
    this.SubscribeLocalEvent<DropshipUtilityPointComponent, InteractHandEvent>(new EntityEventRefHandler<DropshipUtilityPointComponent, InteractHandEvent>(this.OnInteract));
  }

  private void OnTargetChange(
    Entity<DropshipUtilityPointComponent> ent,
    ref DropshipTargetChangedEvent args)
  {
    DropshipUtilityComponent comp;
    if (!this.TryComp<DropshipUtilityComponent>(this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.UtilitySlotId).ContainedEntity, out comp))
      return;
    comp.Target = this.GetEntity(args.DropshipTarget);
  }

  private void OnInteract(Entity<DropshipUtilityPointComponent> ent, ref InteractHandEvent args)
  {
    EntityUid? containedEntity = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.UtilitySlotId).ContainedEntity;
    if (!this.HasComp<DropshipUtilityComponent>(containedEntity))
      return;
    InteractHandEvent args1 = new InteractHandEvent(args.User, args.Target);
    this.RaiseLocalEvent<InteractHandEvent>(containedEntity.Value, args1);
    args.Handled = args1.Handled;
  }

  public bool IsActivatable(Entity<DropshipUtilityComponent> ent, EntityUid user, [NotNullWhen(false)] out string? popup)
  {
    if (ent.Comp.Skills != null && !this._skills.HasSkills((Entity<SkillsComponent>) user, ent.Comp.Skills))
    {
      popup = this.Loc.GetString("rmc-dropship-utility-not-skilled");
      return false;
    }
    Entity<DropshipComponent> dropship;
    if (!this._dropship.TryGetGridDropship((EntityUid) ent, out dropship))
    {
      popup = "";
      return false;
    }
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? nextActivateAt = ent.Comp.NextActivateAt;
    if ((nextActivateAt.HasValue ? (curTime < nextActivateAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
    {
      popup = this.Loc.GetString("rmc-dropship-utility-cooldown", ("utility", (object) ent.Owner));
      return false;
    }
    if (this._dropshipWeapon.CasDebug)
    {
      popup = (string) null;
      return true;
    }
    FTLComponent comp;
    if (!this.TryComp<FTLComponent>((EntityUid) dropship, out comp) || comp.State != FTLState.Travelling && comp.State != FTLState.Arriving)
    {
      popup = this.Loc.GetString("rmc-dropship-utility-activate-not-flying");
      return false;
    }
    if (!ent.Comp.ActivateInTransport && !this.HasComp<DropshipInFlyByComponent>((EntityUid) dropship))
    {
      popup = this.Loc.GetString("rmc-dropship-utility-not-flyby", ("utility", (object) ent.Owner));
      return false;
    }
    popup = (string) null;
    return true;
  }

  public void ResetActivationCooldown(Entity<DropshipUtilityComponent> ent)
  {
    ent.Comp.NextActivateAt = new TimeSpan?(this._timing.CurTime + ent.Comp.ActivateDelay);
  }
}
