using System;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Components.Localization;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.TrainingDummy;

public sealed class RMCTrainingDummySystem : EntitySystem
{
	[Dependency]
	private GrammarSystem _grammarSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCTrainingDummyComponent, ComponentStartup>((EntityEventRefHandler<RMCTrainingDummyComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<RMCTrainingDummyComponent> ent, ref ComponentStartup args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.RemoveComponents != null)
		{
			base.EntityManager.RemoveComponents(ent.Owner, ent.Comp.RemoveComponents);
		}
		GrammarComponent grammar = default(GrammarComponent);
		if (((EntitySystem)this).TryComp<GrammarComponent>(ent.Owner, ref grammar))
		{
			_grammarSystem.SetGender(Entity<GrammarComponent>.op_Implicit((ent.Owner, grammar)), (Gender?)(Gender)0);
			_grammarSystem.SetProperNoun(Entity<GrammarComponent>.op_Implicit((ent.Owner, grammar)), (bool?)false);
		}
	}
}
