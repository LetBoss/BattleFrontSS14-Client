// Decompiled with JetBrains decompiler
// Type: Content.Shared.Revolutionary.SharedRevolutionarySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Antag;
using Content.Shared.IdentityManagement;
using Content.Shared.Mindshield.Components;
using Content.Shared.Popups;
using Content.Shared.Revolutionary.Components;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Shared.Revolutionary;

public abstract class SharedRevolutionarySystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedStunSystem _sharedStun;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MindShieldComponent, MapInitEvent>(new ComponentEventHandler<MindShieldComponent, MapInitEvent>(this.MindShieldImplanted));
    this.SubscribeLocalEvent<RevolutionaryComponent, ComponentGetStateAttemptEvent>(new ComponentEventRefHandler<RevolutionaryComponent, ComponentGetStateAttemptEvent>(this.OnRevCompGetStateAttempt));
    this.SubscribeLocalEvent<HeadRevolutionaryComponent, ComponentGetStateAttemptEvent>(new ComponentEventRefHandler<HeadRevolutionaryComponent, ComponentGetStateAttemptEvent>(this.OnRevCompGetStateAttempt));
    this.SubscribeLocalEvent<RevolutionaryComponent, ComponentStartup>(new ComponentEventHandler<RevolutionaryComponent, ComponentStartup>(this.DirtyRevComps<RevolutionaryComponent>));
    this.SubscribeLocalEvent<HeadRevolutionaryComponent, ComponentStartup>(new ComponentEventHandler<HeadRevolutionaryComponent, ComponentStartup>(this.DirtyRevComps<HeadRevolutionaryComponent>));
    this.SubscribeLocalEvent<ShowAntagIconsComponent, ComponentStartup>(new ComponentEventHandler<ShowAntagIconsComponent, ComponentStartup>(this.DirtyRevComps<ShowAntagIconsComponent>));
  }

  private void MindShieldImplanted(EntityUid uid, MindShieldComponent comp, MapInitEvent init)
  {
    if (this.HasComp<HeadRevolutionaryComponent>(uid))
    {
      this.RemCompDeferred<MindShieldComponent>(uid);
    }
    else
    {
      if (!this.HasComp<RevolutionaryComponent>(uid))
        return;
      TimeSpan time = TimeSpan.FromSeconds(4L);
      EntityUid entityUid = Identity.Entity(uid, (IEntityManager) this.EntityManager);
      this.RemComp<RevolutionaryComponent>(uid);
      this._sharedStun.TryParalyze(uid, time, true);
      this._popupSystem.PopupEntity(this.Loc.GetString("rev-break-control", ("name", (object) entityUid)), uid);
    }
  }

  private void OnRevCompGetStateAttempt(
    EntityUid uid,
    HeadRevolutionaryComponent comp,
    ref ComponentGetStateAttemptEvent args)
  {
    args.Cancelled = !this.CanGetState(args.Player);
  }

  private void OnRevCompGetStateAttempt(
    EntityUid uid,
    RevolutionaryComponent comp,
    ref ComponentGetStateAttemptEvent args)
  {
    args.Cancelled = !this.CanGetState(args.Player);
  }

  private bool CanGetState(ICommonSession? player)
  {
    EntityUid? attachedEntity = (EntityUid?) player?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return true;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    return this.HasComp<RevolutionaryComponent>(valueOrDefault) || this.HasComp<HeadRevolutionaryComponent>(valueOrDefault) || this.HasComp<ShowAntagIconsComponent>(valueOrDefault);
  }

  private void DirtyRevComps<T>(EntityUid someUid, T someComp, ComponentStartup ev)
  {
    AllEntityQueryEnumerator<RevolutionaryComponent> entityQueryEnumerator1 = this.AllEntityQuery<RevolutionaryComponent>();
    EntityUid uid1;
    RevolutionaryComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
      this.Dirty(uid1, (IComponent) comp1_1);
    AllEntityQueryEnumerator<HeadRevolutionaryComponent> entityQueryEnumerator2 = this.AllEntityQuery<HeadRevolutionaryComponent>();
    EntityUid uid2;
    HeadRevolutionaryComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
      this.Dirty(uid2, (IComponent) comp1_2);
  }
}
