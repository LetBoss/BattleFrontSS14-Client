// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.EntitySystems.ShakeableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Nutrition.EntitySystems;

public sealed class ShakeableSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ShakeableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<ShakeableComponent, GetVerbsEvent<Verb>>(this.AddShakeVerb));
    this.SubscribeLocalEvent<ShakeableComponent, ShakeDoAfterEvent>(new EntityEventRefHandler<ShakeableComponent, ShakeDoAfterEvent>(this.OnShakeDoAfter));
  }

  private void AddShakeVerb(EntityUid uid, ShakeableComponent component, GetVerbsEvent<Verb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract || !this.CanShake((Entity<ShakeableComponent>) (uid, component), new EntityUid?(args.User)))
      return;
    Verb verb = new Verb()
    {
      Text = this.Loc.GetString((string) component.ShakeVerbText),
      Act = (Action) (() => this.TryStartShake((Entity<ShakeableComponent>) (args.Target, component), args.User))
    };
    args.Verbs.Add(verb);
  }

  private void OnShakeDoAfter(Entity<ShakeableComponent> entity, ref ShakeDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    this.TryShake((Entity<ShakeableComponent>) ((EntityUid) entity, entity.Comp), new EntityUid?(args.User));
  }

  public bool TryStartShake(Entity<ShakeableComponent?> entity, EntityUid user)
  {
    if (!this.Resolve<ShakeableComponent>((EntityUid) entity, ref entity.Comp) || !this.CanShake(entity, new EntityUid?(user)))
      return false;
    DoAfterArgs args = new DoAfterArgs((IEntityManager) this.EntityManager, user, entity.Comp.ShakeDuration, (DoAfterEvent) new ShakeDoAfterEvent(), new EntityUid?((EntityUid) entity), new EntityUid?(user), new EntityUid?((EntityUid) entity))
    {
      NeedHand = true,
      BreakOnDamage = true,
      DistanceThreshold = new float?(1f),
      MovementThreshold = 0.01f,
      BreakOnHandChange = entity.Comp.RequireInHand
    };
    if (entity.Comp.RequireInHand)
      args.BreakOnHandChange = true;
    if (!this._doAfter.TryStartDoAfter(args))
      return false;
    EntityUid entityUid1 = Identity.Entity(user, (IEntityManager) this.EntityManager);
    EntityUid entityUid2 = Identity.Entity((EntityUid) entity, (IEntityManager) this.EntityManager);
    this._popup.PopupPredicted(this.Loc.GetString((string) entity.Comp.ShakePopupMessageSelf, (nameof (user), (object) entityUid1), ("shakeable", (object) entityUid2)), this.Loc.GetString((string) entity.Comp.ShakePopupMessageOthers, (nameof (user), (object) entityUid1), ("shakeable", (object) entityUid2)), user, new EntityUid?(user));
    this._audio.PlayPredicted(entity.Comp.ShakeSound, (EntityUid) entity, new EntityUid?(user));
    return true;
  }

  public bool TryShake(Entity<ShakeableComponent?> entity, EntityUid? user = null)
  {
    if (!this.Resolve<ShakeableComponent>((EntityUid) entity, ref entity.Comp) || !this.CanShake(entity, user))
      return false;
    ShakeEvent args = new ShakeEvent(user);
    this.RaiseLocalEvent<ShakeEvent>((EntityUid) entity, ref args);
    return true;
  }

  public bool CanShake(Entity<ShakeableComponent?> entity, EntityUid? user = null)
  {
    if (!this.Resolve<ShakeableComponent>((EntityUid) entity, ref entity.Comp, false) || user.HasValue && entity.Comp.RequireInHand && !this._hands.IsHolding((Entity<HandsComponent>) user.Value, new EntityUid?((EntityUid) entity), out string _))
      return false;
    AttemptShakeEvent args = new AttemptShakeEvent();
    this.RaiseLocalEvent<AttemptShakeEvent>((EntityUid) entity, ref args);
    return !args.Cancelled;
  }
}
