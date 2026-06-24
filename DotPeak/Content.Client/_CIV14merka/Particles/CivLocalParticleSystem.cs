// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Particles.CivLocalParticleSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Particles;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.Particles;

public sealed class CivLocalParticleSystem : EntitySystem
{
  [Dependency]
  private readonly IOverlayManager _overlay;
  [Dependency]
  private readonly IPrototypeManager _proto;
  [Dependency]
  private readonly IPlayerManager _player;
  [Dependency]
  private readonly IResourceCache _resource;
  [Dependency]
  private readonly IGameTiming _timing;
  [Dependency]
  private readonly IRobustRandom _random;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  private static readonly ResPath DefaultParticle = new ResPath("/Textures/_CIV14merka/Particles/soft.png");
  private const float DegToRad = 0.0174532924f;
  private const int MaxParticles = 4000;
  private CivLocalParticleOverlay? _overlayInstance;
  private readonly List<CivLocalParticle> _particles = new List<CivLocalParticle>();
  private readonly Dictionary<EntityUid, float> _accum = new Dictionary<EntityUid, float>();
  private readonly Dictionary<string, Texture> _texCache = new Dictionary<string, Texture>();
  private Texture? _defaultTex;
  private MapId _playerMap = MapId.Nullspace;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CivParticleEmitterComponent, ComponentStartup>(new EntityEventRefHandler<CivParticleEmitterComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    this._proto.PrototypesReloaded += (Action<PrototypesReloadedEventArgs>) (_ => this._texCache.Clear());
    this._overlayInstance = new CivLocalParticleOverlay()
    {
      Particles = this._particles
    };
    this._overlay.AddOverlay((Overlay) this._overlayInstance);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    if (this._overlayInstance != null)
      this._overlay.RemoveOverlay((Overlay) this._overlayInstance);
    this._overlayInstance = (CivLocalParticleOverlay) null;
  }

