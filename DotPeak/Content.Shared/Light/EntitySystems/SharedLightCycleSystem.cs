// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.SharedLightCycleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Light.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public abstract class SharedLightCycleSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<LightCycleComponent, MapInitEvent>(new EntityEventRefHandler<LightCycleComponent, MapInitEvent>(this.OnCycleMapInit));
    this.SubscribeLocalEvent<LightCycleComponent, ComponentShutdown>(new EntityEventRefHandler<LightCycleComponent, ComponentShutdown>(this.OnCycleShutdown));
  }

  protected virtual void OnCycleMapInit(Entity<LightCycleComponent> ent, ref MapInitEvent args)
  {
    MapLightComponent comp;
    if (!this.TryComp<MapLightComponent>(ent.Owner, out comp))
      return;
    ent.Comp.OriginalColor = comp.AmbientLightColor;
    this.Dirty<LightCycleComponent>(ent);
  }

  private void OnCycleShutdown(Entity<LightCycleComponent> ent, ref ComponentShutdown args)
  {
    MapLightComponent comp;
    if (!this.TryComp<MapLightComponent>(ent.Owner, out comp))
      return;
    comp.AmbientLightColor = ent.Comp.OriginalColor;
    this.Dirty(ent.Owner, (IComponent) comp);
  }

  public void SetOffset(Entity<LightCycleComponent> entity, TimeSpan offset)
  {
    entity.Comp.Offset = offset;
    LightCycleOffsetEvent args = new LightCycleOffsetEvent(offset);
    this.RaiseLocalEvent<LightCycleOffsetEvent>((EntityUid) entity, ref args);
    this.Dirty<LightCycleComponent>(entity);
  }

  public static Color GetColor(Entity<LightCycleComponent> cycle, Color color, float time)
  {
    if (!cycle.Comp.Enabled)
      return color;
    double lightLevel = SharedLightCycleSystem.CalculateLightLevel(cycle.Comp, time);
    Color colorLevel = SharedLightCycleSystem.CalculateColorLevel(cycle.Comp, time);
    return new Color((byte) Math.Min((double) byte.MaxValue, (double) ((Color) ref color).RByte * (double) colorLevel.R * lightLevel), (byte) Math.Min((double) byte.MaxValue, (double) ((Color) ref color).GByte * (double) colorLevel.G * lightLevel), (byte) Math.Min((double) byte.MaxValue, (double) ((Color) ref color).BByte * (double) colorLevel.B * lightLevel), byte.MaxValue);
  }

  public static double CalculateLightLevel(LightCycleComponent comp, float time)
  {
    float waveLength = MathF.Max(1f, (float) comp.Duration.TotalSeconds);
    float crest = MathF.Max(0.0f, comp.MaxLightLevel);
    float shift = MathF.Max(0.0f, comp.MinLightLevel);
    return (double) Math.Min(comp.ClipLight, SharedLightCycleSystem.CalculateCurve(time, waveLength, crest, shift, 6f));
  }

  public static Color CalculateColorLevel(LightCycleComponent comp, float time)
  {
    float waveLength = MathF.Max(1f, (float) comp.Duration.TotalSeconds);
    double num1 = (double) MathF.Min(comp.ClipLevel.R, SharedLightCycleSystem.CalculateCurve(time, waveLength, MathF.Max(0.0f, comp.MaxLevel.R), MathF.Max(0.0f, comp.MinLevel.R), 4f));
    float num2 = MathF.Min(comp.ClipLevel.G, SharedLightCycleSystem.CalculateCurve(time, waveLength, MathF.Max(0.0f, comp.MaxLevel.G), MathF.Max(0.0f, comp.MinLevel.G), 10f));
    float num3 = MathF.Min(comp.ClipLevel.B, SharedLightCycleSystem.CalculateCurve(time, waveLength / 2f, MathF.Max(0.0f, comp.MaxLevel.B), MathF.Max(0.0f, comp.MinLevel.B), 2f, waveLength / 4f));
    double num4 = (double) num2;
    double num5 = (double) num3;
    return new Color((float) num1, (float) num4, (float) num5, 1f);
  }

  public static float CalculateCurve(
    float x,
    float waveLength,
    float crest,
    float shift,
    float exponent,
    float phase = 0.0f)
  {
    float num = MathF.Pow(MathF.Sin((float) (3.1415927410125732 * ((double) phase + (double) x)) / waveLength), exponent);
    return (crest - shift) * num + shift;
  }
}
