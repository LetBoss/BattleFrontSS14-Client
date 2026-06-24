// Decompiled with JetBrains decompiler
// Type: Content.Shared.Burial.BurialSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Burial.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Placeable;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Burial;

public sealed class BurialSystem : EntitySystem
{
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedEntityStorageSystem _storageSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, InteractUsingEvent>(new ComponentEventHandler<GraveComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, ActivateInWorldEvent>(new ComponentEventHandler<GraveComponent, ActivateInWorldEvent>((object) this, __methodptr(OnActivate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, AfterInteractUsingEvent>(new ComponentEventHandler<GraveComponent, AfterInteractUsingEvent>((object) this, __methodptr(OnAfterInteractUsing)), new Type[1]
    {
      typeof (PlaceableSurfaceSystem)
    }, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, GraveDiggingDoAfterEvent>(new ComponentEventHandler<GraveComponent, GraveDiggingDoAfterEvent>((object) this, __methodptr(OnGraveDigging)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, StorageOpenAttemptEvent>(new ComponentEventRefHandler<GraveComponent, StorageOpenAttemptEvent>((object) this, __methodptr(OnOpenAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, StorageCloseAttemptEvent>(new ComponentEventRefHandler<GraveComponent, StorageCloseAttemptEvent>((object) this, __methodptr(OnCloseAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, StorageAfterOpenEvent>(new ComponentEventRefHandler<GraveComponent, StorageAfterOpenEvent>((object) this, __methodptr(OnAfterOpen)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, StorageAfterCloseEvent>(new ComponentEventRefHandler<GraveComponent, StorageAfterCloseEvent>((object) this, __methodptr(OnAfterClose)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<GraveComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<GraveComponent, ContainerRelayMovementEntityEvent>((object) this, __methodptr(OnRelayMovement)), (Type[]) null, (Type[]) null);
  }

  private void OnInteractUsing(EntityUid uid, GraveComponent component, InteractUsingEvent args)
  {
    if (args.Handled || component.ActiveShovelDigging)
      return;
    ShovelComponent shovelComponent;
    if (this.TryComp<ShovelComponent>(args.Used, ref shovelComponent))
    {
      DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, args.User, component.DigDelay / (double) shovelComponent.SpeedModifier, (DoAfterEvent) new GraveDiggingDoAfterEvent(), new EntityUid?(uid), new EntityUid?(args.Target), new EntityUid?(uid))
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        NeedHand = true
      };
      if (!component.Stream.HasValue)
      {
        GraveComponent graveComponent = component;
        (EntityUid, AudioComponent)? nullable1 = this._audioSystem.PlayPredicted((SoundSpecifier) component.DigSound, uid, new EntityUid?(args.User), new AudioParams?());
        ref (EntityUid, AudioComponent)? local = ref nullable1;
        EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
        graveComponent.Stream = nullable2;
      }
      if (!this._doAfterSystem.TryStartDoAfter(args1))
      {
        this._audioSystem.Stop(component.Stream, (AudioComponent) null);
        return;
      }
      this.StartDigging(uid, args.User, new EntityUid?(args.Used), component);
    }
    else
      this._popupSystem.PopupClient(this.Loc.GetString("grave-digging-requires-tool", ("grave", (object) args.Target)), uid, new EntityUid?(args.User));
    args.Handled = true;
  }

  private void OnAfterInteractUsing(
    EntityUid uid,
    GraveComponent component,
    AfterInteractUsingEvent args)
  {
    if (args.Handled || !this.HasComp<ShovelComponent>(args.Used))
      return;
    args.Handled = true;
  }

  private void OnActivate(EntityUid uid, GraveComponent component, ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    this._popupSystem.PopupClient(this.Loc.GetString("grave-digging-requires-tool", ("grave", (object) args.Target)), uid, new EntityUid?(args.User));
    args.Handled = true;
  }

  private void OnGraveDigging(
    EntityUid uid,
    GraveComponent component,
    GraveDiggingDoAfterEvent args)
  {
    if (args.Used.HasValue)
    {
      component.ActiveShovelDigging = false;
      component.Stream = this._audioSystem.Stop(component.Stream, (AudioComponent) null);
    }
    else
      component.HandDiggingDoAfter = new DoAfterId?();
    if (args.Cancelled || args.Handled)
      return;
    component.DiggingComplete = true;
    if (args.Used.HasValue)
      this._storageSystem.ToggleOpen(args.User, uid);
    else
      this._storageSystem.TryOpenStorage(args.User, uid);
  }

  private void StartDigging(
    EntityUid uid,
    EntityUid user,
    EntityUid? used,
    GraveComponent component)
  {
    if (used.HasValue)
    {
      this._popupSystem.PopupPredicted(this.Loc.GetString("grave-start-digging-user", ("grave", (object) uid), ("tool", (object) used)), this.Loc.GetString("grave-start-digging-others", new (string, object)[3]
      {
        (nameof (user), (object) user),
        ("grave", (object) uid),
        ("tool", (object) used)
      }), user, new EntityUid?(user));
      component.ActiveShovelDigging = true;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
    else
      this._popupSystem.PopupClient(this.Loc.GetString("grave-start-digging-user-trapped", ("grave", (object) uid)), user, new EntityUid?(user), PopupType.Medium);
  }

  private void OnOpenAttempt(
    EntityUid uid,
    GraveComponent component,
    ref StorageOpenAttemptEvent args)
  {
    if (component.DiggingComplete)
      return;
    args.Cancelled = true;
  }

  private void OnCloseAttempt(
    EntityUid uid,
    GraveComponent component,
    ref StorageCloseAttemptEvent args)
  {
    if (component.DiggingComplete)
      return;
    args.Cancelled = true;
  }

  private void OnAfterOpen(EntityUid uid, GraveComponent component, ref StorageAfterOpenEvent args)
  {
    component.DiggingComplete = false;
  }

  private void OnAfterClose(
    EntityUid uid,
    GraveComponent component,
    ref StorageAfterCloseEvent args)
  {
    component.DiggingComplete = false;
  }

  private void OnRelayMovement(
    EntityUid uid,
    GraveComponent component,
    ref ContainerRelayMovementEntityEvent args)
  {
    if (this._doAfterSystem.IsRunning(component.HandDiggingDoAfter) || !this._actionBlocker.CanMove(args.Entity))
      return;
    DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, args.Entity, component.DigDelay / (double) component.DigOutByHandModifier, (DoAfterEvent) new GraveDiggingDoAfterEvent(), new EntityUid?(uid), new EntityUid?(uid))
    {
      NeedHand = false,
      BreakOnMove = true,
      BreakOnHandChange = false,
      BreakOnDamage = false
    };
    if (!component.Stream.HasValue)
    {
      GraveComponent graveComponent = component;
      (EntityUid, AudioComponent)? nullable1 = this._audioSystem.PlayPredicted((SoundSpecifier) component.DigSound, uid, new EntityUid?(args.Entity), new AudioParams?());
      ref (EntityUid, AudioComponent)? local = ref nullable1;
      EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
      graveComponent.Stream = nullable2;
    }
    if (!this._doAfterSystem.TryStartDoAfter(args1, out component.HandDiggingDoAfter))
      this._audioSystem.Stop(component.Stream, (AudioComponent) null);
    else
      this.StartDigging(uid, args.Entity, new EntityUid?(), component);
  }
}
