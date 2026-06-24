// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivParticleOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Particles;
using Content.Shared.Light.Components;
using Content.Shared.Light.EntitySystems;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

public sealed class CivParticleOverlay : Overlay
{
  private static readonly ResPath DefaultParticle = new ResPath("/Textures/_CIV14merka/Particles/soft.png");
  [Dependency]
  private readonly IRobustRandom _random;
  [Dependency]
  private readonly IGameTiming _timing;
  private readonly IEntityManager _entMan;
  private readonly IMapManager _mapMan;
  private readonly SharedTransformSystem _xform;
  private readonly SharedRoofSystem _roof;
  private readonly IResourceCache _resource;
  public CivParticlePresetPrototype? Preset;
  public float Intensity;
  public Vector2 Wind;
  private CivParticleOverlay.Particle[] _particles = Array.Empty<CivParticleOverlay.Particle>();
  private int _active;
  private DrawVertexUV2DColor[] _verts = Array.Empty<DrawVertexUV2DColor>();
  private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();
  private readonly List<CivParticleOverlay.RoofGrid> _roofGrids = new List<CivParticleOverlay.RoofGrid>();
  private CivParticlePresetPrototype? _builtFor;
  private Texture? _defaultTex;
  private TimeSpan _lastDraw;

  public virtual OverlaySpace Space => (OverlaySpace) 8;

  public CivParticleOverlay(
    IEntityManager entMan,
    IMapManager mapMan,
    SharedTransformSystem xform,
    SharedRoofSystem roof,
    IResourceCache resource)
  {
    IoCManager.InjectDependencies<CivParticleOverlay>(this);
    this._entMan = entMan;
    this._mapMan = mapMan;
    this._xform = xform;
    this._roof = roof;
    this._resource = resource;
    this.ZIndex = new int?(0);
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    return this.Preset != null && (double) this.Intensity > 0.0099999997764825821;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    CivParticlePresetPrototype preset = this.Preset;
    if (preset == null || (double) this.Intensity <= 0.0099999997764825821)
      return;
    TimeSpan realTime = this._timing.RealTime;
    float x = (float) (realTime - this._lastDraw).TotalSeconds;
    this._lastDraw = realTime;
    if ((double) x <= 0.0 || (double) x > 0.10000000149011612)
      x = MathF.Min(MathF.Max(x, 0.0f), 0.1f);
    Box2 box = ((Box2) ref args.WorldAABB).Enlarged(3f);
    int count = (int) MathF.Min(((Box2) ref box).Width * ((Box2) ref box).Height * preset.Density * this.Intensity, (float) preset.MaxParticles);
    if (count <= 0)
      return;
    this.EnsureCapacity(count, preset, box);
    Vector2 wind = this.Wind * preset.WindResponse;
    for (int index = 0; index < this._active; ++index)
    {
      ref CivParticleOverlay.Particle local = ref this._particles[index];
      local.Pos += (local.Vel + wind) * x;
      if (!((Box2) ref box).Contains(local.Pos, true))
        this.Respawn(ref local, box, preset);
    }
    this.BuildRoofGrids(args.MapId, box);
    Texture texture = this.ResolveTexture(preset);
    Color color = Color.FromSrgb(((Color) ref preset.Color).WithAlpha(MathF.Min(preset.Alpha * this.Intensity, 1f)));
    int num = this.BuildVertices(wind, color, preset.Stretch);
    if (num == 0)
      return;
    ((DrawingHandleBase) ((OverlayDrawArgs) ref args).WorldHandle).DrawPrimitives((DrawPrimitiveTopology) 1, texture, (ReadOnlySpan<DrawVertexUV2DColor>) this._verts.AsSpan<DrawVertexUV2DColor>(0, num * 6));
  }

  private int BuildVertices(Vector2 wind, Color color, float stretch)
  {
    int num1 = 0;
    for (int index1 = 0; index1 < this._active; ++index1)
    {
      CivParticleOverlay.Particle particle = this._particles[index1];
      if (!this.IsRoofed(particle.Pos))
      {
        Vector2 vector2_1 = particle.Vel + wind;
        Vector2 vector2_2 = (double) vector2_1.LengthSquared() > 9.9999997473787516E-05 ? Vector2.Normalize(vector2_1) : new Vector2(0.0f, -1f);
        Vector2 vector2_3 = new Vector2(vector2_2.Y, -vector2_2.X);
        float num2 = particle.Size * 0.5f;
        float num3 = (float) ((double) particle.Size * (double) stretch * 0.5);
        Vector2 vector2_4 = particle.Pos - vector2_3 * num2 - vector2_2 * num3;
        Vector2 vector2_5 = particle.Pos + vector2_3 * num2 - vector2_2 * num3;
        Vector2 vector2_6 = particle.Pos + vector2_3 * num2 + vector2_2 * num3;
        Vector2 vector2_7 = particle.Pos - vector2_3 * num2 + vector2_2 * num3;
        int index2 = num1 * 6;
        this._verts[index2] = new DrawVertexUV2DColor(vector2_4, new Vector2(0.0f, 1f), color);
        this._verts[index2 + 1] = new DrawVertexUV2DColor(vector2_5, new Vector2(1f, 1f), color);
        this._verts[index2 + 2] = new DrawVertexUV2DColor(vector2_6, new Vector2(1f, 0.0f), color);
        this._verts[index2 + 3] = new DrawVertexUV2DColor(vector2_4, new Vector2(0.0f, 1f), color);
        this._verts[index2 + 4] = new DrawVertexUV2DColor(vector2_6, new Vector2(1f, 0.0f), color);
        this._verts[index2 + 5] = new DrawVertexUV2DColor(vector2_7, new Vector2(0.0f, 0.0f), color);
        ++num1;
      }
    }
    return num1;
  }

