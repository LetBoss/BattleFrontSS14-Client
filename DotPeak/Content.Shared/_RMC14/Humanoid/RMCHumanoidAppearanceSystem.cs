// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Humanoid.RMCHumanoidAppearanceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Station;
using Content.Shared.DisplacementMap;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.Whitelist;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Humanoid;

public sealed class RMCHumanoidAppearanceSystem : EntitySystem
{
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedRMCStationSpawningSystem _rmcStationSpawning;
  private EntityUid? _spawnMap;

  public bool HidePlayerIdentities { get; private set; }

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PlayerSpawnCompleteEvent>(new EntityEventHandler<PlayerSpawnCompleteEvent>(this.OnPlayerSpawnComplete));
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart));
    this.Subs.CVar<bool>(this._config, RMCCVars.HidePlayerIdentities, new Action<bool>(this.OnHidePlayerIdentitiesChanged), true);
  }

  private void OnRoundRestart(RoundRestartCleanupEvent ev) => this._spawnMap = new EntityUid?();

  private void OnPlayerSpawnComplete(PlayerSpawnCompleteEvent ev)
  {
    HumanoidAppearanceComponent comp1;
    if (!this.TryComp<HumanoidAppearanceComponent>(ev.Mob, out comp1))
      return;
    if (!this._spawnMap.HasValue || this.TerminatingOrDeleted(this._spawnMap))
      this._spawnMap = new EntityUid?(this._map.CreateMap());
    EntityUid? uid = this._rmcStationSpawning.SpawnPlayerMob(new EntityCoordinates(this._spawnMap.Value, Vector2.Zero), new ProtoId<JobPrototype>?(), HumanoidCharacterProfile.RandomWithSpecies((string) comp1.Species), new EntityUid?());
    HumanoidAppearanceComponent comp2;
    if (!this.TryComp<HumanoidAppearanceComponent>(uid, out comp2))
      return;
    HiddenAppearanceComponent appearanceComponent = this.EnsureComp<HiddenAppearanceComponent>(ev.Mob);
    appearanceComponent.Appearance = new RMCHumanoidAppearance()
    {
      ClientOldMarkings = new MarkingSet(comp2.ClientOldMarkings),
      MarkingSet = new MarkingSet(comp2.MarkingSet),
      BaseLayers = new Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>((IDictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>) comp2.BaseLayers),
      PermanentlyHidden = new HashSet<HumanoidVisualLayers>((IEnumerable<HumanoidVisualLayers>) comp2.PermanentlyHidden),
      Gender = comp2.Gender,
      Age = comp2.Age,
      CustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>((IDictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) comp2.CustomBaseLayers),
      Species = comp2.Species,
      SkinColor = comp2.SkinColor,
      HiddenLayers = new Dictionary<HumanoidVisualLayers, SlotFlags>((IDictionary<HumanoidVisualLayers, SlotFlags>) comp2.HiddenLayers),
      Sex = comp2.Sex,
      EyeColor = comp2.EyeColor,
      CachedHairColor = comp2.CachedHairColor,
      CachedFacialHairColor = comp2.CachedFacialHairColor,
      HideLayersOnEquip = new HashSet<HumanoidVisualLayers>((IEnumerable<HumanoidVisualLayers>) comp2.HideLayersOnEquip),
      UndergarmentTop = comp2.UndergarmentTop,
      UndergarmentBottom = comp2.UndergarmentBottom,
      MarkingsDisplacement = new Dictionary<HumanoidVisualLayers, DisplacementData>((IDictionary<HumanoidVisualLayers, DisplacementData>) comp2.MarkingsDisplacement)
    };
    this.Dirty(ev.Mob, (IComponent) appearanceComponent);
    this.QueueDel(uid);
  }

  private void OnHidePlayerIdentitiesChanged(bool value)
  {
    this.HidePlayerIdentities = value;
    if (this.HidePlayerIdentities)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<HiddenAppearanceComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HiddenAppearanceComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out HiddenAppearanceComponent _))
      this.RemCompDeferred<HiddenAppearanceComponent>(uid);
  }

  public bool TryGetLocalHiddenAppearance(EntityUid ent, [NotNullWhen(true)] out IRMCHumanoidAppearance? appearance)
  {
    appearance = (IRMCHumanoidAppearance) null;
    HiddenAppearanceComponent comp;
    if (!this.TryComp<HiddenAppearanceComponent>(ent, out comp) || comp.Appearance == null)
      return false;
    appearance = (IRMCHumanoidAppearance) comp.Appearance;
    EntityUid? localEntity = this._player.LocalEntity;
    if (!localEntity.HasValue)
      return false;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    return this._entityWhitelist.IsWhitelistPass(comp.Whitelist, valueOrDefault);
  }
}
