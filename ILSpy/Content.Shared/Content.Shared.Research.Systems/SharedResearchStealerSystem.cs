using System;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Ninja.Systems;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Research.Systems;

public abstract class SharedResearchStealerSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedNinjaGlovesSystem _gloves;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ResearchStealerComponent, BeforeInteractHandEvent>((ComponentEventHandler<ResearchStealerComponent, BeforeInteractHandEvent>)OnBeforeInteractHand, (Type[])null, (Type[])null);
	}

	private void OnBeforeInteractHand(EntityUid uid, ResearchStealerComponent comp, BeforeInteractHandEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		TechnologyDatabaseComponent database = default(TechnologyDatabaseComponent);
		if (!((HandledEntityEventArgs)args).Handled && _gloves.AbilityCheck(uid, args, out var target) && ((EntitySystem)this).TryComp<TechnologyDatabaseComponent>(target, ref database) && !((EntitySystem)this).HasComp<ResearchClientComponent>(target))
		{
			((HandledEntityEventArgs)args).Handled = true;
			if (database.UnlockedTechnologies.Count == 0)
			{
				_popup.PopupClient(base.Loc.GetString("ninja-download-fail"), uid, uid);
				return;
			}
			DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, uid, comp.Delay, new ResearchStealDoAfterEvent(), target: target, used: uid, eventTarget: uid)
			{
				BreakOnDamage = true,
				BreakOnMove = true,
				MovementThreshold = 0.5f
			};
			_doAfter.TryStartDoAfter(doAfterArgs);
		}
	}
}
