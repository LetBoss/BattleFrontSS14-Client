using System;
using Prometheus;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Robust.Shared.Physics.Controllers;

public abstract class VirtualController : EntitySystem
{
	[Dependency]
	protected readonly SharedPhysicsSystem PhysicsSystem;

	[Dependency]
	protected readonly SharedTransformSystem TransformSystem;

	private static readonly Stopwatch Stopwatch = new Stopwatch();

	public Child BeforeMonitor;

	public Child AfterMonitor;

	public override void Initialize()
	{
		base.Initialize();
		BeforeMonitor = ((Collector<Child>)(object)SharedPhysicsSystem.TickUsageControllerBeforeSolveHistogram).WithLabels(new string[1] { GetType().Name });
		AfterMonitor = ((Collector<Child>)(object)SharedPhysicsSystem.TickUsageControllerAfterSolveHistogram).WithLabels(new string[1] { GetType().Name });
		Type[] before = base.UpdatesBefore.ToArray();
		Type[] after = base.UpdatesAfter.ToArray();
		SubscribeLocalEvent<PhysicsUpdateBeforeSolveEvent>(OnBeforeSolve, before, after);
		SubscribeLocalEvent<PhysicsUpdateAfterSolveEvent>(OnAfterSolve, before, after);
	}

	private void OnBeforeSolve(ref PhysicsUpdateBeforeSolveEvent ev)
	{
		if (PhysicsSystem.MetricsEnabled)
		{
			Stopwatch.Restart();
		}
		UpdateBeforeSolve(ev.Prediction, ev.DeltaTime);
		if (PhysicsSystem.MetricsEnabled)
		{
			BeforeMonitor.Observe(Stopwatch.Elapsed.TotalSeconds);
		}
	}

	private void OnAfterSolve(ref PhysicsUpdateAfterSolveEvent ev)
	{
		if (PhysicsSystem.MetricsEnabled)
		{
			Stopwatch.Restart();
		}
		UpdateAfterSolve(ev.Prediction, ev.DeltaTime);
		if (PhysicsSystem.MetricsEnabled)
		{
			AfterMonitor.Observe(Stopwatch.Elapsed.TotalSeconds);
		}
	}

	public virtual void UpdateBeforeSolve(bool prediction, float frameTime)
	{
	}

	public virtual void UpdateAfterSolve(bool prediction, float frameTime)
	{
	}
}
