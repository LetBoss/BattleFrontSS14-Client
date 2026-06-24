using System;
using Content.Shared.CriminalRecords.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.CriminalRecords.Systems;

public abstract class SharedCriminalRecordsHackerSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedNinjaGlovesSystem _gloves;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CriminalRecordsHackerComponent, BeforeInteractHandEvent>((EntityEventRefHandler<CriminalRecordsHackerComponent, BeforeInteractHandEvent>)OnBeforeInteractHand, (Type[])null, (Type[])null);
	}

	private void OnBeforeInteractHand(Entity<CriminalRecordsHackerComponent> ent, ref BeforeInteractHandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _gloves.AbilityCheck(Entity<CriminalRecordsHackerComponent>.op_Implicit(ent), args, out var target) && ((EntitySystem)this).HasComp<CriminalRecordsConsoleComponent>(target))
		{
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<CriminalRecordsHackerComponent>.op_Implicit(ent), ent.Comp.Delay, new CriminalRecordsHackDoAfterEvent(), target: target, used: Entity<CriminalRecordsHackerComponent>.op_Implicit(ent), eventTarget: Entity<CriminalRecordsHackerComponent>.op_Implicit(ent))
			{
				BreakOnDamage = true,
				BreakOnMove = true,
				MovementThreshold = 0.5f
			};
			_doAfter.TryStartDoAfter(doAfterArgs);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}
}
