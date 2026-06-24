// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.EntitySystems.SharedSingularitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radiation.Components;
using Content.Shared.Singularity.Components;
using Content.Shared.Singularity.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Singularity.EntitySystems;

public abstract class SharedSingularitySystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _visualizer;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedEventHorizonSystem _horizons;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  protected IViewVariablesManager Vvm;
  public const byte MinSingularityLevel = 0;
  public const byte MaxSingularityLevel = 6;
  public const float DistortionContainerScaling = 4f;
  public const float BaseGravityWellRadius = 2f;
  public const float BaseGravityWellAcceleration = 10f;
  public const byte SingularityBreachThreshold = 5;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SingularityComponent, ComponentStartup>(new ComponentEventHandler<SingularityComponent, ComponentStartup>(this.OnSingularityStartup));
    this.SubscribeLocalEvent<SingularityDistortionComponent, SingularityLevelChangedEvent>(new ComponentEventHandler<SingularityDistortionComponent, SingularityLevelChangedEvent>(this.UpdateDistortion));
    this.SubscribeLocalEvent<SingularityDistortionComponent, EntGotInsertedIntoContainerMessage>(new ComponentEventHandler<SingularityDistortionComponent, EntGotInsertedIntoContainerMessage>(this.UpdateDistortion));
    this.SubscribeLocalEvent<SingularityDistortionComponent, EntGotRemovedFromContainerMessage>(new ComponentEventHandler<SingularityDistortionComponent, EntGotRemovedFromContainerMessage>(this.UpdateDistortion));
    ViewVariablesTypeHandler<SingularityComponent> typeHandler = this.Vvm.GetTypeHandler<SingularityComponent>();
    typeHandler.AddPath<byte>("Level", (ComponentPropertyGetter<SingularityComponent, byte>) ((_, comp) => comp.Level), new ComponentPropertySetter<SingularityComponent, byte>(this.SetLevel));
    typeHandler.AddPath<float>("RadsPerLevel", (ComponentPropertyGetter<SingularityComponent, float>) ((_, comp) => comp.RadsPerLevel), new ComponentPropertySetter<SingularityComponent, float>(this.SetRadsPerLevel));
  }

  public override void Shutdown()
  {
    ViewVariablesTypeHandler<SingularityComponent> typeHandler = this.Vvm.GetTypeHandler<SingularityComponent>();
    typeHandler.RemovePath("Level");
    typeHandler.RemovePath("RadsPerLevel");
    base.Shutdown();
  }

  public void SetLevel(EntityUid uid, byte value, SingularityComponent? singularity = null)
  {
    if (!this.Resolve<SingularityComponent>(uid, ref singularity))
      return;
    value = MathHelper.Clamp(value, (byte) 0, (byte) 6);
    byte level = singularity.Level;
    if ((int) level == (int) value)
      return;
    singularity.Level = value;
    this.UpdateSingularityLevel(uid, level, singularity);
    if (this.Deleted(uid))
      return;
    this.Dirty(uid, (IComponent) singularity);
  }

  public void SetRadsPerLevel(EntityUid uid, float value, SingularityComponent? singularity = null)
  {
    if (!this.Resolve<SingularityComponent>(uid, ref singularity) || (double) singularity.RadsPerLevel == (double) value)
      return;
    singularity.RadsPerLevel = value;
    this.UpdateRadiation(uid, singularity);
  }

  public void UpdateSingularityLevel(
    EntityUid uid,
    byte oldValue,
    SingularityComponent? singularity = null)
  {
    if (!this.Resolve<SingularityComponent>(uid, ref singularity))
      return;
    EventHorizonComponent comp1;
    if (this.TryComp<EventHorizonComponent>(uid, out comp1))
    {
      this._horizons.SetRadius(uid, this.EventHorizonRadius(singularity), false, comp1);
      this._horizons.SetCanBreachContainment(uid, this.CanBreachContainment(singularity), false, comp1);
      this._horizons.UpdateEventHorizonFixture(uid, eventHorizon: comp1);
    }
    PhysicsComponent comp2;
    if (this.TryComp<PhysicsComponent>(uid, out comp2) && singularity.Level <= (byte) 1 && oldValue > (byte) 1)
      this._physics.SetLinearVelocity(uid, Vector2.Zero, body: comp2);
    AppearanceComponent comp3;
    if (this.TryComp<AppearanceComponent>(uid, out comp3))
      this._visualizer.SetData(uid, (Enum) SingularityAppearanceKeys.Singularity, (object) singularity.Level, comp3);
    RadiationSourceComponent comp4;
    if (this.TryComp<RadiationSourceComponent>(uid, out comp4))
      this.UpdateRadiation(uid, singularity, comp4);
    this.RaiseLocalEvent<SingularityLevelChangedEvent>(uid, new SingularityLevelChangedEvent(singularity.Level, oldValue, singularity));
    if (singularity.Level > (byte) 0)
      return;
    this.QueueDel(new EntityUid?(uid));
  }

  public void UpdateSingularityLevel(EntityUid uid, SingularityComponent? singularity = null)
  {
    if (!this.Resolve<SingularityComponent>(uid, ref singularity))
      return;
    this.UpdateSingularityLevel(uid, singularity.Level, singularity);
  }

  private void UpdateRadiation(
    EntityUid uid,
    SingularityComponent? singularity = null,
    RadiationSourceComponent? rads = null)
  {
    if (!this.Resolve<SingularityComponent, RadiationSourceComponent>(uid, ref singularity, ref rads, false))
      return;
    rads.Intensity = (float) singularity.Level * singularity.RadsPerLevel;
  }

  public float GravPulseRange(SingularityComponent singulo)
  {
    return 2f * (float) ((int) singulo.Level + 1);
  }

  public (float, float) GravPulseAcceleration(SingularityComponent singulo)
  {
    return (10f * (float) singulo.Level, 0.0f);
  }

  public float EventHorizonRadius(SingularityComponent singulo) => (float) singulo.Level - 0.5f;

  public bool CanBreachContainment(SingularityComponent singulo) => singulo.Level >= (byte) 5;

  public float GetFalloff(float level)
  {
    return (double) level == 0.0 ? 9999f : ((double) level == 1.0 ? MathF.Sqrt(6.4f) : ((double) level == 2.0 ? MathF.Sqrt(7f) : ((double) level == 3.0 ? MathF.Sqrt(8f) : ((double) level == 4.0 ? MathF.Sqrt(10f) : ((double) level == 5.0 ? MathF.Sqrt(12f) : ((double) level == 6.0 ? MathF.Sqrt(12f) : -1f))))));
  }

  public float GetIntensity(float level)
  {
    return (double) level == 0.0 ? 0.0f : ((double) level == 1.0 ? 3645f : ((double) level == 2.0 ? 103680f : ((double) level == 3.0 ? 1113920f : ((double) level == 4.0 ? 1.62E+07f : ((double) level == 5.0 ? 1.8E+08f : ((double) level == 6.0 ? 1.8E+08f : -1f))))));
  }

  protected virtual void OnSingularityStartup(
    EntityUid uid,
    SingularityComponent comp,
    ComponentStartup args)
  {
    this.UpdateSingularityLevel(uid, comp);
  }

  private void UpdateDistortion(
    EntityUid uid,
    SingularityDistortionComponent comp,
    SingularityLevelChangedEvent args)
  {
    float x1 = this.GetFalloff((float) args.NewValue);
    float x2 = this.GetIntensity((float) args.NewValue);
    if (this._containers.IsEntityInContainer(uid))
    {
      float x3 = MathF.Abs(x1);
      float x4 = MathF.Abs(x2);
      float y = -0.75f;
      x1 = (double) x3 > 1.0 ? x1 * MathF.Pow(x3, y) : x1;
      x2 = (double) x4 > 1.0 ? x2 * MathF.Pow(x4, y) : x2;
    }
    comp.FalloffPower = x1;
    comp.Intensity = x2;
    this.Dirty(uid, (IComponent) comp);
  }

  private void UpdateDistortion(
    EntityUid uid,
    SingularityDistortionComponent comp,
    EntGotInsertedIntoContainerMessage args)
  {
    float x1 = MathF.Abs(comp.FalloffPower);
    float x2 = MathF.Abs(comp.Intensity);
    float y = -0.75f;
    comp.FalloffPower = (double) x1 > 1.0 ? comp.FalloffPower * MathF.Pow(x1, y) : comp.FalloffPower;
    comp.Intensity = (double) x2 > 1.0 ? comp.Intensity * MathF.Pow(x2, y) : comp.Intensity;
  }

  private void UpdateDistortion(
    EntityUid uid,
    SingularityDistortionComponent comp,
    EntGotRemovedFromContainerMessage args)
  {
    float x1 = MathF.Abs(comp.FalloffPower);
    float x2 = MathF.Abs(comp.Intensity);
    float y = 3f;
    comp.FalloffPower = (double) x1 > 1.0 ? comp.FalloffPower * MathF.Pow(x1, y) : comp.FalloffPower;
    comp.Intensity = (double) x2 > 1.0 ? comp.Intensity * MathF.Pow(x2, y) : comp.Intensity;
  }

  [NetSerializable]
  [Serializable]
  protected sealed class SingularityComponentState : ComponentState
  {
    public readonly byte Level;

    public SingularityComponentState(SingularityComponent singulo) => this.Level = singulo.Level;
  }
}
