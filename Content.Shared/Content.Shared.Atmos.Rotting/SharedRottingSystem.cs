using System;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Rejuvenate;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Atmos.Rotting;

public abstract class SharedRottingSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private MobStateSystem _mobState;

	public const int MaxStages = 3;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PerishableComponent, MapInitEvent>((ComponentEventHandler<PerishableComponent, MapInitEvent>)OnPerishableMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PerishableComponent, MobStateChangedEvent>((ComponentEventHandler<PerishableComponent, MobStateChangedEvent>)OnMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PerishableComponent, ExaminedEvent>((EntityEventRefHandler<PerishableComponent, ExaminedEvent>)OnPerishableExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RottingComponent, ComponentShutdown>((ComponentEventHandler<RottingComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RottingComponent, MobStateChangedEvent>((ComponentEventHandler<RottingComponent, MobStateChangedEvent>)OnRottingMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RottingComponent, RejuvenateEvent>((ComponentEventHandler<RottingComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RottingComponent, ExaminedEvent>((ComponentEventHandler<RottingComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnPerishableMapInit(EntityUid uid, PerishableComponent component, MapInitEvent args)
	{
		component.RotNextUpdate = _timing.CurTime + component.PerishUpdateRate;
	}

	private void OnMobStateChanged(EntityUid uid, PerishableComponent component, MobStateChangedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if ((args.NewMobState == MobState.Dead || args.OldMobState == MobState.Dead) && !((EntitySystem)this).HasComp<RottingComponent>(uid))
		{
			component.RotAccumulator = TimeSpan.Zero;
			component.RotNextUpdate = _timing.CurTime + component.PerishUpdateRate;
		}
	}

	private void OnPerishableExamined(Entity<PerishableComponent> perishable, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		int stage = PerishStage(perishable, 3);
		if (stage >= 1 && stage <= 3)
		{
			bool isMob = ((EntitySystem)this).HasComp<MobStateComponent>(Entity<PerishableComponent>.op_Implicit(perishable));
			string description = "perishable-" + stage + ((!isMob) ? "-nonmob" : string.Empty);
			args.PushMarkup(base.Loc.GetString(description, (ValueTuple<string, object>)("target", Identity.Entity(Entity<PerishableComponent>.op_Implicit(perishable), (IEntityManager)(object)base.EntityManager))));
		}
	}

	private void OnShutdown(EntityUid uid, RottingComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		PerishableComponent perishable = default(PerishableComponent);
		if (((EntitySystem)this).TryComp<PerishableComponent>(uid, ref perishable))
		{
			perishable.RotNextUpdate = TimeSpan.Zero;
		}
	}

	private void OnRottingMobStateChanged(EntityUid uid, RottingComponent component, MobStateChangedEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Dead)
		{
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)component);
		}
	}

	private void OnRejuvenate(EntityUid uid, RottingComponent component, RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemCompDeferred<RottingComponent>(uid);
	}

	private void OnExamined(EntityUid uid, RottingComponent component, ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		int stage = RotStage(uid, component);
		string text = ((stage >= 2) ? "rotting-extremely-bloated" : ((stage < 1) ? "rotting-rotting" : "rotting-bloated"));
		string description = text;
		if (!((EntitySystem)this).HasComp<MobStateComponent>(uid))
		{
			description += "-nonmob";
		}
		args.PushMarkup(base.Loc.GetString(description, (ValueTuple<string, object>)("target", Identity.Entity(uid, (IEntityManager)(object)base.EntityManager))));
	}

	public int PerishStage(Entity<PerishableComponent> perishable, int maxStages)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (perishable.Comp.RotAfter.TotalSeconds == 0.0 || perishable.Comp.RotAccumulator.TotalSeconds == 0.0)
		{
			return 0;
		}
		return (int)(1.0 + (double)maxStages * perishable.Comp.RotAccumulator.TotalSeconds / perishable.Comp.RotAfter.TotalSeconds);
	}

	public bool IsRotProgressing(EntityUid uid, PerishableComponent? perishable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PerishableComponent>(uid, ref perishable, false))
		{
			return false;
		}
		if (perishable.ForceRotProgression)
		{
			return true;
		}
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(uid, ref mobState) && !_mobState.IsDead(uid, mobState))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (_container.TryGetOuterContainer(uid, ((EntitySystem)this).Transform(uid), ref container) && ((EntitySystem)this).HasComp<AntiRottingContainerComponent>(container.Owner))
		{
			return false;
		}
		IsRottingEvent ev = default(IsRottingEvent);
		((EntitySystem)this).RaiseLocalEvent<IsRottingEvent>(uid, ref ev, false);
		return !ev.Handled;
	}

	public bool IsRotten(EntityUid uid, RottingComponent? rotting = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return ((EntitySystem)this).Resolve<RottingComponent>(uid, ref rotting, false);
	}

	public void ReduceAccumulator(EntityUid uid, TimeSpan time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		PerishableComponent perishable = default(PerishableComponent);
		if (!((EntitySystem)this).TryComp<PerishableComponent>(uid, ref perishable))
		{
			return;
		}
		RottingComponent rotting = default(RottingComponent);
		if (!((EntitySystem)this).TryComp<RottingComponent>(uid, ref rotting))
		{
			perishable.RotAccumulator -= time;
			return;
		}
		TimeSpan total = rotting.TotalRotTime + perishable.RotAccumulator - time;
		if (total < perishable.RotAfter)
		{
			((EntitySystem)this).RemCompDeferred(uid, (IComponent)(object)rotting);
			perishable.RotAccumulator = total;
		}
		else
		{
			rotting.TotalRotTime = total - perishable.RotAfter;
		}
	}

	public int RotStage(EntityUid uid, RottingComponent? comp = null, PerishableComponent? perishable = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<RottingComponent, PerishableComponent>(uid, ref comp, ref perishable, true))
		{
			return 0;
		}
		return (int)(comp.TotalRotTime.TotalSeconds / perishable.RotAfter.TotalSeconds);
	}
}
