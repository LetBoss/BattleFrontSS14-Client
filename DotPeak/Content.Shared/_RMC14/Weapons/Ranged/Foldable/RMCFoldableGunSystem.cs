// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Foldable.RMCFoldableGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Common;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Foldable;

public sealed class RMCFoldableGunSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedHandsSystem _hands;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCFoldableGunComponent, ExaminedEvent>(new EntityEventRefHandler<RMCFoldableGunComponent, ExaminedEvent>(this.OnExamined), new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<RMCFoldableGunComponent, GunShotEvent>(new EntityEventRefHandler<RMCFoldableGunComponent, GunShotEvent>(this.OnGunShoot));
    this.SubscribeLocalEvent<RMCFoldableGunComponent, AttemptShootEvent>(new EntityEventRefHandler<RMCFoldableGunComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<RMCFoldableGunComponent, UniqueActionEvent>(new EntityEventRefHandler<RMCFoldableGunComponent, UniqueActionEvent>(this.OnUniqueAction));
    this.SubscribeLocalEvent<RMCFoldableGunComponent, ActivateInWorldEvent>(new EntityEventRefHandler<RMCFoldableGunComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<RMCFoldableGunComponent, RMCFoldableGunDoAfterEvent>(new EntityEventRefHandler<RMCFoldableGunComponent, RMCFoldableGunDoAfterEvent>(this.OnFoldableGunDoAfter));
  }

  private void OnExamined(Entity<RMCFoldableGunComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString((string) ent.Comp.ExamineText), 1);
  }

  private void OnGunShoot(Entity<RMCFoldableGunComponent> ent, ref GunShotEvent args)
  {
    ent.Comp.Fired = true;
  }

  private void OnAttemptShoot(Entity<RMCFoldableGunComponent> ent, ref AttemptShootEvent args)
  {
    if (args.Cancelled || !ent.Comp.Fired)
      return;
    args.Cancelled = true;
  }

  private void OnUniqueAction(Entity<RMCFoldableGunComponent> ent, ref UniqueActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.AttemptFold(ent, args.UserUid);
  }

  private void OnActivate(Entity<RMCFoldableGunComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !ent.Comp.OnActivate)
      return;
    EntityUid user = args.User;
    if (!this._hands.IsHolding((Entity<HandsComponent>) user, new EntityUid?(ent.Owner)))
      return;
    args.Handled = this.AttemptFold(ent, user);
  }

  private void OnFoldableGunDoAfter(
    Entity<RMCFoldableGunComponent> ent,
    ref RMCFoldableGunDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    EntityUid user = args.User;
    if (args.Handled)
      return;
    string recipientMessage = this.Loc.GetString((string) ent.Comp.FinishText, ("weapon", (object) ent));
    string othersMessage = this.Loc.GetString((string) ent.Comp.FinishTextOthers, ("user", (object) user), ("weapon", (object) ent));
    string activeHand = this._hands.GetActiveHand((Entity<HandsComponent>) user);
    if (activeHand == null)
      return;
    this._hands.TryForcePickup((Entity<HandsComponent>) this.PredictedSpawnNextToOrDrop((string) ent.Comp.FoldedEntity, user), user, activeHand, false);
    this._popup.PopupPredicted(recipientMessage, othersMessage, user, new EntityUid?(user));
    this._audio.PlayPredicted(ent.Comp.ToggleFoldSound, user, new EntityUid?(user));
    this.PredictedQueueDel(ent.Owner);
    args.Handled = true;
  }

  public bool AttemptFold(Entity<RMCFoldableGunComponent> ent, EntityUid user)
  {
    if (ent.Comp.Fired)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-gun-foldable-launcher-fold-already-fired-attempt", ("weapon", (object) ent)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    RMCFoldableGunDoAfterEvent @event = new RMCFoldableGunDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.FoldDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true,
      BreakOnDamage = false,
      MovementThreshold = 0.5f,
      DuplicateCondition = DuplicateConditions.SameEvent,
      CancelDuplicate = true,
      NeedHand = true,
      BreakOnDropItem = true
    }))
      return false;
    this._popup.PopupPredicted(this.Loc.GetString((string) ent.Comp.FoldText, ("weapon", (object) ent)), this.Loc.GetString((string) ent.Comp.FoldTextOthers, (nameof (user), (object) user), ("weapon", (object) ent)), user, new EntityUid?(user));
    return true;
  }
}
