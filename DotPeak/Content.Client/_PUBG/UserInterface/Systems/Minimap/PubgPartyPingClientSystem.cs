// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgPartyPingClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Party;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgPartyPingClientSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAudioSystem _audio;
  private static readonly TimeSpan DoubleClickWindow = TimeSpan.FromMilliseconds(300L);
  private static readonly SoundSpecifier EnemyPingSound = (SoundSpecifier) new SoundCollectionSpecifier("PubgSoundCollectionEnemyPing", new AudioParams?(((AudioParams) ref AudioParams.Default).WithVolume(-6f)));
  private readonly List<PubgActivePingState> _activePings = new List<PubgActivePingState>();
  private PubgActivePingState[] _activePingSnapshot = Array.Empty<PubgActivePingState>();
  private MapCoordinates? _pendingClickCoords;
  private TimeSpan? _pendingClickSendAt;

  public IReadOnlyList<PubgActivePingState> ActivePings
  {
    get => (IReadOnlyList<PubgActivePingState>) this._activePingSnapshot;
  }

  public PubgActivePingState? LatestPing
  {
    get
    {
      if (this._activePingSnapshot.Length == 0)
        return new PubgActivePingState?();
      PubgActivePingState[] activePingSnapshot = this._activePingSnapshot;
      PubgActivePingState pubgActivePingState = activePingSnapshot[activePingSnapshot.Length - 1];
      return !(pubgActivePingState.ExpiresAtUtc > DateTime.UtcNow) ? new PubgActivePingState?() : new PubgActivePingState?(pubgActivePingState);
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgPartyPingBroadcastEvent>(new EntitySessionEventHandler<PubgPartyPingBroadcastEvent>(this.OnPartyPingBroadcast), (Type[]) null, (Type[]) null);
  }

  public void QueueMapClick(MapCoordinates coords)
  {
    if (MapId.op_Equality(coords.MapId, MapId.Nullspace) || !float.IsFinite(coords.Position.X) || !float.IsFinite(coords.Position.Y))
      return;
    if (this._pendingClickCoords.HasValue && this._pendingClickSendAt.HasValue && this._timing.CurTime >= this._pendingClickSendAt.Value)
    {
      this.SendPing(this._pendingClickCoords.Value, PubgPartyPingKind.Normal);
      this.ClearPendingClick();
    }
    if (this._pendingClickCoords.HasValue && this._pendingClickSendAt.HasValue && this._timing.CurTime <= this._pendingClickSendAt.Value)
    {
      this.SendPing(coords, PubgPartyPingKind.Enemy);
      this.ClearPendingClick();
    }
    else
    {
      this._pendingClickCoords = new MapCoordinates?(coords);
      this._pendingClickSendAt = new TimeSpan?(this._timing.CurTime + PubgPartyPingClientSystem.DoubleClickWindow);
    }
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (this._pendingClickCoords.HasValue && this._pendingClickSendAt.HasValue && this._timing.CurTime >= this._pendingClickSendAt.Value)
    {
      this.SendPing(this._pendingClickCoords.Value, PubgPartyPingKind.Normal);
      this.ClearPendingClick();
    }
    this.TrimExpiredPings();
  }

  private void OnPartyPingBroadcast(PubgPartyPingBroadcastEvent msg, EntitySessionEventArgs args)
  {
    if (msg.ExpiresAtUtc <= DateTime.UtcNow)
      return;
    for (int index = this._activePings.Count - 1; index >= 0; --index)
    {
      if (!NetEntity.op_Inequality(this._activePings[index].Source, msg.Source))
        this._activePings.RemoveAt(index);
    }
    this._activePings.Add(new PubgActivePingState(msg.Source, msg.MapId, msg.Position, msg.Kind, msg.ItemPrototypeId, msg.ExpiresAtUtc));
    this.RefreshSnapshot();
    if (msg.Kind != PubgPartyPingKind.Enemy)
      return;
    this._audio.PlayGlobal(PubgPartyPingClientSystem.EnemyPingSound, Filter.Local(), false, new AudioParams?());
  }

  private void SendPing(MapCoordinates coords, PubgPartyPingKind kind)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgPartyPingRequestEvent(coords.MapId, coords.Position, kind));
  }

  private void TrimExpiredPings()
  {
    DateTime utcNow = DateTime.UtcNow;
    bool flag = false;
    for (int index = this._activePings.Count - 1; index >= 0; --index)
    {
      if (!(this._activePings[index].ExpiresAtUtc > utcNow))
      {
        this._activePings.RemoveAt(index);
        flag = true;
      }
    }
    if (!flag)
      return;
    this.RefreshSnapshot();
  }

  private void RefreshSnapshot() => this._activePingSnapshot = this._activePings.ToArray();

  private void ClearPendingClick()
  {
    this._pendingClickCoords = new MapCoordinates?();
    this._pendingClickSendAt = new TimeSpan?();
  }
}
