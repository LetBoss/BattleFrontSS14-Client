// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Controllers.VirtualController
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Prometheus;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.Physics.Controllers;

public abstract class VirtualController : EntitySystem
{
  [Dependency]
  protected readonly SharedPhysicsSystem PhysicsSystem;
  [Dependency]
  protected readonly SharedTransformSystem TransformSystem;
  private static readonly Stopwatch Stopwatch = new Stopwatch();
  public Histogram.Child BeforeMonitor;
  public Histogram.Child AfterMonitor;

  public override void Initialize()
  {
    base.Initialize();
    this.BeforeMonitor = ((Collector<Histogram.Child>) SharedPhysicsSystem.TickUsageControllerBeforeSolveHistogram).WithLabels(new string[1]
    {
      this.GetType().Name
    });
    this.AfterMonitor = ((Collector<Histogram.Child>) SharedPhysicsSystem.TickUsageControllerAfterSolveHistogram).WithLabels(new string[1]
    {
      this.GetType().Name
    });
    Type[] array1 = this.UpdatesBefore.ToArray();
    Type[] array2 = this.UpdatesAfter.ToArray();
    this.SubscribeLocalEvent<PhysicsUpdateBeforeSolveEvent>(new EntityEventRefHandler<PhysicsUpdateBeforeSolveEvent>(this.OnBeforeSolve), array1, array2);
    this.SubscribeLocalEvent<PhysicsUpdateAfterSolveEvent>(new EntityEventRefHandler<PhysicsUpdateAfterSolveEvent>(this.OnAfterSolve), array1, array2);
  }

  private void OnBeforeSolve(ref PhysicsUpdateBeforeSolveEvent ev)
  {
    if (this.PhysicsSystem.MetricsEnabled)
      VirtualController.Stopwatch.Restart();
    this.UpdateBeforeSolve(ev.Prediction, ev.DeltaTime);
    if (!this.PhysicsSystem.MetricsEnabled)
      return;
    this.BeforeMonitor.Observe(VirtualController.Stopwatch.Elapsed.TotalSeconds);
  }

  private void OnAfterSolve(ref PhysicsUpdateAfterSolveEvent ev)
  {
    if (this.PhysicsSystem.MetricsEnabled)
      VirtualController.Stopwatch.Restart();
    this.UpdateAfterSolve(ev.Prediction, ev.DeltaTime);
    if (!this.PhysicsSystem.MetricsEnabled)
      return;
    this.AfterMonitor.Observe(VirtualController.Stopwatch.Elapsed.TotalSeconds);
  }

  public virtual void UpdateBeforeSolve(bool prediction, float frameTime)
  {
  }

  public virtual void UpdateAfterSolve(bool prediction, float frameTime)
  {
  }
}