  private bool IsRoofed(Vector2 worldPos)
  {
    foreach (CivParticleOverlay.RoofGrid roofGrid in this._roofGrids)
    {
      Vector2 vector2 = Vector2.Transform(worldPos, roofGrid.Inv);
      ushort tileSize = roofGrid.Grid.TileSize;
      Vector2i index;
      // ISSUE: explicit constructor call
      ((Vector2i) ref index).\u002Ector((int) MathF.Floor(vector2.X / (float) tileSize), (int) MathF.Floor(vector2.Y / (float) tileSize));
      if (this._roof.IsRooved(Entity<MapGridComponent, RoofComponent>.op_Implicit((roofGrid.Uid, roofGrid.Grid, roofGrid.Roof)), index))
        return true;
    }
    return false;
  }

  private void BuildRoofGrids(MapId mapId, Box2 box)
  {
    this._roofGrids.Clear();
    this._grids.Clear();
    this._mapMan.FindGridsIntersecting(mapId, box, ref this._grids, false, true);
    foreach (Entity<MapGridComponent> grid in this._grids)
    {
      RoofComponent roof;
      Matrix3x2 result;
      if (this._entMan.TryGetComponent<RoofComponent>(grid.Owner, ref roof) && Matrix3x2.Invert(this._xform.GetWorldMatrix(grid.Owner), out result))
        this._roofGrids.Add(new CivParticleOverlay.RoofGrid(grid.Owner, grid.Comp, roof, result));
    }
  }

  private Texture ResolveTexture(CivParticlePresetPrototype preset)
  {
    if (preset.Texture.HasValue)
      return this._resource.GetResource<TextureResource>(preset.Texture.Value, true).Texture;
    if (this._defaultTex == null)
      this._defaultTex = this._resource.GetResource<TextureResource>(CivParticleOverlay.DefaultParticle, true).Texture;
    return this._defaultTex;
  }

  private void Respawn(
    ref CivParticleOverlay.Particle p,
    Box2 box,
    CivParticlePresetPrototype preset)
  {
    p.Pos = new Vector2(this._random.NextFloat(box.Left, box.Right), this._random.NextFloat(box.Bottom, box.Top));
    p.Vel = preset.Velocity + new Vector2(this._random.NextFloat(-preset.VelocityVariance.X, preset.VelocityVariance.X), this._random.NextFloat(-preset.VelocityVariance.Y, preset.VelocityVariance.Y));
    p.Size = this._random.NextFloat(preset.SizeMin, preset.SizeMax);
  }

  private void EnsureCapacity(int count, CivParticlePresetPrototype preset, Box2 box)
  {
    bool flag = this._builtFor != preset;
    if (count > this._particles.Length | flag)
    {
      CivParticleOverlay.Particle[] destinationArray = new CivParticleOverlay.Particle[count];
      int length = flag ? 0 : Math.Min(this._active, this._particles.Length);
      Array.Copy((Array) this._particles, (Array) destinationArray, length);
      for (int index = length; index < count; ++index)
        this.Respawn(ref destinationArray[index], box, preset);
      this._particles = destinationArray;
    }
    if (count * 6 > this._verts.Length)
      this._verts = new DrawVertexUV2DColor[count * 6];
    this._builtFor = preset;
    this._active = count;
  }

  private struct Particle
  {
    public Vector2 Pos;
    public Vector2 Vel;
    public float Size;
  }

  private readonly struct RoofGrid(
    EntityUid uid,
    MapGridComponent grid,
    RoofComponent roof,
    Matrix3x2 inv)
  {
    public readonly EntityUid Uid = uid;
    public readonly MapGridComponent Grid = grid;
    public readonly RoofComponent Roof = roof;
    public readonly Matrix3x2 Inv = inv;
  }
}
