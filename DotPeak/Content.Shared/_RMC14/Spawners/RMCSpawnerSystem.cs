// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Spawners.RMCSpawnerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Evacuation;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;

#nullable enable
namespace Content.Shared._RMC14.Spawners;

public sealed class RMCSpawnerSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedEvacuationSystem _evacuation;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SpawnOnInteractComponent, InteractHandEvent>(new EntityEventRefHandler<SpawnOnInteractComponent, InteractHandEvent>(this.OnSpawnOnInteractHand));
  }

  private void OnSpawnOnInteractHand(
    Entity<SpawnOnInteractComponent> ent,
    ref InteractHandEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid user = args.User;
    if (this.TerminatingOrDeleted((EntityUid) ent) || this.EntityManager.IsQueuedForDeletion((EntityUid) ent) || this._entityWhitelist.IsBlacklistPass(ent.Comp.Blacklist, user))
      return;
    if (ent.Comp.RequireEvacuation && !this._evacuation.IsEvacuationInProgress())
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-sentry-not-emergency", ("deployer", (object) ent)), (EntityUid) ent, user);
    }
    else
    {
      EntityUid uid = this.SpawnAtPosition((string) ent.Comp.Spawn, ent.Owner.ToCoordinates());
      LocId? popup = ent.Comp.Popup;
      if (popup.HasValue)
        this._popup.PopupEntity(this.Loc.GetString((string) popup.GetValueOrDefault(), ("spawned", (object) uid)), (EntityUid) ent, user);
      this._audio.PlayPvs(ent.Comp.Sound, uid);
      this.QueueDel(new EntityUid?((EntityUid) ent));
    }
  }
}
