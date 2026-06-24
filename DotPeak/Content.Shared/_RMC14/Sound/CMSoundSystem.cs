// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sound.CMSoundSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Sound;
using Content.Shared.Sound.Components;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Sound;

public sealed class CMSoundSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedEmitSoundSystem _emitSound;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCEmitSoundOnSpawnComponent, MapInitEvent>(new EntityEventRefHandler<RMCEmitSoundOnSpawnComponent, MapInitEvent>(this.OnEmitSpawnOnInit));
    this.SubscribeLocalEvent<RandomSoundComponent, MapInitEvent>(new EntityEventRefHandler<RandomSoundComponent, MapInitEvent>(this.OnRandomMapInit));
    this.SubscribeLocalEvent<SoundOnDeathComponent, MobStateChangedEvent>(new EntityEventRefHandler<SoundOnDeathComponent, MobStateChangedEvent>(this.OnDeathMobStateChanged));
    this.SubscribeLocalEvent<SoundOnDeathComponent, EntityTerminatingEvent>(new EntityEventRefHandler<SoundOnDeathComponent, EntityTerminatingEvent>(this.OnDeathMobTerminating));
    this.SubscribeLocalEvent<SoundOnDeathSoundComponent, EntityTerminatingEvent>(new EntityEventRefHandler<SoundOnDeathSoundComponent, EntityTerminatingEvent>(this.OnDeathSoundTerminating));
    this.SubscribeLocalEvent<EmitSoundOnActionComponent, SoundActionEvent>(new EntityEventRefHandler<EmitSoundOnActionComponent, SoundActionEvent>(this.OnEmitSoundOnAction));
  }

  private void OnEmitSpawnOnInit(Entity<RMCEmitSoundOnSpawnComponent> ent, ref MapInitEvent args)
  {
    if (this._net.IsClient || ent.Comp.Sound == null)
      return;
    RMCEmitSoundOnSpawnComponent comp = ent.Comp;
    (EntityUid, AudioComponent)? nullable1 = this._audio.PlayPvs(ent.Comp.Sound, ent.Owner);
    ref (EntityUid, AudioComponent)? local = ref nullable1;
    EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
    comp.Entity = nullable2;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent);
    if (this.TerminatingOrDeleted(moverCoordinates.EntityId) || !ent.Comp.Entity.HasValue)
      return;
    this._transform.SetCoordinates(ent.Comp.Entity.Value, moverCoordinates);
    this.QueueDel(new EntityUid?(ent.Owner));
  }

  private void OnRandomMapInit(Entity<RandomSoundComponent> ent, ref MapInitEvent args)
  {
    TimeSpan min = ent.Comp.Min;
    TimeSpan maxTime = ent.Comp.Max;
    if (maxTime <= min)
      maxTime = min.Add(TimeSpan.FromTicks(1L));
    ent.Comp.PlayAt = new TimeSpan?(this._timing.CurTime + this._random.Next(min, maxTime));
    this.Dirty<RandomSoundComponent>(ent);
  }

  private void OnDeathMobStateChanged(
    Entity<SoundOnDeathComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead || !this._net.IsServer)
      return;
    SoundOnDeathComponent comp = ent.Comp;
    (EntityUid, AudioComponent)? nullable1 = this._audio.PlayPvs(ent.Comp.Sound, (EntityUid) ent);
    ref (EntityUid, AudioComponent)? local = ref nullable1;
    EntityUid? nullable2 = local.HasValue ? new EntityUid?(local.GetValueOrDefault().Item1) : new EntityUid?();
    comp.Entity = nullable2;
    this.Dirty<SoundOnDeathComponent>(ent);
  }

  private void OnDeathMobTerminating(
    Entity<SoundOnDeathComponent> ent,
    ref EntityTerminatingEvent args)
  {
    if (!ent.Comp.Entity.HasValue || this.TerminatingOrDeleted(ent.Comp.Entity))
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent);
    if (this.TerminatingOrDeleted(moverCoordinates.EntityId))
      return;
    this._transform.SetCoordinates(ent.Comp.Entity.Value, moverCoordinates);
    ent.Comp.Entity = new EntityUid?();
  }

  private void OnDeathSoundTerminating(
    Entity<SoundOnDeathSoundComponent> ent,
    ref EntityTerminatingEvent args)
  {
    EntityUid? parent = ent.Comp.Parent;
    ent.Comp.Parent = new EntityUid?();
    SoundOnDeathComponent comp;
    if (!this.TryComp<SoundOnDeathComponent>(parent, out comp))
      return;
    comp.Entity = new EntityUid?();
    this.Dirty(parent.Value, (IComponent) comp);
  }

  private void OnEmitSoundOnAction(
    Entity<EmitSoundOnActionComponent> ent,
    ref SoundActionEvent args)
  {
    this._emitSound.TryEmitSound((EntityUid) ent, (BaseEmitSoundComponent) (EmitSoundOnActionComponent) ent, new EntityUid?(args.Performer));
    if (!ent.Comp.Handle)
      return;
    args.Handled = true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RandomSoundComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RandomSoundComponent>();
    EntityUid uid;
    RandomSoundComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!this._mobState.IsDead(uid))
      {
        TimeSpan timeSpan = curTime;
        TimeSpan? playAt = comp1.PlayAt;
        if ((playAt.HasValue ? (timeSpan <= playAt.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          comp1.PlayAt = new TimeSpan?(curTime + this._random.Next(comp1.Min, comp1.Max));
          this.Dirty(uid, (IComponent) comp1);
          this._audio.PlayPvs(comp1.Sound, uid);
        }
      }
    }
  }
}
