// Decompiled with JetBrains decompiler
// Type: Content.Client.Radiation.Overlays.RadiationPulseOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Radiation.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Radiation.Overlays;

public sealed class RadiationPulseOverlay : Overlay
{
  private static readonly ProtoId<ShaderPrototype> RadiationShader = ProtoId<ShaderPrototype>.op_Implicit("Radiation");
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IGameTiming _gameTiming;
  private TransformSystem? _transform;
  private const float MaxDist = 15f;
  private readonly ShaderInstance _baseShader;
  private readonly Dictionary<EntityUid, (ShaderInstance shd, RadiationPulseOverlay.RadiationShaderInstance instance)> _pulses = new Dictionary<EntityUid, (ShaderInstance, RadiationPulseOverlay.RadiationShaderInstance)>();

  public virtual OverlaySpace Space => (OverlaySpace) 4;

  public virtual bool RequestScreenTexture => true;

  public RadiationPulseOverlay()
  {
    IoCManager.InjectDependencies<RadiationPulseOverlay>(this);
    this._baseShader = this._prototypeManager.Index<ShaderPrototype>(RadiationPulseOverlay.RadiationShader).Instance().Duplicate();
  }

  protected virtual bool BeforeDraw(in OverlayDrawArgs args)
  {
    this.RadiationQuery(args.Viewport.Eye);
    return this._pulses.Count > 0;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.ScreenTexture == null)
      return;
    DrawingHandleWorld worldHandle = ((OverlayDrawArgs) ref args).WorldHandle;
    IClydeViewport viewport = args.Viewport;
    foreach ((ShaderInstance shd, RadiationPulseOverlay.RadiationShaderInstance instance) in this._pulses.Values)
    {
      if (!MapId.op_Inequality(instance.CurrentMapCoords.MapId, args.MapId))
      {
        Vector2 local = viewport.WorldToLocal(instance.CurrentMapCoords.Position);
        local.Y = (float) viewport.Size.Y - local.Y;
        shd?.SetParameter("renderScale", viewport.RenderScale);
        shd?.SetParameter("positionInput", local);
        shd?.SetParameter("range", instance.Range);
        double num = (this._gameTiming.RealTime - instance.Start).TotalSeconds / (double) instance.Duration;
        shd?.SetParameter("life", (float) num);
        shd?.SetParameter("SCREEN_TEXTURE", viewport.RenderTarget.Texture);
        ((DrawingHandleBase) worldHandle).UseShader(shd);
        worldHandle.DrawRect(Box2.CenteredAround(instance.CurrentMapCoords.Position, new Vector2(instance.Range, instance.Range) * 2f), Color.White, true);
      }
    }
    ((DrawingHandleBase) worldHandle).UseShader((ShaderInstance) null);
  }

  private void RadiationQuery(IEye? currentEye)
  {
    if (this._transform == null)
      this._transform = this._entityManager.System<TransformSystem>();
    if (currentEye == null)
    {
      this._pulses.Clear();
    }
    else
    {
      MapCoordinates position = currentEye.Position;
      EntityQueryEnumerator<RadiationPulseComponent> entityQueryEnumerator = this._entityManager.EntityQueryEnumerator<RadiationPulseComponent>();
      EntityUid entityUid;
      RadiationPulseComponent radiationPulseComponent1;
      while (entityQueryEnumerator.MoveNext(ref entityUid, ref radiationPulseComponent1))
      {
        if (!this._pulses.ContainsKey(entityUid) && this.PulseQualifies(entityUid, position))
          this._pulses.Add(entityUid, (this._baseShader.Duplicate(), new RadiationPulseOverlay.RadiationShaderInstance(((SharedTransformSystem) this._transform).GetMapCoordinates(entityUid, (TransformComponent) null), radiationPulseComponent1.VisualRange, radiationPulseComponent1.StartTime, radiationPulseComponent1.VisualDuration)));
      }
      foreach (EntityUid key in this._pulses.Keys)
      {
        RadiationPulseComponent radiationPulseComponent2;
        if (this._entityManager.EntityExists(key) && this.PulseQualifies(key, position) && this._entityManager.TryGetComponent<RadiationPulseComponent>(key, ref radiationPulseComponent2))
        {
          (ShaderInstance shd, RadiationPulseOverlay.RadiationShaderInstance instance) pulse = this._pulses[key];
          pulse.instance.CurrentMapCoords = ((SharedTransformSystem) this._transform).GetMapCoordinates(key, (TransformComponent) null);
          pulse.instance.Range = radiationPulseComponent2.VisualRange;
        }
        else
        {
          this._pulses[key].shd.Dispose();
          this._pulses.Remove(key);
        }
      }
    }
  }

  private bool PulseQualifies(EntityUid pulseEntity, MapCoordinates currentEyeLoc)
  {
    TransformComponent component = this._entityManager.GetComponent<TransformComponent>(pulseEntity);
    SharedTransformSystem sharedTransformSystem = this._entityManager.System<SharedTransformSystem>();
    return MapId.op_Equality(component.MapID, currentEyeLoc.MapId) && sharedTransformSystem.InRange(component.Coordinates, sharedTransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(component.ParentUid), currentEyeLoc), 15f);
  }

  private sealed record RadiationShaderInstance(
    MapCoordinates CurrentMapCoords,
    float Range,
    TimeSpan Start,
    float Duration)
  {
    public MapCoordinates CurrentMapCoords = CurrentMapCoords;
    public float Range = Range;
    public TimeSpan Start = Start;
    public float Duration = Duration;

    [CompilerGenerated]
    public override int GetHashCode()
    {
      return (((EqualityComparer<Type>.Default.GetHashCode(this.EqualityContract) * -1521134295 + EqualityComparer<MapCoordinates>.Default.GetHashCode(this.CurrentMapCoords)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Range)) * -1521134295 + EqualityComparer<TimeSpan>.Default.GetHashCode(this.Start)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Duration);
    }

    [CompilerGenerated]
    public bool Equals(
      RadiationPulseOverlay.RadiationShaderInstance? other)
    {
      if ((object) this == (object) other)
        return true;
      return (object) other != null && this.EqualityContract == other.EqualityContract && EqualityComparer<MapCoordinates>.Default.Equals(this.CurrentMapCoords, other.CurrentMapCoords) && EqualityComparer<float>.Default.Equals(this.Range, other.Range) && EqualityComparer<TimeSpan>.Default.Equals(this.Start, other.Start) && EqualityComparer<float>.Default.Equals(this.Duration, other.Duration);
    }

    [CompilerGenerated]
    public void Deconstruct(
      out MapCoordinates CurrentMapCoords,
      out float Range,
      out TimeSpan Start,
      out float Duration)
    {
      CurrentMapCoords = this.CurrentMapCoords;
      Range = this.Range;
      Start = this.Start;
      Duration = this.Duration;
    }
  }
}
