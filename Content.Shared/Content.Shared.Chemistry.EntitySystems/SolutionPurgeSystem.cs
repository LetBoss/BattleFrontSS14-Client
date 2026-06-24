using System;
using System.Linq;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Chemistry.EntitySystems;

public sealed class SolutionPurgeSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solutionContainer;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SolutionPurgeComponent, MapInitEvent>((EntityEventRefHandler<SolutionPurgeComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<SolutionPurgeComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextPurgeTime = _timing.CurTime + ent.Comp.Duration;
		((EntitySystem)this).Dirty<SolutionPurgeComponent>(ent, (MetaDataComponent)null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<SolutionPurgeComponent, SolutionContainerManagerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SolutionPurgeComponent, SolutionContainerManagerComponent>();
		EntityUid uid = default(EntityUid);
		SolutionPurgeComponent purge = default(SolutionPurgeComponent);
		SolutionContainerManagerComponent manager = default(SolutionContainerManagerComponent);
		while (query.MoveNext(ref uid, ref purge, ref manager))
		{
			if (_timing.CurTime < purge.NextPurgeTime)
			{
				continue;
			}
			purge.NextPurgeTime += purge.Duration;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)purge, (MetaDataComponent)null);
			if (_solutionContainer.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, manager)), purge.Solution, out Entity<SolutionComponent>? solution))
			{
				_solutionContainer.SplitSolutionWithout(solution.Value, purge.Quantity, purge.Preserve.Select<ProtoId<ReagentPrototype>, string>((ProtoId<ReagentPrototype> proto) => proto.Id).ToArray());
			}
		}
	}
}
