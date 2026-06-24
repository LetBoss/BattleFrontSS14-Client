using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Implants;
using Content.Shared.Implants.Components;
using Content.Shared.Mindshield.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Mindshield.FakeMindShield;

public sealed class SharedFakeMindShieldImplantSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actionsSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<SubdermalImplantComponent, FakeMindShieldToggleEvent>((EntityEventRefHandler<SubdermalImplantComponent, FakeMindShieldToggleEvent>)OnFakeMindShieldToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FakeMindShieldImplantComponent, ImplantImplantedEvent>((ComponentEventRefHandler<FakeMindShieldImplantComponent, ImplantImplantedEvent>)ImplantCheck, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FakeMindShieldImplantComponent, EntGotRemovedFromContainerMessage>((EntityEventRefHandler<FakeMindShieldImplantComponent, EntGotRemovedFromContainerMessage>)ImplantDraw, (Type[])null, (Type[])null);
	}

	private void OnFakeMindShieldToggle(Entity<SubdermalImplantComponent> entity, ref FakeMindShieldToggleEvent ev)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)ev).Handled = true;
		EntityUid? implantedEntity = entity.Comp.ImplantedEntity;
		if (implantedEntity.HasValue)
		{
			EntityUid ent = implantedEntity.GetValueOrDefault();
			FakeMindShieldComponent comp = default(FakeMindShieldComponent);
			if (((EntitySystem)this).TryComp<FakeMindShieldComponent>(ent, ref comp))
			{
				_actionsSystem.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(ev.Action), Entity<ActionComponent>.op_Implicit(ev.Action))), !comp.IsEnabled);
				((EntitySystem)this).RaiseLocalEvent<FakeMindShieldToggleEvent>(ent, ev, false);
			}
		}
	}

	private void ImplantCheck(EntityUid uid, FakeMindShieldImplantComponent component, ref ImplantImplantedEvent ev)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (ev.Implanted.HasValue)
		{
			((EntitySystem)this).EnsureComp<FakeMindShieldComponent>(ev.Implanted.Value);
		}
	}

	private void ImplantDraw(Entity<FakeMindShieldImplantComponent> ent, ref EntGotRemovedFromContainerMessage ev)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RemComp<FakeMindShieldComponent>(((ContainerModifiedMessage)ev).Container.Owner);
	}
}
