// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Overwatch.OverwatchConsoleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Overwatch;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Overwatch;

public sealed class OverwatchConsoleSystem : SharedOverwatchConsoleSystem
{
  [Dependency]
  private AudioSystem _audio;
  [Dependency]
  private IEyeManager _eye;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly List<(Entity<AudioComponent, OverwatchRelayedSoundComponent> Audio, EntityCoordinates Position)> _toRelay = new List<(Entity<AudioComponent, OverwatchRelayedSoundComponent>, EntityCoordinates)>();

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OverwatchConsoleComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<OverwatchConsoleComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnOverwatchAfterState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OverwatchRelayedSoundComponent, ComponentRemove>(new EntityEventRefHandler<OverwatchRelayedSoundComponent, ComponentRemove>((object) this, __methodptr(OnRelayedRemove<ComponentRemove>)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OverwatchRelayedSoundComponent, EntityTerminatingEvent>(new EntityEventRefHandler<OverwatchRelayedSoundComponent, EntityTerminatingEvent>((object) this, __methodptr(OnRelayedRemove<EntityTerminatingEvent>)), (Type[]) null, (Type[]) null);
  }

  private void OnOverwatchAfterState(
    Entity<OverwatchConsoleComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<OverwatchConsoleComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is OverwatchConsoleBui overwatchConsoleBui)
          overwatchConsoleBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"OverwatchConsoleBui"}\n{ex}");
    }
  }

  private void OnRelayedRemove<T>(Entity<OverwatchRelayedSoundComponent> ent, ref T args)
  {
    this.TryDeleteRelayed(ent.Comp.Relay);
  }

  private void TryDeleteRelayed(EntityUid? relay)
  {
    if (!relay.HasValue || !this.IsClientSide(relay.Value, (MetaDataComponent) null))
      return;
    this.QueueDel(relay);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (localEntity.HasValue)
    {
      EntityUid valueOrDefault = localEntity.GetValueOrDefault();
      TransformComponent transformComponent1;
      if (this.HasComp<OverwatchWatchingComponent>(valueOrDefault) && this.TryComp(valueOrDefault, ref transformComponent1))
      {
        this._toRelay.Clear();
        MapCoordinates position = this._eye.CurrentEye.Position;
        EntityCoordinates coordinates1 = transformComponent1.Coordinates;
        AllEntityQueryEnumerator<AudioComponent, TransformComponent> entityQueryEnumerator = this.AllEntityQuery<AudioComponent, TransformComponent>();
        EntityUid entityUid1;
        AudioComponent audioComponent;
        TransformComponent transformComponent2;
        while (entityQueryEnumerator.MoveNext(ref entityUid1, ref audioComponent, ref transformComponent2))
        {
          EntityCoordinates coordinates2 = transformComponent2.Coordinates;
          Vector2 vector2;
          if (((EntityCoordinates) ref coordinates2).TryDelta((IEntityManager) this.EntityManager, this._transform, coordinates1, ref vector2))
          {
            if (MapId.op_Equality(position.MapId, transformComponent2.MapID) && (double) vector2.LengthSquared() <= (double) audioComponent.MaxDistance * (double) audioComponent.MaxDistance)
            {
              this.RemCompDeferred<OverwatchRelayedSoundComponent>(entityUid1);
            }
            else
            {
              MapCoordinates mapCoordinates = ((MapCoordinates) ref position).Offset(vector2);
              OverwatchRelayedSoundComponent relayedSoundComponent = this.EnsureComp<OverwatchRelayedSoundComponent>(entityUid1);
              if (relayedSoundComponent.Relay.HasValue && !this.TerminatingOrDeleted(relayedSoundComponent.Relay, (MetaDataComponent) null))
              {
                this._transform.SetMapCoordinates(relayedSoundComponent.Relay.Value, mapCoordinates);
              }
              else
              {
                EntityCoordinates coordinates3 = this._transform.ToCoordinates(mapCoordinates);
                this._toRelay.Add((Entity<AudioComponent, OverwatchRelayedSoundComponent>.op_Implicit((entityUid1, audioComponent, relayedSoundComponent)), coordinates3));
              }
            }
          }
        }
        using (List<(Entity<AudioComponent, OverwatchRelayedSoundComponent> Audio, EntityCoordinates Position)>.Enumerator enumerator = this._toRelay.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            (Entity<AudioComponent, OverwatchRelayedSoundComponent> Audio, EntityCoordinates Position) = enumerator.Current;
            (EntityUid, AudioComponent)? nullable = ((SharedAudioSystem) this._audio).PlayStatic((SoundSpecifier) new SoundPathSpecifier(Audio.Comp1.FileName, new AudioParams?()), valueOrDefault, Position, new AudioParams?(Audio.Comp1.Params));
            if (nullable.HasValue)
            {
              EntityUid entityUid2 = nullable.GetValueOrDefault().Item1;
              ((SharedAudioSystem) this._audio).SetPlaybackPosition(new Entity<AudioComponent>?(Entity<AudioComponent>.op_Implicit(entityUid2)), Audio.Comp1.PlaybackPosition);
              Audio.Comp2.Relay = new EntityUid?(entityUid2);
            }
          }
          return;
        }
      }
    }
    AllEntityQueryEnumerator<OverwatchRelayedSoundComponent> entityQueryEnumerator1 = this.AllEntityQuery<OverwatchRelayedSoundComponent>();
    EntityUid entityUid;
    OverwatchRelayedSoundComponent relayedSoundComponent1;
    while (entityQueryEnumerator1.MoveNext(ref entityUid, ref relayedSoundComponent1))
    {
      this.TryDeleteRelayed(relayedSoundComponent1.Relay);
      this.RemCompDeferred<OverwatchRelayedSoundComponent>(entityUid);
    }
  }
}
