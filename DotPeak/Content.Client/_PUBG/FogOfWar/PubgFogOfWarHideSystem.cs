// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.FogOfWar.PubgFogOfWarHideSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Party;
using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.FogOfWar;
using Content.Shared._PUBG.Medicine;
using Content.Shared._PUBG.Party;
using Content.Shared._PUBG.Skin;
using Content.Shared.Armor;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Storage;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.FogOfWar;

public sealed class PubgFogOfWarHideSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private PubgFogOfWarSystem _fogSystem;
  [Dependency]
  private PubgFovModifierSystem _fovModifiers;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private PubgPartyClientSystem _party;
  public readonly List<(Entity<SpriteComponent?> Ent, float BaseAlpha)> CachedBaseAlphas = new List<(Entity<SpriteComponent>, float)>(128 /*0x80*/);
  private readonly HashSet<EntityUid> _revealed = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _shouldHide = new HashSet<EntityUid>();
  private readonly Dictionary<EntityUid, TimeSpan> _lastSeen = new Dictionary<EntityUid, TimeSpan>();
  private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();
  private readonly List<Box2> _occluderBoxes = new List<Box2>();
  private Vector2 _origin;
  private float _range;
  private float _cosHalfFov;
  private Vector2 _forward;
  private MapId _mapId;
  private EntityUid _playerEntity;
  private bool _prepared;
  private TimeSpan _nextOccludableRefresh;
  private static readonly TimeSpan OccludableRefreshInterval = TimeSpan.FromSeconds(1L);
  private static readonly TimeSpan MemoryFadeDuration = TimeSpan.FromSeconds(2L);
  private static readonly HashSet<ProtoId<TagPrototype>> AmmoTags = new HashSet<ProtoId<TagPrototype>>()
  {
    ProtoId<TagPrototype>.op_Implicit("Magazine762Pubg"),
    ProtoId<TagPrototype>.op_Implicit("Cartridge762Pubg"),
    ProtoId<TagPrototype>.op_Implicit("Ammo762Pubg"),
    ProtoId<TagPrototype>.op_Implicit("Ammo5-56x45mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRifle5-56x45mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo7-62x51mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRifle7-62x51mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo9x39mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRifle9x39mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo7-62x39mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRifle7-62x39mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo30-06sprPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRifle30-06spr"),
    ProtoId<TagPrototype>.op_Implicit("Ammo12x55mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRifle12x55mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo5-7x28mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeSmg5-7x28mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo9mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgePistol9mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo7-65mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgePistol7-65mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo12-7x33mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgePistol12-7x33mm"),
    ProtoId<TagPrototype>.op_Implicit("Ammo357mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRevolverMagnum357"),
    ProtoId<TagPrototype>.op_Implicit("Ammo500mmPubg"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeRevolverMagnum500"),
    ProtoId<TagPrototype>.op_Implicit("Ammo12x70Pubg"),
    ProtoId<TagPrototype>.op_Implicit("ShellShotgun12x70"),
    ProtoId<TagPrototype>.op_Implicit("Ammo23x75Pubg"),
    ProtoId<TagPrototype>.op_Implicit("ShellShotgun23x75"),
    ProtoId<TagPrototype>.op_Implicit("CartridgeGL-40mm")
  };

  public virtual void Initialize()
  {
    base.Initialize();
    this.RefreshOccludables();
  }

  public bool TryPrepare()
  {
    this._prepared = false;
    this._playerEntity = EntityUid.Invalid;
    if (this._timing.RealTime >= this._nextOccludableRefresh)
    {
      this.RefreshOccludables();
      this._nextOccludableRefresh = this._timing.RealTime + PubgFogOfWarHideSystem.OccludableRefreshInterval;
    }
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    TransformComponent transformComponent;
    PubgFogOfWarComponent fog;
    if (!localEntity.HasValue || !this._fogSystem.Active || !this.TryComp(localEntity.Value, ref transformComponent) || !this.TryComp<PubgFogOfWarComponent>(localEntity.Value, ref fog) || !fog.Enabled)
      return false;
    this._playerEntity = localEntity.Value;
    this._mapId = transformComponent.MapID;
    this._origin = this._xform.GetWorldPosition(transformComponent);
    this._range = MathF.Max(0.1f, fog.Range);
    this._cosHalfFov = MathF.Cos(MathHelper.DegreesToRadians(this._fovModifiers.GetEffectiveFov(localEntity.Value, fog) * 0.5f));
    Angle angle = fog.DesiredViewAngle.HasValue ? fog.CurrentAngle : this._xform.GetWorldRotation(transformComponent);
    this._forward = ((Angle) ref angle).ToWorldVec();
    this.BuildOccluderCache(this._mapId, this._origin, this._range);
    this._prepared = true;
    return true;
  }

  private void RefreshOccludables()
  {
    this._shouldHide.Clear();
    this.MarkWithComponent<PubgSkinComponent>();
    this.MarkWithComponent<HumanoidAppearanceComponent>();
    this.MarkAmmoTags();
    this.MarkWithComponent<PubgAmmoProviderComponent>();
    this.MarkWithComponent<PubgMedicalComponent>();
    this.MarkWithComponent<PubgEnergyDrinkComponent>();
    this.MarkWithComponent<GunComponent>();
    this.MarkWithComponent<ArmorComponent>();
    this.MarkStorageClothing();
    EntityQueryEnumerator<PubgFogOfWarOccludableComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PubgFogOfWarOccludableComponent>();
    EntityUid entityUid1;
    PubgFogOfWarOccludableComponent occludableComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid1, ref occludableComponent))
    {
      if (!this._shouldHide.Contains(entityUid1) || !this.HasComp<SpriteComponent>(entityUid1))
        this.RemComp<PubgFogOfWarOccludableComponent>(entityUid1);
    }
    foreach (EntityUid entityUid2 in this._shouldHide)
    {
      if (this.HasComp<SpriteComponent>(entityUid2))
        this.EnsureComp<PubgFogOfWarOccludableComponent>(entityUid2);
    }
    this.CleanupRevealedEntries();
    this.CleanupMemoryEntries();
  }

  private void MarkWithComponent<T>() where T : Component
  {
    EntityQueryEnumerator<T, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<T, SpriteComponent>();
    EntityUid entityUid;
    T obj;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref obj, ref spriteComponent))
      this._shouldHide.Add(entityUid);
  }

  private void MarkAmmoTags()
  {
    EntityQueryEnumerator<TagComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TagComponent, SpriteComponent>();
    EntityUid entityUid;
    TagComponent component;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref component, ref spriteComponent))
    {
      if (this._tags.HasAnyTag(component, PubgFogOfWarHideSystem.AmmoTags))
        this._shouldHide.Add(entityUid);
    }
  }

  private void MarkStorageClothing()
  {
    EntityQueryEnumerator<StorageComponent, ClothingComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<StorageComponent, ClothingComponent, SpriteComponent>();
    EntityUid entityUid;
    StorageComponent storageComponent;
    ClothingComponent clothingComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref storageComponent, ref clothingComponent, ref spriteComponent))
      this._shouldHide.Add(entityUid);
  }

  public float GetTargetAlpha(EntityUid uid, SpriteComponent sprite, TransformComponent xform)
  {
    if (!this._prepared || EntityUid.op_Equality(uid, this._playerEntity) || this.IsPartyMember(uid) || MapId.op_Inequality(xform.MapID, this._mapId) || xform.Anchored || !this.ShouldHide(uid))
      return sprite.Color.A;
    bool flag = this.ShouldRevealPersist(uid);
    if (flag && this._revealed.Contains(uid))
      return sprite.Color.A;
    if (!this.IsVisible(this._origin, this._xform.GetWorldPosition(xform), this._forward, this._cosHalfFov, this._range))
    {
      TimeSpan timeSpan1;
      if (!this.IsMemoryTarget(uid) || !this._lastSeen.TryGetValue(uid, out timeSpan1))
        return 0.0f;
      TimeSpan timeSpan2 = this._timing.CurTime - timeSpan1;
      if (timeSpan2 <= TimeSpan.Zero)
        return sprite.Color.A;
      if (timeSpan2 >= PubgFogOfWarHideSystem.MemoryFadeDuration)
        return 0.0f;
      float num = 1f - (float) (timeSpan2.TotalSeconds / PubgFogOfWarHideSystem.MemoryFadeDuration.TotalSeconds);
      return sprite.Color.A * num;
    }
    if (flag)
      this._revealed.Add(uid);
    if (this.IsMemoryTarget(uid))
      this._lastSeen[uid] = this._timing.CurTime;
    return sprite.Color.A;
  }

  private bool IsPartyMember(EntityUid uid)
  {
    if (this._party.Members.Count == 0)
      return false;
    NetEntity netEntity = this.GetNetEntity(uid, (MetaDataComponent) null);
    foreach (PubgPartyMemberState member in (IEnumerable<PubgPartyMemberState>) this._party.Members)
    {
      if (NetEntity.op_Equality(member.Entity, netEntity))
        return true;
    }
    return false;
  }

  private void BuildOccluderCache(MapId mapId, Vector2 origin, float range)
  {
    this._occluders.Clear();
    this._occluderBoxes.Clear();
    this._lookup.GetEntitiesInRange<OccluderComponent>(mapId, origin, range, this._occluders, (LookupFlags) 5);
    foreach (Entity<OccluderComponent> occluder in this._occluders)
    {
      EntityUid entityUid1;
      OccluderComponent occluderComponent1;
      occluder.Deconstruct(ref entityUid1, ref occluderComponent1);
      EntityUid entityUid2 = entityUid1;
      OccluderComponent occluderComponent2 = occluderComponent1;
      TransformComponent transformComponent;
      if (occluderComponent2.Enabled && this.TryComp(entityUid2, ref transformComponent) && !MapId.op_Inequality(transformComponent.MapID, mapId) && transformComponent.Anchored)
        this._occluderBoxes.Add(Matrix3Helpers.TransformBox(this._xform.GetWorldMatrix(entityUid2), ref occluderComponent2.BoundingBox));
    }
  }

  private bool IsVisible(
    Vector2 origin,
    Vector2 target,
    Vector2 forward,
    float cosHalfFov,
    float range)
  {
    Vector2 vector2_1 = target - origin;
    float x = vector2_1.LengthSquared();
    if ((double) x > (double) range * (double) range)
      return false;
    if ((double) x > 9.9999997473787516E-05)
    {
      Vector2 vector2_2 = vector2_1 / MathF.Sqrt(x);
      if ((double) Vector2.Dot(forward, vector2_2) < (double) cosHalfFov)
        return false;
    }
    float num = MathF.Sqrt(x);
    if ((double) num <= 1.0 / 1000.0)
      return true;
    Vector2 dir = vector2_1 / num;
    for (int index = 0; index < this._occluderBoxes.Count; ++index)
    {
      float distance;
      if (PubgFogOfWarHideSystem.RayAabb(origin, dir, this._occluderBoxes[index], out distance) && (double) distance >= 0.0 && (double) distance < (double) num - 0.05000000074505806)
        return false;
    }
    return true;
  }

  private bool ShouldHide(EntityUid uid)
  {
    TagComponent component;
    return this.HasComp<PubgSkinComponent>(uid) || this.HasComp<HumanoidAppearanceComponent>(uid) || this.TryComp<TagComponent>(uid, ref component) && this._tags.HasAnyTag(component, PubgFogOfWarHideSystem.AmmoTags) || this.HasComp<PubgAmmoProviderComponent>(uid) || this.HasComp<PubgMedicalComponent>(uid) || this.HasComp<PubgEnergyDrinkComponent>(uid) || this.HasComp<GunComponent>(uid) || this.HasComp<ArmorComponent>(uid) || this.HasComp<StorageComponent>(uid) && this.HasComp<ClothingComponent>(uid);
  }

  private bool ShouldRevealPersist(EntityUid uid)
  {
    if (this.HasComp<PubgSkinComponent>(uid) || this.HasComp<HumanoidAppearanceComponent>(uid))
      return false;
    TagComponent component;
    if (this.TryComp<TagComponent>(uid, ref component) && this._tags.HasAnyTag(component, PubgFogOfWarHideSystem.AmmoTags))
      return true;
    this.HasComp<PubgAmmoProviderComponent>(uid);
    return true;
  }

  private bool IsMemoryTarget(EntityUid uid)
  {
    return this.HasComp<HumanoidAppearanceComponent>(uid) || this.HasComp<PubgSkinComponent>(uid);
  }

  private void CleanupMemoryEntries()
  {
    if (this._lastSeen.Count == 0)
      return;
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (EntityUid key in this._lastSeen.Keys)
    {
      if (!this.Exists(key) || !this.IsMemoryTarget(key))
        entityUidList.Add(key);
    }
    for (int index = 0; index < entityUidList.Count; ++index)
      this._lastSeen.Remove(entityUidList[index]);
  }

  private void CleanupRevealedEntries()
  {
    if (this._revealed.Count == 0)
      return;
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (EntityUid uid in this._revealed)
    {
      if (!this.Exists(uid) || !this.ShouldRevealPersist(uid))
        entityUidList.Add(uid);
    }
    for (int index = 0; index < entityUidList.Count; ++index)
      this._revealed.Remove(entityUidList[index]);
  }

  private static bool RayAabb(Vector2 origin, Vector2 dir, Box2 box, out float distance)
  {
    distance = 0.0f;
    float tmin = 0.0f;
    float tmax = float.PositiveInfinity;
    if (!PubgFogOfWarHideSystem.RaySlab(origin.X, dir.X, box.Left, box.Right, ref tmin, ref tmax) || !PubgFogOfWarHideSystem.RaySlab(origin.Y, dir.Y, box.Bottom, box.Top, ref tmin, ref tmax) || (double) tmax < 0.0)
      return false;
    distance = (double) tmin >= 0.0 ? tmin : tmax;
    return (double) distance >= 0.0;
  }

  private static bool RaySlab(
    float origin,
    float dir,
    float min,
    float max,
    ref float tmin,
    ref float tmax)
  {
    if ((double) MathF.Abs(dir) < 9.9999997473787516E-05)
      return (double) origin >= (double) min && (double) origin <= (double) max;
    float num1 = 1f / dir;
    float num2 = (min - origin) * num1;
    float num3 = (max - origin) * num1;
    if ((double) num2 > (double) num3)
    {
      double num4 = (double) num2;
      num2 = num3;
      num3 = (float) num4;
    }
    if ((double) num2 > (double) tmin)
      tmin = num2;
    if ((double) num3 < (double) tmax)
      tmax = num3;
    return (double) tmax >= (double) tmin;
  }
}
