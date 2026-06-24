// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.BiomeDebugOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Parallax.Biomes;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

#nullable enable
namespace Content.Client.Parallax;

public sealed class BiomeDebugOverlay : Overlay
{
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IResourceCache _cache;
  [Dependency]
  private ITileDefinitionManager _tileDefManager;
  private BiomeSystem _biomes;
  private SharedMapSystem _maps;
  private Font _font;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public BiomeDebugOverlay()
  {
    IoCManager.InjectDependencies<BiomeDebugOverlay>(this);
    this._biomes = this._entManager.System<BiomeSystem>();
    this._maps = this._entManager.System<SharedMapSystem>();
    this._font = (Font) new VectorFont(this._cache.GetResource<FontResource>("/EngineFonts/NotoSans/NotoSans-Regular.ttf", true), 12);
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this._entManager.HasComponent<BiomeComponent>(this._maps.GetMapOrInvalid(new MapId?(args.MapId)));
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    ScreenCoordinates mouseScreenPosition = this._inputManager.MouseScreenPosition;
    MapCoordinates map = this._eyeManager.ScreenToMap(mouseScreenPosition);
    if (MapId.op_Equality(map.MapId, MapId.Nullspace) || MapId.op_Inequality(map.MapId, args.MapId))
      return;
    EntityUid mapOrInvalid = this._maps.GetMapOrInvalid(new MapId?(args.MapId));
    BiomeComponent component;
    MapGridComponent mapGridComponent;
    if (!this._entManager.TryGetComponent<BiomeComponent>(mapOrInvalid, ref component) || !this._entManager.TryGetComponent<MapGridComponent>(mapOrInvalid, ref mapGridComponent))
      return;
    StringBuilder stringBuilder = new StringBuilder();
    Vector2i tile1 = this._maps.WorldToTile(mapOrInvalid, mapGridComponent, map.Position);
    string entity;
    if (this._biomes.TryGetEntity(tile1, component, new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((mapOrInvalid, mapGridComponent))), out entity))
    {
      string str = "Entity: " + entity;
      stringBuilder.AppendLine(str);
    }
    List<(string ID, Vector2 Position)> decals;
    if (this._biomes.TryGetDecals(tile1, component.Layers, component.Seed, new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((mapOrInvalid, mapGridComponent))), out decals))
    {
      string str1 = $"Decals: {decals.Count}";
      stringBuilder.AppendLine(str1);
      foreach ((string ID, Vector2 Position) tuple in decals)
      {
        string str2 = "- " + tuple.ID;
        stringBuilder.AppendLine(str2);
      }
    }
    Tile? tile2;
    if (this._biomes.TryGetBiomeTile(tile1, component.Layers, component.Seed, new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((mapOrInvalid, mapGridComponent))), out tile2))
    {
      string str = "Tile: " + ((IPrototype) this._tileDefManager[tile2.Value.TypeId]).ID;
      stringBuilder.AppendLine(str);
    }
    ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, mouseScreenPosition.Position + new Vector2(0.0f, 32f), stringBuilder.ToString());
  }
}
