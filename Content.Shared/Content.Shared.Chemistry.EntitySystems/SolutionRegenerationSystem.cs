using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.FixedPoint;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionRegenerationSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SolutionRegenerationComponent, MapInitEvent>((EntityEventRefHandler<SolutionRegenerationComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SolutionRegenerationComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<SolutionRegenerationComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<SolutionRegenerationComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextRegenTime = _timing.CurTime + ent.Comp.Duration;
		((EntitySystem)this).Dirty<SolutionRegenerationComponent>(ent, (MetaDataComponent)null);
	}

	private void OnEntRemoved(Entity<SolutionRegenerationComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityUid entity = ((ContainerModifiedMessage)args).Entity;
		EntityUid? val = ent.Comp.SolutionRef?.Owner;
		if (val.HasValue && entity == val.GetValueOrDefault())
		{
			ent.Comp.SolutionRef = null;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<SolutionRegenerationComponent, SolutionContainerManagerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SolutionRegenerationComponent, SolutionContainerManagerComponent>();
		EntityUid uid = default(EntityUid);
		SolutionRegenerationComponent regen = default(SolutionRegenerationComponent);
		SolutionContainerManagerComponent manager = default(SolutionContainerManagerComponent);
		while (query.MoveNext(ref uid, ref regen, ref manager))
		{
			if (_timing.CurTime < regen.NextRegenTime)
			{
				continue;
			}
			regen.NextRegenTime += regen.Duration;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)regen, (MetaDataComponent)null);
			if (_solutionContainer.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, manager)), regen.SolutionName, ref regen.SolutionRef, out Solution solution))
			{
				FixedPoint2 amount = FixedPoint2.Min(solution.AvailableVolume, regen.Generated.Volume);
				if (!(amount <= FixedPoint2.Zero))
				{
					Solution generated = ((amount == regen.Generated.Volume) ? regen.Generated : regen.Generated.Clone().SplitSolution(amount));
					_solutionContainer.TryAddSolution(regen.SolutionRef.Value, generated);
				}
			}
		}
	}
}
