// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.AegisCrate.SharedAegisCrateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.AegisCrate;

public abstract class SharedAegisCrateSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  protected readonly TimeSpan OpeningSpeed = TimeSpan.FromSeconds(1.5);

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<AegisCrateComponent, ComponentStartup>(new EntityEventRefHandler<AegisCrateComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<AegisCrateComponent, ActivateInWorldEvent>(new EntityEventRefHandler<AegisCrateComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<AegisCrateComponent, InteractUsingEvent>(new EntityEventRefHandler<AegisCrateComponent, InteractUsingEvent>(this.OnInteractUsing));
  }

  protected virtual void OnStartup(Entity<AegisCrateComponent> crate, ref ComponentStartup args)
  {
    this.UpdateCrateVisuals(crate);
  }

  private void UpdateState(Entity<AegisCrateComponent> crate, AegisCrateState newState)
  {
    if (crate.Comp.State == newState)
      return;
    crate.Comp.State = newState;
    this.Dirty<AegisCrateComponent>(crate);
    this.UpdateCrateVisuals(crate);
  }

  private void UpdateCrateVisuals(Entity<AegisCrateComponent> crate)
  {
    AegisCrateStateChangedEvent args = new AegisCrateStateChangedEvent();
    this.RaiseLocalEvent<AegisCrateStateChangedEvent>((EntityUid) crate, ref args);
  }

  private void OpenAegis(Entity<AegisCrateComponent> crate, EntityUid user)
  {
    if (crate.Comp.State != AegisCrateState.Closed || !this._accessReader.IsAllowed(user, (EntityUid) crate))
      return;
    this.UpdateState(crate, AegisCrateState.Opening);
    this._audio.PlayPredicted(crate.Comp.OpenSound, (EntityUid) crate, new EntityUid?(user));
    crate.Comp.OpenAt = new TimeSpan?(this._timing.CurTime + this.OpeningSpeed);
  }

  private void OnActivate(Entity<AegisCrateComponent> crate, ref ActivateInWorldEvent args)
  {
    this.OpenAegis(crate, args.User);
  }

  private void OnInteractUsing(Entity<AegisCrateComponent> crate, ref InteractUsingEvent args)
  {
    this.OpenAegis(crate, args.User);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<AegisCrateComponent> entityQueryEnumerator = this.EntityQueryEnumerator<AegisCrateComponent>();
    EntityUid uid1;
    AegisCrateComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
    {
      if (!comp1.Spawned && comp1.OpenAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        TimeSpan? openAt = comp1.OpenAt;
        FixturesComponent comp;
        if ((openAt.HasValue ? (timeSpan < openAt.GetValueOrDefault() ? 1 : 0) : 0) == 0 && this.TryComp<FixturesComponent>(uid1, out comp))
        {
          this.UpdateState((Entity<AegisCrateComponent>) (uid1, comp1), AegisCrateState.Open);
          KeyValuePair<string, Fixture> keyValuePair = comp.Fixtures.First<KeyValuePair<string, Fixture>>();
          this._physics.SetCollisionLayer(uid1, keyValuePair.Key, keyValuePair.Value, 65, comp);
          comp1.Spawned = true;
          this.Dirty(uid1, (IComponent) comp1);
          EntityCoordinates coordinates = uid1.ToCoordinates();
          EntityUid uid2 = this.SpawnAtPosition((string) comp1.OB, coordinates);
          this.Log.Info($"{uid2.Id} spawned at {this._transform.GetWorldPosition(uid2)}");
        }
      }
    }
  }
}
