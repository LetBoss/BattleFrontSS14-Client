// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Areas.AreaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.GameStates;
using Content.Shared._RMC14.Warps;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.GameTicking;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Areas;

public sealed class AreaSystem : EntitySystem
{
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedRMCPvsSystem _rmcPvs;
  [Dependency]
  private SharedRMCWarpSystem _rmcWarp;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TurfSystem _turf;
  private static readonly ProtoId<TagPrototype> WallTag = (ProtoId<TagPrototype>) "Wall";
  private Robust.Shared.GameObjects.EntityQuery<AreaComponent> _areaQuery;
  private Robust.Shared.GameObjects.EntityQuery<AreaGridComponent> _areaGridQuery;
  private Robust.Shared.GameObjects.EntityQuery<AreaLabelComponent> _areaLabelQuery;
  private Robust.Shared.GameObjects.EntityQuery<DamageableComponent> _damageableQuery;
  private Robust.Shared.GameObjects.EntityQuery<MapGridComponent> _mapGridQuery;
  private Robust.Shared.GameObjects.EntityQuery<MinimapColorComponent> _minimapColorQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoConstructComponent> _xenoConstruct;
  private readonly List<EntityUid> _toRender = new List<EntityUid>();
  private TimeSpan _earlySpreadHiveTime;

