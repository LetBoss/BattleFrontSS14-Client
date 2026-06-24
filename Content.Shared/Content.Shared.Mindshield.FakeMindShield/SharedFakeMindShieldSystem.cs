using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Implants;
using Content.Shared.Mindshield.Components;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Mindshield.FakeMindShield;

public sealed class SharedFakeMindShieldSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private IGameTiming _timing;

	private static readonly ProtoId<TagPrototype> FakeMindShieldImplantTag = ProtoId<TagPrototype>.op_Implicit("FakeMindShieldImplant");

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FakeMindShieldComponent, FakeMindShieldToggleEvent>((ComponentEventHandler<FakeMindShieldComponent, FakeMindShieldToggleEvent>)OnToggleMindshield, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FakeMindShieldComponent, ChameleonControllerOutfitSelectedEvent>((ComponentEventHandler<FakeMindShieldComponent, ChameleonControllerOutfitSelectedEvent>)OnChameleonControllerOutfitSelected, (Type[])null, (Type[])null);
	}

	private void OnToggleMindshield(EntityUid uid, FakeMindShieldComponent comp, FakeMindShieldToggleEvent toggleEvent)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		comp.IsEnabled = !comp.IsEnabled;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
	}

	private void OnChameleonControllerOutfitSelected(EntityUid uid, FakeMindShieldComponent component, ChameleonControllerOutfitSelectedEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		ActionsComponent actionsComp = default(ActionsComponent);
		if (component.IsEnabled == args.ChameleonOutfit.HasMindShield || !((EntitySystem)this).TryComp<ActionsComponent>(uid, ref actionsComp))
		{
			return;
		}
		bool actionFound = false;
		ActionComponent actionComp = default(ActionComponent);
		foreach (EntityUid action in actionsComp.Actions)
		{
			if (!_tag.HasTag(action, FakeMindShieldImplantTag) || !((EntitySystem)this).TryComp<ActionComponent>(action, ref actionComp))
			{
				continue;
			}
			actionFound = true;
			if (!_actions.IsCooldownActive(actionComp, _timing.CurTime))
			{
				component.IsEnabled = args.ChameleonOutfit.HasMindShield;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
				if (actionComp.UseDelay.HasValue)
				{
					_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(action), actionComp.UseDelay.Value);
				}
				return;
			}
		}
		if (!actionFound)
		{
			component.IsEnabled = args.ChameleonOutfit.HasMindShield;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
	}
}
