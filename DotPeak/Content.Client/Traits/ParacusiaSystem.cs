// Decompiled with JetBrains decompiler
// Type: Content.Client.Traits.ParacusiaSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Traits.Assorted;
using Robust.Client.Player;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Traits;

public sealed class ParacusiaSystem : SharedParacusiaSystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAudioSystem _audio;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ParacusiaComponent, ComponentStartup>(new ComponentEventHandler<ParacusiaComponent, ComponentStartup>((object) this, __methodptr(OnComponentStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ParacusiaComponent, LocalPlayerDetachedEvent>(new ComponentEventHandler<ParacusiaComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetach)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    this.PlayParacusiaSounds(localEntity.GetValueOrDefault());
  }

  private void OnComponentStartup(
    EntityUid uid,
    ParacusiaComponent component,
    ComponentStartup args)
  {
    component.NextIncidentTime = this._timing.CurTime + TimeSpan.FromSeconds((double) this._random.NextFloat(component.MinTimeBetweenIncidents, component.MaxTimeBetweenIncidents));
  }

  private void OnPlayerDetach(
    EntityUid uid,
    ParacusiaComponent component,
    LocalPlayerDetachedEvent args)
  {
    component.Stream = this._audio.Stop(component.Stream, (AudioComponent) null);
  }

  private void PlayParacusiaSounds(EntityUid uid)
  {
    ParacusiaComponent paracusiaComponent1;
    if (!this.TryComp<ParacusiaComponent>(uid, ref paracusiaComponent1) || this._timing.CurTime <= paracusiaComponent1.NextIncidentTime)
      return;
    float num = this._random.NextFloat(paracusiaComponent1.MinTimeBetweenIncidents, paracusiaComponent1.MaxTimeBetweenIncidents);
    paracusiaComponent1.NextIncidentTime += TimeSpan.FromSeconds((double) num);
    Vector2 vector2 = new Vector2(this._random.NextFloat(-paracusiaComponent1.MaxSoundDistance, paracusiaComponent1.MaxSoundDistance), this._random.NextFloat(-paracusiaComponent1.MaxSoundDistance, paracusiaComponent1.MaxSoundDistance));
    EntityCoordinates coordinates = this.Transform(uid).Coordinates;
    EntityCoordinates entityCoordinates = ((EntityCoordinates) ref coordinates).Offset(vector2);
    ParacusiaComponent paracusiaComponent2 = paracusiaComponent1;
    (EntityUid, AudioComponent)? nullable1 = this._audio.PlayStatic(paracusiaComponent1.Sounds, uid, entityCoordinates, new AudioParams?());
    ref (EntityUid, AudioComponent)? local = ref nullable1;
    EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
    paracusiaComponent2.Stream = nullable2;
  }
}
