// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.SharedPointLightSystem
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.GameObjects;

public abstract class SharedPointLightSystem : EntitySystem
{
  public abstract SharedPointLightComponent EnsureLight(EntityUid uid);

  public abstract bool ResolveLight(EntityUid uid, [NotNullWhen(true)] ref SharedPointLightComponent? component);

  public abstract bool TryGetLight(EntityUid uid, [NotNullWhen(true)] out SharedPointLightComponent? component);

  public abstract bool RemoveLightDeferred(EntityUid uid);

  protected abstract void UpdatePriority(
    EntityUid uid,
    SharedPointLightComponent comp,
    MetaDataComponent meta);

  public void SetCastShadows(
    EntityUid uid,
    bool value,
    SharedPointLightComponent? comp = null,
    MetaDataComponent? meta = null)
  {
    if (!this.ResolveLight(uid, ref comp) || value == comp.CastShadows)
      return;
    comp.CastShadows = value;
    if (!this.Resolve(uid, ref meta))
      return;
    this.Dirty(uid, (IComponent) comp, meta);
    this.UpdatePriority(uid, comp, meta);
  }

  public void SetColor(EntityUid uid, Color value, SharedPointLightComponent? comp = null)
  {
    if (!this.ResolveLight(uid, ref comp) || Color.op_Equality(value, comp.Color))
      return;
    comp.Color = value;
    this.Dirty(uid, (IComponent) comp);
  }

  public virtual void SetEnabled(
    EntityUid uid,
    bool enabled,
    SharedPointLightComponent? comp = null,
    MetaDataComponent? meta = null)
  {
    if (!this.ResolveLight(uid, ref comp) || enabled == comp.Enabled)
      return;
    AttemptPointLightToggleEvent args = new AttemptPointLightToggleEvent(enabled);
    this.RaiseLocalEvent<AttemptPointLightToggleEvent>(uid, ref args);
    if (args.Cancelled)
      return;
    comp.Enabled = enabled;
    this.RaiseLocalEvent<PointLightToggleEvent>(uid, new PointLightToggleEvent(comp.Enabled));
    if (!this.Resolve(uid, ref meta))
      return;
    this.Dirty(uid, (IComponent) comp, meta);
    this.UpdatePriority(uid, comp, meta);
  }

  public void SetEnergy(EntityUid uid, float value, SharedPointLightComponent? comp = null)
  {
    if (!this.ResolveLight(uid, ref comp) || MathHelper.CloseToPercent(comp.Energy, value, 1E-05))
      return;
    comp.Energy = value;
    this.Dirty(uid, (IComponent) comp);
  }

  public virtual void SetRadius(
    EntityUid uid,
    float radius,
    SharedPointLightComponent? comp = null,
    MetaDataComponent? meta = null)
  {
    if (!this.ResolveLight(uid, ref comp) || MathHelper.CloseToPercent(comp.Radius, radius, 1E-05))
      return;
    comp.Radius = radius;
    if (!this.Resolve(uid, ref meta))
      return;
    this.Dirty(uid, (IComponent) comp, meta);
    this.UpdatePriority(uid, comp, meta);
  }

  public void SetSoftness(EntityUid uid, float value, SharedPointLightComponent? comp = null)
  {
    if (!this.ResolveLight(uid, ref comp) || MathHelper.CloseToPercent(comp.Softness, value, 1E-05))
      return;
    comp.Softness = value;
    this.Dirty(uid, (IComponent) comp);
  }

  public void SetFalloff(EntityUid uid, float value, SharedPointLightComponent? comp = null)
  {
    if (!this.ResolveLight(uid, ref comp) || MathHelper.CloseToPercent(comp.Falloff, value, 1E-05))
      return;
    comp.Falloff = value;
    this.Dirty(uid, (IComponent) comp);
  }

  public void SetCurveFactor(EntityUid uid, float value, SharedPointLightComponent? comp = null)
  {
    if (!this.ResolveLight(uid, ref comp) || MathHelper.CloseToPercent(comp.CurveFactor, value, 1E-05))
      return;
    comp.CurveFactor = value;
    this.Dirty(uid, (IComponent) comp);
  }

  protected static void OnLightGetState(
    EntityUid uid,
    SharedPointLightComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new PointLightComponentState()
    {
      Color = component.Color,
      Enabled = component.Enabled,
      Energy = component.Energy,
      Offset = component.Offset,
      Radius = component.Radius,
      Softness = component.Softness,
      Falloff = component.Falloff,
      CurveFactor = component.CurveFactor,
      CastShadows = component.CastShadows
    };
  }
}
