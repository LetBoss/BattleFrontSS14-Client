// Decompiled with JetBrains decompiler
// Type: Content.Shared.Sound.SharedEmitSoundSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Audio;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Sound.Components;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Sound;

public abstract class SharedEmitSoundSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  protected IRobustRandom Random;
  [Dependency]
  private SharedAmbientSoundSystem _ambient;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private TurfSystem _turf;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EmitSoundOnSpawnComponent, MapInitEvent>(new ComponentEventHandler<EmitSoundOnSpawnComponent, MapInitEvent>(this.OnEmitSpawnOnInit));
    this.SubscribeLocalEvent<EmitSoundOnLandComponent, LandEvent>(new ComponentEventRefHandler<EmitSoundOnLandComponent, LandEvent>(this.OnEmitSoundOnLand));
    this.SubscribeLocalEvent<EmitSoundOnUseComponent, UseInHandEvent>(new ComponentEventHandler<EmitSoundOnUseComponent, UseInHandEvent>(this.OnEmitSoundOnUseInHand));
    this.SubscribeLocalEvent<EmitSoundOnThrowComponent, ThrownEvent>(new ComponentEventRefHandler<EmitSoundOnThrowComponent, ThrownEvent>(this.OnEmitSoundOnThrown));
    this.SubscribeLocalEvent<EmitSoundOnActivateComponent, ActivateInWorldEvent>(new ComponentEventHandler<EmitSoundOnActivateComponent, ActivateInWorldEvent>(this.OnEmitSoundOnActivateInWorld));
    this.SubscribeLocalEvent<EmitSoundOnPickupComponent, GotEquippedHandEvent>(new ComponentEventHandler<EmitSoundOnPickupComponent, GotEquippedHandEvent>(this.OnEmitSoundOnPickup));
    this.SubscribeLocalEvent<EmitSoundOnDropComponent, DroppedEvent>(new ComponentEventHandler<EmitSoundOnDropComponent, DroppedEvent>(this.OnEmitSoundOnDrop));
    this.SubscribeLocalEvent<EmitSoundOnInteractUsingComponent, InteractUsingEvent>(new EntityEventRefHandler<EmitSoundOnInteractUsingComponent, InteractUsingEvent>(this.OnEmitSoundOnInteractUsing));
    this.SubscribeLocalEvent<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>(new ComponentEventHandler<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>(this.HandleEmitSoundOnUIOpen));
    this.SubscribeLocalEvent<EmitSoundOnCollideComponent, StartCollideEvent>(new ComponentEventRefHandler<EmitSoundOnCollideComponent, StartCollideEvent>(this.OnEmitSoundOnCollide));
    this.SubscribeLocalEvent<SoundWhileAliveComponent, MobStateChangedEvent>(new EntityEventRefHandler<SoundWhileAliveComponent, MobStateChangedEvent>(this.OnMobState));
    SubscribeEmitComponent<EmitSoundOnActivateComponent>();
    SubscribeEmitComponent<EmitSoundOnCollideComponent>();
    SubscribeEmitComponent<EmitSoundOnDropComponent>();
    SubscribeEmitComponent<EmitSoundOnInteractUsingComponent>();
    SubscribeEmitComponent<EmitSoundOnLandComponent>();
    SubscribeEmitComponent<EmitSoundOnPickupComponent>();
    SubscribeEmitComponent<EmitSoundOnSpawnComponent>();
    SubscribeEmitComponent<EmitSoundOnThrowComponent>();
    SubscribeEmitComponent<EmitSoundOnUIOpenComponent>();
    SubscribeEmitComponent<EmitSoundOnUseComponent>();

    void SubscribeEmitComponent<T>() where T : notnull, BaseEmitSoundComponent
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.SubscribeLocalEvent<T, ComponentGetState>(SharedEmitSoundSystem.\u003CSubscribeEmitComponent\u003EO__9_0<T>.\u003C0\u003E__GetBaseEmitState ?? (SharedEmitSoundSystem.\u003CSubscribeEmitComponent\u003EO__9_0<T>.\u003C0\u003E__GetBaseEmitState = new EntityEventRefHandler<T, ComponentGetState>(SharedEmitSoundSystem.GetBaseEmitState<T>)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.SubscribeLocalEvent<T, ComponentHandleState>(SharedEmitSoundSystem.\u003CSubscribeEmitComponent\u003EO__9_0<T>.\u003C1\u003E__HandleBaseEmitState ?? (SharedEmitSoundSystem.\u003CSubscribeEmitComponent\u003EO__9_0<T>.\u003C1\u003E__HandleBaseEmitState = new EntityEventRefHandler<T, ComponentHandleState>(SharedEmitSoundSystem.HandleBaseEmitState<T>)));
    }
  }

  private static void GetBaseEmitState<T>(Entity<T> ent, ref ComponentGetState args) where T : BaseEmitSoundComponent
  {
    args.State = (IComponentState) new EmitSoundComponentState(ent.Comp.Sound);
  }

  private static void HandleBaseEmitState<T>(Entity<T> ent, ref ComponentHandleState args) where T : BaseEmitSoundComponent
  {
    if (!(args.Current is EmitSoundComponentState current))
      return;
    T comp = ent.Comp;
    SoundSpecifier soundSpecifier;
    switch (current.Sound)
    {
      case SoundPathSpecifier soundPathSpecifier:
        soundSpecifier = (SoundSpecifier) new SoundPathSpecifier(soundPathSpecifier.Path, new AudioParams?(soundPathSpecifier.Params));
        break;
      case SoundCollectionSpecifier collectionSpecifier:
        soundSpecifier = collectionSpecifier.Collection != null ? (SoundSpecifier) new SoundCollectionSpecifier(collectionSpecifier.Collection, new AudioParams?(collectionSpecifier.Params)) : (SoundSpecifier) null;
        break;
      default:
        soundSpecifier = (SoundSpecifier) null;
        break;
    }
    comp.Sound = soundSpecifier;
  }

  private void HandleEmitSoundOnUIOpen(
    EntityUid uid,
    EmitSoundOnUIOpenComponent component,
    AfterActivatableUIOpenEvent args)
  {
    if (!this._whitelistSystem.IsBlacklistFail(component.Blacklist, args.User))
      return;
    this.TryEmitSound(uid, (BaseEmitSoundComponent) component, new EntityUid?(args.User));
  }

  private void OnMobState(Entity<SoundWhileAliveComponent> entity, ref MobStateChangedEvent args)
  {
    SpamEmitSoundComponent comp;
    if (this.TryComp<SpamEmitSoundComponent>((EntityUid) entity, out comp))
    {
      comp.Enabled = args.NewMobState == MobState.Alive;
      this.Dirty(entity.Owner, (IComponent) comp);
    }
    this._ambient.SetAmbience(entity.Owner, args.NewMobState != MobState.Dead);
  }

  private void OnEmitSpawnOnInit(
    EntityUid uid,
    EmitSoundOnSpawnComponent component,
    MapInitEvent args)
  {
    this.TryEmitSound(uid, (BaseEmitSoundComponent) component, predict: false);
  }

  private void OnEmitSoundOnLand(
    EntityUid uid,
    BaseEmitSoundComponent component,
    ref LandEvent args)
  {
    TransformComponent comp1;
    MapGridComponent comp2;
    if (!args.PlaySound || !this.TryComp(uid, out comp1) || !this.TryComp<MapGridComponent>(comp1.GridUid, out comp2))
      return;
    TileRef tileRef = this._map.GetTileRef(comp1.GridUid.Value, comp2, comp1.Coordinates);
    EntityUid? gridUid = comp1.GridUid;
    EntityUid? mapUid = comp1.MapUid;
    if ((gridUid.HasValue == mapUid.HasValue ? (gridUid.HasValue ? (gridUid.GetValueOrDefault() != mapUid.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0 && this._turf.IsSpace(tileRef))
      return;
    this.TryEmitSound(uid, component, args.User, false);
  }

  private void OnEmitSoundOnUseInHand(
    EntityUid uid,
    EmitSoundOnUseComponent component,
    UseInHandEvent args)
  {
    this.TryEmitSound(uid, (BaseEmitSoundComponent) component, new EntityUid?(args.User));
    if (!component.Handle)
      return;
    args.Handled = true;
  }

  private void OnEmitSoundOnThrown(
    EntityUid uid,
    BaseEmitSoundComponent component,
    ref ThrownEvent args)
  {
    this.TryEmitSound(uid, component, args.User, false);
  }

  private void OnEmitSoundOnActivateInWorld(
    EntityUid uid,
    EmitSoundOnActivateComponent component,
    ActivateInWorldEvent args)
  {
    this.TryEmitSound(uid, (BaseEmitSoundComponent) component, new EntityUid?(args.User));
    if (!component.Handle)
      return;
    args.Handled = true;
  }

  private void OnEmitSoundOnPickup(
    EntityUid uid,
    EmitSoundOnPickupComponent component,
    GotEquippedHandEvent args)
  {
    this.TryEmitSound(uid, (BaseEmitSoundComponent) component, new EntityUid?(args.User));
  }

  private void OnEmitSoundOnDrop(
    EntityUid uid,
    EmitSoundOnDropComponent component,
    DroppedEvent args)
  {
    this.TryEmitSound(uid, (BaseEmitSoundComponent) component, new EntityUid?(args.User));
  }

  private void OnEmitSoundOnInteractUsing(
    Entity<EmitSoundOnInteractUsingComponent> ent,
    ref InteractUsingEvent args)
  {
    if (!this._whitelistSystem.IsWhitelistPass(ent.Comp.Whitelist, args.Used))
      return;
    this.TryEmitSound((EntityUid) ent, (BaseEmitSoundComponent) ent.Comp, new EntityUid?(args.User));
  }

  public void TryEmitSound(
    EntityUid uid,
    BaseEmitSoundComponent component,
    EntityUid? user = null,
    bool predict = true)
  {
    if (component.Sound == null)
      return;
    if (component.Positional)
    {
      EntityCoordinates coordinates = this.Transform(uid).Coordinates;
      if (predict)
      {
        this._audioSystem.PlayPredicted(component.Sound, coordinates, user);
      }
      else
      {
        if (!this._netMan.IsServer)
          return;
        this._audioSystem.PlayPvs(component.Sound, coordinates);
      }
    }
    else if (predict)
    {
      this._audioSystem.PlayPredicted(component.Sound, uid, user);
    }
    else
    {
      if (!this._netMan.IsServer)
        return;
      this._audioSystem.PlayPvs(component.Sound, uid);
    }
  }

  private void OnEmitSoundOnCollide(
    EntityUid uid,
    EmitSoundOnCollideComponent component,
    ref StartCollideEvent args)
  {
    PhysicsComponent comp;
    if (!args.OurFixture.Hard || !args.OtherFixture.Hard || !this.TryComp<PhysicsComponent>(uid, out comp) || (double) comp.LinearVelocity.Length() < (double) component.MinimumVelocity || this.Timing.CurTime < component.NextSound || this.MetaData(uid).EntityPaused)
      return;
    float volume = (float) (12.0 * (double) MathF.Min(1f, (float) (((double) comp.LinearVelocity.Length() - (double) component.MinimumVelocity) / 10.0)) - 10.0);
    component.NextSound = this.Timing.CurTime + EmitSoundOnCollideComponent.CollideCooldown;
    SoundSpecifier sound = component.Sound;
    if (!this._netMan.IsServer || sound == null)
      return;
    this._audioSystem.PlayPvs(this._audioSystem.ResolveSound(sound), uid, new AudioParams?(AudioParams.Default.WithVolume(volume)));
  }

  public virtual void SetEnabled(Entity<SpamEmitSoundComponent?> entity, bool enabled)
  {
  }
}
