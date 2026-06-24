// Decompiled with JetBrains decompiler
// Type: Content.Shared.Security.Systems.DeployableBarrierSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Lock;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Security.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

#nullable enable
namespace Content.Shared.Security.Systems;

public sealed class DeployableBarrierSystem : EntitySystem
{
  [Dependency]
  private FixtureSystem _fixtures;
  [Dependency]
  private SharedPointLightSystem _pointLight;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DeployableBarrierComponent, MapInitEvent>(new ComponentEventHandler<DeployableBarrierComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<DeployableBarrierComponent, LockToggledEvent>(new ComponentEventRefHandler<DeployableBarrierComponent, LockToggledEvent>(this.OnLockToggled));
  }

  private void OnMapInit(EntityUid uid, DeployableBarrierComponent component, MapInitEvent args)
  {
    LockComponent comp;
    if (!this.TryComp<LockComponent>(uid, out comp))
      return;
    this.ToggleBarrierDeploy(uid, comp.Locked, component);
  }

  private void OnLockToggled(
    EntityUid uid,
    DeployableBarrierComponent component,
    ref LockToggledEvent args)
  {
    this.ToggleBarrierDeploy(uid, args.Locked, component);
  }

  private void ToggleBarrierDeploy(
    EntityUid uid,
    bool isDeployed,
    DeployableBarrierComponent? component)
  {
    if (!this.Resolve<DeployableBarrierComponent>(uid, ref component))
      return;
    TransformComponent xform = this.Transform(uid);
    Fixture fixtureOrNull = this._fixtures.GetFixtureOrNull(uid, component.FixtureId);
    if (isDeployed && xform.GridUid.HasValue)
    {
      this._transform.AnchorEntity(uid, xform);
      if (fixtureOrNull != null)
        this._physics.SetHard(uid, fixtureOrNull, true);
    }
    else
    {
      this._transform.Unanchor(uid, xform);
      if (fixtureOrNull != null)
        this._physics.SetHard(uid, fixtureOrNull, false);
    }
    PullableComponent comp;
    if (this.TryComp<PullableComponent>(uid, out comp))
      this._pulling.TryStopPull(uid, comp);
    SharedPointLightComponent component1 = (SharedPointLightComponent) null;
    if (!this._pointLight.ResolveLight(uid, ref component1))
      return;
    this._pointLight.SetEnabled(uid, isDeployed, component1);
  }
}
