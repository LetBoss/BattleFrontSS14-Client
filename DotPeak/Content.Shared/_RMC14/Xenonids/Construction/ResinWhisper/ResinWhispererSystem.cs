// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.ResinWhisper.ResinWhispererSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Construction.Events;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Database;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;

public sealed class ResinWhispererSystem : EntitySystem
{
  [Dependency]
  private SharedDoorSystem _door;
  [Dependency]
  private ExamineSystemShared _examineSystem;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedXenoWeedsSystem _weeds;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ResinDoorComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<ResinDoorComponent, GetVerbsEvent<AlternativeVerb>>(this.OnDoorAltVerb));
    this.SubscribeLocalEvent<ResinWhispererComponent, XenoSecreteStructureAdjustFields>(new EntityEventRefHandler<ResinWhispererComponent, XenoSecreteStructureAdjustFields>(this.OnRemoteSecreteStructure));
    this.SubscribeLocalEvent<ResinWhispererComponent, InRangeOverrideEvent>(new EntityEventRefHandler<ResinWhispererComponent, InRangeOverrideEvent>(this.OnInRangeOverride));
  }

  private void OnDoorAltVerb(
    Entity<ResinDoorComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!this.HasComp<ResinWhispererComponent>(args.User))
      return;
    EntityUid target = args.Target;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = "Open Door";
    alternativeVerb.Impact = LogImpact.Low;
    alternativeVerb.Act = (Action) (() =>
    {
      DoorComponent comp;
      if (!this.CanRemoteOpenDoorPopup((Entity<ResinWhispererComponent>) user, target) || !this.TryComp<DoorComponent>(target, out comp) || !this._door.TryToggleDoor(target, predicted: true))
        return;
      if (comp.State == DoorState.Opening)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-remote-open-door"), user, new EntityUid?(user));
      if (comp.State != DoorState.Closing)
        return;
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-remote-close-door"), user, new EntityUid?(user));
    });
    alternativeVerb.Priority = 100;
    verbs.Add(alternativeVerb);
  }

  private bool CanRemoteOpenDoorPopup(
    Entity<ResinWhispererComponent?> user,
    EntityUid target,
    bool doPopup = true)
  {
    if (!this.Resolve<ResinWhispererComponent>((EntityUid) user, ref user.Comp, false))
      return false;
    if (!this._weeds.IsOnFriendlyWeeds((Entity<TransformComponent>) user.Owner))
    {
      if (doPopup)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-remote-failed-need-on-weeds"), (EntityUid) user, new EntityUid?((EntityUid) user));
      return false;
    }
    return this.HasComp<DoorComponent>(target) && this.HasComp<ResinDoorComponent>(target);
  }

  private void OnRemoteSecreteStructure(
    Entity<ResinWhispererComponent> ent,
    ref XenoSecreteStructureAdjustFields args)
  {
    XenoConstructionComponent comp;
    if (!this.TryComp<XenoConstructionComponent>((EntityUid) ent, out comp))
      return;
    if (ent.Comp.StandardConstructDelay.HasValue)
      comp.BuildDelay = ent.Comp.StandardConstructDelay.Value;
    else
      ent.Comp.StandardConstructDelay = new TimeSpan?(comp.BuildDelay);
    if (ent.Comp.MaxConstructDistance.HasValue)
      comp.BuildRange = ent.Comp.MaxConstructDistance.Value;
    else
      ent.Comp.MaxConstructDistance = new FixedPoint2?(comp.BuildRange);
    if (this._interaction.InRangeUnobstructed((EntityUid) ent, args.TargetCoordinates, ent.Comp.MaxConstructDistance.Value.Float()))
      return;
    if (!this.TileIsVisible(ent, args.TargetCoordinates))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-remote-failed-need-line-of-sight"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    else if (!this._weeds.IsOnFriendlyWeeds((Entity<TransformComponent>) ent.Owner))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-construction-remote-failed-need-on-weeds"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    }
    else
    {
      comp.BuildDelay = ent.Comp.StandardConstructDelay.Value.Multiply((double) ent.Comp.RemoteConstructDelayMultiplier);
      comp.BuildRange = (FixedPoint2) ent.Comp.MaxRemoteConstructDistance;
    }
  }

  private void OnInRangeOverride(Entity<ResinWhispererComponent> ent, ref InRangeOverrideEvent args)
  {
    if (!this.CanRemoteOpenDoorPopup((Entity<ResinWhispererComponent>) ent.Owner, args.Target, false))
      return;
    args.InRange = true;
    args.Handled = true;
  }

  private bool TileIsVisible(
    Entity<ResinWhispererComponent> ent,
    EntityCoordinates targetCoordinates)
  {
    MapCoordinates other = this._transform.ToMapCoordinates(targetCoordinates);
    for (int index = 0; index < 9; ++index)
    {
      switch (index - 1)
      {
        case 0:
        case 6:
        case 7:
          other = other.Offset(0.499f, 0.0f);
          break;
        case 1:
          other = other.Offset(0.0f, -0.499f);
          break;
        case 2:
        case 3:
          other = other.Offset(-0.499f, 0.0f);
          break;
        case 4:
        case 5:
          other = other.Offset(0.0f, 0.499f);
          break;
      }
      if (this._examineSystem.InRangeUnOccluded((EntityUid) ent, other, ent.Comp.MaxRemoteConstructDistance))
        return true;
    }
    return false;
  }
}