  public override void Initialize()
  {
    this._areaQuery = this.GetEntityQuery<AreaComponent>();
    this._areaGridQuery = this.GetEntityQuery<AreaGridComponent>();
    this._areaLabelQuery = this.GetEntityQuery<AreaLabelComponent>();
    this._damageableQuery = this.GetEntityQuery<DamageableComponent>();
    this._mapGridQuery = this.GetEntityQuery<MapGridComponent>();
    this._minimapColorQuery = this.GetEntityQuery<MinimapColorComponent>();
    this._xenoConstruct = this.GetEntityQuery<XenoConstructComponent>();
    this.SubscribeLocalEvent<AreaGridComponent, MapInitEvent>(new EntityEventRefHandler<AreaGridComponent, MapInitEvent>(this.OnAreaGridMapInit));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCHiveSpreadEarlyMinutes, (Action<int>) (v => this._earlySpreadHiveTime = TimeSpan.FromMinutes((long) v)), true);
  }

  private void OnAreaGridMapInit(Entity<AreaGridComponent> ent, ref MapInitEvent args)
  {
    this._toRender.Add((EntityUid) ent);
    foreach (EntProtoId<AreaComponent> entProtoId in ent.Comp.Areas.Values.DistinctBy<EntProtoId<AreaComponent>, string>((Func<EntProtoId<AreaComponent>, string>) (a => a.Id)))
    {
      if (ent.Comp.AreaEntities.ContainsKey(entProtoId))
        this.Log.Warning($"Duplicate area {entProtoId} found in entity {this.ToPrettyString(new EntityUid?((EntityUid) ent))}");
      else
        this.EnsureAreaEntityExists(ent.Comp, entProtoId);
    }
  }

  private void EnsureAreaEntityExists(AreaGridComponent areaGrid, EntProtoId<AreaComponent> area)
  {
    if (areaGrid.AreaEntities.ContainsKey(area))
      return;
    EntityUid ent = this.Spawn((string) area, MapCoordinates.Nullspace, rotation: new Angle());
    areaGrid.AreaEntities[area] = ent;
    this._rmcPvs.AddGlobalOverride(ent);
  }

  public void ReplaceArea(
    AreaGridComponent areaGrid,
    Vector2i position,
    EntProtoId<AreaComponent> area)
  {
    areaGrid.Areas[position] = area;
    this.EnsureAreaEntityExists(areaGrid, area);
  }

  public bool TryGetArea(
    Entity<MapGridComponent, AreaGridComponent?> grid,
    Vector2i indices,
    [NotNullWhen(true)] out Entity<AreaComponent>? area,
    [NotNullWhen(true)] out Robust.Shared.Prototypes.EntityPrototype? areaPrototype)
  {
    area = new Entity<AreaComponent>?();
    areaPrototype = (Robust.Shared.Prototypes.EntityPrototype) null;
    EntProtoId<AreaComponent> entProtoId;
    EntityUid uid;
    AreaComponent comp;
    if (!this.Resolve<AreaGridComponent>((EntityUid) grid, ref grid.Comp2, false) || !grid.Comp2.Areas.TryGetValue(indices, out entProtoId) || !this._prototypes.TryIndex((EntProtoId) entProtoId, out areaPrototype) || !grid.Comp2.AreaEntities.TryGetValue(entProtoId, out uid) || !this.TryComp<AreaComponent>(uid, out comp))
      return false;
    area = new Entity<AreaComponent>?((Entity<AreaComponent>) (uid, comp));
    return true;
  }

  public bool TryGetArea(
    EntityCoordinates coordinates,
    [NotNullWhen(true)] out Entity<AreaComponent>? area,
    [NotNullWhen(true)] out Robust.Shared.Prototypes.EntityPrototype? areaPrototype)
  {
    area = new Entity<AreaComponent>?();
    areaPrototype = (Robust.Shared.Prototypes.EntityPrototype) null;
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent component1;
      AreaGridComponent component2;
      if (this._mapGridQuery.TryComp(valueOrDefault, out component1) && this._areaGridQuery.TryComp(valueOrDefault, out component2))
      {
        Vector2i tile = this._map.CoordinatesToTile(valueOrDefault, component1, coordinates);
        return this.TryGetArea((Entity<MapGridComponent, AreaGridComponent>) (valueOrDefault, component1, component2), tile, out area, out areaPrototype);
      }
    }
    return false;
  }

  public bool TryGetArea(
    MapCoordinates coordinates,
    [NotNullWhen(true)] out Entity<AreaComponent>? area,
    [NotNullWhen(true)] out Robust.Shared.Prototypes.EntityPrototype? areaPrototype)
  {
    return this.TryGetArea(this._transform.ToCoordinates(coordinates), out area, out areaPrototype);
  }

  public bool TryGetArea(
    EntityUid coordinates,
    [NotNullWhen(true)] out Entity<AreaComponent>? area,
    [NotNullWhen(true)] out Robust.Shared.Prototypes.EntityPrototype? areaPrototype)
  {
    return this.TryGetArea(coordinates.ToCoordinates(), out area, out areaPrototype);
  }

  public bool TryGetAllAreas(EntityCoordinates coordinates, [NotNullWhen(true)] out Entity<AreaGridComponent>? areaGrid)
  {
    areaGrid = new Entity<AreaGridComponent>?();
    EntityUid? map = this._transform.GetMap(coordinates);
    if (map.HasValue)
    {
      EntityUid valueOrDefault = map.GetValueOrDefault();
      AreaGridComponent component;
      if (this._areaGridQuery.TryComp(valueOrDefault, out component))
      {
        areaGrid = new Entity<AreaGridComponent>?((Entity<AreaGridComponent>) (valueOrDefault, component));
        return true;
      }
    }
    return false;
  }

  public bool BioscanBlocked(EntityUid coordinates, out string? name)
  {
    name = (string) null;
    Entity<AreaComponent>? area;
    Robust.Shared.Prototypes.EntityPrototype areaPrototype;
    if (!this.TryGetArea(coordinates, out area, out areaPrototype))
      return false;
    name = areaPrototype.Name;
    return area.Value.Comp.AvoidBioscan;
  }

  public bool IsWeatherEnabled(Entity<MapGridComponent> grid, Vector2i indices)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea((Entity<MapGridComponent, AreaGridComponent>) grid, indices, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(new EntityCoordinates(grid.Owner, Vector2i.op_Implicit(indices)), (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanMortarPlace)) && area.Value.Comp.WeatherEnabled;
  }

  public bool IsLightBlocked(Entity<MapGridComponent> grid, Vector2i indices)
  {
    Entity<AreaComponent>? area;
    if (!this.TryGetArea((Entity<MapGridComponent, AreaGridComponent>) grid, indices, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return false;
    return this.IsRoofed(new EntityCoordinates(grid.Owner, Vector2i.op_Implicit(indices)), (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanMortarPlace)) || !area.Value.Comp.WeatherEnabled;
  }

  public bool CanCAS(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanCAS)) && area.Value.Comp.CAS;
  }

  public bool CanMortarFire(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanMortarFire)) && area.Value.Comp.MortarFire;
  }

  public bool CanMortarPlacement(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanMortarPlace)) && area.Value.Comp.MortarPlacement;
  }

  public bool CanOrbitalBombard(EntityCoordinates coordinates, out bool roofed)
  {
    roofed = false;
    Entity<AreaComponent>? area;
    if (!this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return false;
    if (!this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanOrbitalBombard)))
      return area.Value.Comp.OB;
    roofed = true;
    return false;
  }

  public bool CanFulton(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanFulton)) && area.Value.Comp.Fulton;
  }

  public bool CanLase(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanLase)) && area.Value.Comp.Lasing;
  }

  public bool CanMedevac(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanMedevac)) && area.Value.Comp.Medevac;
  }

  public bool CanParadrop(EntityCoordinates coordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(coordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(coordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanParadrop)) && area.Value.Comp.Paradropping;
  }

  private bool IsRoofed(
    EntityCoordinates coordinates,
    Predicate<Entity<RoofingEntityComponent>> predicate)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RoofingEntityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RoofingEntityComponent>();
    EntityUid uid;
    RoofingEntityComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      float distance;
      if (predicate((Entity<RoofingEntityComponent>) (uid, comp1)) && coordinates.TryDistance((IEntityManager) this.EntityManager, uid.ToCoordinates(), out distance) && (double) distance <= (double) comp1.Range)
        return true;
    }
    return false;
  }

  private bool IsRoofed(
    MapCoordinates mapCoordinates,
    Predicate<Entity<RoofingEntityComponent>> predicate)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RoofingEntityComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RoofingEntityComponent>();
    EntityUid uid;
    RoofingEntityComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (predicate((Entity<RoofingEntityComponent>) (uid, comp1)) && (double) (mapCoordinates.Position - this._transform.ToMapCoordinates(uid.ToCoordinates()).Position).Length() <= (double) comp1.Range)
        return true;
    }
    return false;
  }

  public bool CanResinPopup(
    Entity<MapGridComponent, AreaGridComponent?> grid,
    Vector2i indices,
    EntityUid? user)
  {
    Entity<AreaComponent>? area;
    if (!this.TryGetArea(grid, indices, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return true;
    if (area.Value.Comp.WeedKilling)
    {
      if (user.HasValue)
        this._popup.PopupClient("This area is unsuited to host the hive!", user.Value, new EntityUid?(user.Value), PopupType.MediumCaution);
      return false;
    }
    if (area.Value.Comp.ResinAllowed || this._gameTicker.RoundDuration() > this._earlySpreadHiveTime)
      return true;
    if (user.HasValue)
      this._popup.PopupClient("It's too early to spread the hive this far.", user.Value, new EntityUid?(user.Value), PopupType.MediumCaution);
    return false;
  }

  public bool CanSupplyDrop(MapCoordinates mapCoordinates)
  {
    Entity<AreaComponent>? area;
    return this.TryGetArea(mapCoordinates, out area, out Robust.Shared.Prototypes.EntityPrototype _) && !this.IsRoofed(mapCoordinates, (Predicate<Entity<RoofingEntityComponent>>) (r => !r.Comp.CanSupplyDrop)) && area.Value.Comp.SupplyDrop;
  }

  public void TrySetCanOrbitalBombardRoofing(Entity<RoofingEntityComponent?> roofing, bool ob)
  {
    if (!this.Resolve<RoofingEntityComponent>((EntityUid) roofing, ref roofing.Comp, false) || roofing.Comp.CanOrbitalBombard == ob)
      return;
    roofing.Comp.CanOrbitalBombard = ob;
    this.Dirty<RoofingEntityComponent>(roofing);
  }

  public string GetAreaName(EntityUid coordinates)
  {
    Robust.Shared.Prototypes.EntityPrototype areaPrototype;
    return !this.TryGetArea(coordinates.ToCoordinates(), out Entity<AreaComponent>? _, out areaPrototype) ? this.Loc.GetString("rmc-tacmap-alert-no-area") : areaPrototype.Name;
  }

  public override void Update(float frameTime)
  {
    try
    {
      foreach (EntityUid uid1 in this._toRender)
      {
        AreaGridComponent comp1;
        MapGridComponent comp2;
        if (this.TryComp<AreaGridComponent>(uid1, out comp1) && this.TryComp<MapGridComponent>(uid1, out comp2))
        {
          comp1.Colors.Clear();
          this.Dirty(uid1, (IComponent) comp1);
          GridTileEnumerator allTilesEnumerator = this._map.GetAllTilesEnumerator(uid1, comp2);
          Dictionary<EntProtoId<AreaComponent>, (int, int)> dict = new Dictionary<EntProtoId<AreaComponent>, (int, int)>();
          TileRef? tileRef;
          (int, int) valueTuple2;
          while (allTilesEnumerator.MoveNext(out tileRef))
          {
            TileRef tile = tileRef.Value;
            Vector2i gridIndices = tile.GridIndices;
            AnchoredEntitiesEnumerator entitiesEnumerator = this._map.GetAnchoredEntitiesEnumerator(uid1, comp2, gridIndices);
            bool flag1 = false;
            bool flag2 = false;
            bool flag3 = false;
            EntityUid? uid2;
            while (entitiesEnumerator.MoveNext(out uid2))
            {
              MinimapColorComponent component;
              if (this._minimapColorQuery.TryComp(uid2, out component))
              {
                comp1.Colors[gridIndices] = component.Color;
                flag1 = true;
              }
              if (this._areaLabelQuery.HasComp(uid2))
                comp1.Labels[gridIndices] = this._rmcWarp.GetName(uid2.Value) ?? this.Name(uid2.Value);
              if (!flag2 && this._tag.HasTag(uid2.Value, AreaSystem.WallTag) && !this._damageableQuery.HasComp(uid2.Value))
                flag2 = true;
              if (this._xenoConstruct.HasComp(uid2))
                flag3 = true;
            }
            EntProtoId<AreaComponent> key;
            comp1.Areas.TryGetValue(gridIndices, out key);
            (int, int)? nullable = new (int, int)?();
            if (flag3)
            {
              valueTuple2 = nullable.GetValueOrDefault();
              if (!nullable.HasValue)
              {
                valueTuple2 = dict.GetOrNew<EntProtoId<AreaComponent>, (int, int)>(key);
                nullable = new (int, int)?(valueTuple2);
              }
              nullable = new (int, int)?((nullable.Value.Item1 + 1, nullable.Value.Item2));
            }
            if (!flag2)
            {
              valueTuple2 = nullable.GetValueOrDefault();
              if (!nullable.HasValue)
              {
                valueTuple2 = dict.GetOrNew<EntProtoId<AreaComponent>, (int, int)>(key);
                nullable = new (int, int)?(valueTuple2);
              }
              nullable = new (int, int)?((nullable.Value.Item1, nullable.Value.Item2 + 1));
            }
            if (nullable.HasValue)
              dict[key] = nullable.Value;
            if (!flag1)
            {
              ContentTileDefinition contentTileDefinition = this._turf.GetContentTileDefinition(tile);
              AreaComponent comp3;
              comp1.Colors[gridIndices] = !Color.op_Inequality(contentTileDefinition.MinimapColor, new Color()) ? (!comp1.Areas.TryGetValue(gridIndices, out key) || !key.TryGet(out comp3, this._prototypes, this._compFactory) || !Color.op_Inequality(comp3.MinimapColor, new Color()) ? Color.FromHex((ReadOnlySpan<char>) "#6c6767d8", new Color?()) : ((Color) ref comp3.MinimapColor).WithAlpha(0.5f)) : contentTileDefinition.MinimapColor;
            }
          }
          EntProtoId<AreaComponent> key2;
          foreach ((key2, valueTuple2) in dict)
          {
            (int, int) valueTuple3 = valueTuple2;
            EntProtoId<AreaComponent> key3 = key2;
            int num1 = valueTuple3.Item1;
            int num2 = valueTuple3.Item2;
            EntityUid uid3;
            AreaComponent component;
            if (comp1.AreaEntities.TryGetValue(key3, out uid3) && this._areaQuery.TryComp(uid3, out component))
            {
              component.ResinConstructCount = num1;
              component.BuildableTiles = num2;
              this.Dirty(uid3, (IComponent) component);
            }
          }
          this.Dirty(uid1, (IComponent) comp1);
        }
      }
    }
    finally
    {
      this._toRender.Clear();
    }
  }
}