  private void OnStartup(Entity<CivParticleEmitterComponent> ent, ref ComponentStartup args)
  {
    CivEmitterPresetPrototype preset;
    if (!this._proto.TryIndex<CivEmitterPresetPrototype>(ent.Comp.Preset, ref preset) || preset.BurstCount <= 0)
      return;
    MapId mapId = this.Transform(Entity<CivParticleEmitterComponent>.op_Implicit(ent)).MapID;
    if (MapId.op_Inequality(this._playerMap, MapId.Nullspace) && MapId.op_Inequality(mapId, this._playerMap))
      return;
    this.Emit(preset, this._transform.GetWorldPosition(ent.Owner), preset.BurstCount);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    this._playerMap = localEntity.HasValue ? this.Transform(localEntity.Value).MapID : MapId.Nullspace;
    Vector2 wind = this.GetWind();
    EntityQueryEnumerator<CivParticleEmitterComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivParticleEmitterComponent, TransformComponent>();
    EntityUid key;
    CivParticleEmitterComponent emitterComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref key, ref emitterComponent, ref transformComponent))
    {
      CivEmitterPresetPrototype preset;
      if (emitterComponent.Active && !MapId.op_Inequality(transformComponent.MapID, this._playerMap) && !MapId.op_Equality(this._playerMap, MapId.Nullspace) && this._proto.TryIndex<CivEmitterPresetPrototype>(emitterComponent.Preset, ref preset) && (double) preset.Rate > 0.0)
      {
        float num = this._accum.GetValueOrDefault<EntityUid, float>(key) + preset.Rate * frameTime;
        int count = (int) num;
        this._accum[key] = num - (float) count;
        if (count > 0)
          this.Emit(preset, this._transform.GetWorldPosition(key), count);
      }
    }
    for (int index1 = this._particles.Count - 1; index1 >= 0; --index1)
    {
      CivLocalParticle particle = this._particles[index1];
      particle.Age += frameTime;
      if ((double) particle.Age >= (double) particle.Life)
      {
        List<CivLocalParticle> particles1 = this._particles;
        int index2 = index1;
        List<CivLocalParticle> particles2 = this._particles;
        CivLocalParticle civLocalParticle = particles2[particles2.Count - 1];
        particles1[index2] = civLocalParticle;
        this._particles.RemoveAt(this._particles.Count - 1);
      }
      else
      {
        particle.Vel += particle.Gravity * frameTime;
        particle.Vel *= MathF.Max(0.0f, (float) (1.0 - (double) particle.Drag * (double) frameTime));
        particle.Pos += (particle.Vel + wind * particle.Wind) * frameTime;
        this._particles[index1] = particle;
      }
    }
    if (this._accum.Count <= 256 /*0x0100*/)
      return;
    this.PruneAccum();
  }

  public void EmitBurst(
    ProtoId<CivEmitterPresetPrototype> presetId,
    MapCoordinates coords,
    float scale = 1f,
    float? direction = null)
  {
    CivEmitterPresetPrototype preset;
    if (MapId.op_Equality(this._playerMap, MapId.Nullspace) || MapId.op_Inequality(coords.MapId, this._playerMap) || !this._proto.TryIndex<CivEmitterPresetPrototype>(presetId, ref preset))
      return;
    int count = (int) MathF.Max(1f, (float) preset.BurstCount * scale);
    this.Emit(preset, coords.Position, count, direction);
  }

  private void Emit(
    CivEmitterPresetPrototype preset,
    Vector2 origin,
    int count,
    float? directionOverride = null)
  {
    Texture texture = this.ResolveTexture(preset);
    float num1 = (float) ((double) directionOverride ?? (double) preset.Direction);
    for (int index = 0; index < count && this._particles.Count < 4000; ++index)
    {
      float x = (float) (((double) num1 + (double) this._random.NextFloat((float) (-(double) preset.Spread * 0.5), preset.Spread * 0.5f)) * (Math.PI / 180.0));
      float num2 = MathF.Max(0.0f, preset.Speed + this._random.NextFloat(-preset.SpeedVariance, preset.SpeedVariance));
      Vector2 vector2 = new Vector2(MathF.Cos(x), MathF.Sin(x)) * num2;
      float num3 = MathF.Max(0.05f, preset.Life + this._random.NextFloat(-preset.LifeVariance, preset.LifeVariance));
      this._particles.Add(new CivLocalParticle()
      {
        Pos = origin + this.RandomDisk(preset.Radius),
        Vel = vector2,
        Gravity = preset.Gravity,
        Drag = preset.Drag,
        Wind = preset.WindResponse,
        Age = 0.0f,
        Life = num3,
        Size0 = preset.SizeStart,
        Size1 = preset.SizeEnd,
        A0 = preset.AlphaStart,
        A1 = preset.AlphaEnd,
        Stretch = preset.Stretch,
        Rgb = preset.Color,
        Tex = texture
      });
    }
  }

  private Vector2 RandomDisk(float radius)
  {
    if ((double) radius <= 0.0)
      return Vector2.Zero;
    float x = this._random.NextFloat(0.0f, 6.28318548f);
    float num = MathF.Sqrt(this._random.NextFloat()) * radius;
    return new Vector2(MathF.Cos(x), MathF.Sin(x)) * num;
  }

  private Texture ResolveTexture(CivEmitterPresetPrototype preset)
  {
    if (!preset.Texture.HasValue)
      return this._defaultTex ?? (this._defaultTex = this._resource.GetResource<TextureResource>(CivLocalParticleSystem.DefaultParticle, true).Texture);
    Texture texture1;
    if (this._texCache.TryGetValue(preset.ID, out texture1))
      return texture1;
    Texture texture2 = this._resource.GetResource<TextureResource>(preset.Texture.Value, true).Texture;
    this._texCache[preset.ID] = texture2;
    return texture2;
  }

  private void PruneAccum()
  {
    List<EntityUid> entityUidList = new List<EntityUid>();
    foreach (EntityUid key in this._accum.Keys)
    {
      if (!this.EntityManager.EntityExists(key))
        entityUidList.Add(key);
    }
    foreach (EntityUid key in entityUidList)
      this._accum.Remove(key);
  }

  private Vector2 GetWind()
  {
    float totalSeconds = (float) this._timing.RealTime.TotalSeconds;
    float num = (float) (1.3999999761581421 + (double) MathF.Sin(totalSeconds * 0.13f) * 0.89999997615814209 + (double) MathF.Sin(totalSeconds * 0.41f) * 0.40000000596046448);
    float x = MathF.Sin(totalSeconds * 0.05f) * 0.6f;
    return new Vector2(MathF.Cos(x), MathF.Sin(x)) * num;
  }
}
