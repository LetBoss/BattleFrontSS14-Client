using System;
using Content.Shared.Mindshield.Components;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowMindShieldIconsSystem : EquipmentHudSystem<ShowMindShieldIconsComponent>
{
	[Dependency]
	private IPrototypeManager _prototype;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MindShieldComponent, GetStatusIconsEvent>((ComponentEventRefHandler<MindShieldComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FakeMindShieldComponent, GetStatusIconsEvent>((ComponentEventRefHandler<FakeMindShieldComponent, GetStatusIconsEvent>)OnGetStatusIconsEventFake, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIconsEventFake(EntityUid uid, FakeMindShieldComponent component, ref GetStatusIconsEvent ev)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		SecurityIconPrototype item = default(SecurityIconPrototype);
		if (IsActive && component.IsEnabled && _prototype.TryIndex<SecurityIconPrototype>(component.MindShieldStatusIcon, ref item))
		{
			ev.StatusIcons.Add(item);
		}
	}

	private void OnGetStatusIconsEvent(EntityUid uid, MindShieldComponent component, ref GetStatusIconsEvent ev)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SecurityIconPrototype item = default(SecurityIconPrototype);
		if (IsActive && _prototype.TryIndex<SecurityIconPrototype>(component.MindShieldStatusIcon, ref item))
		{
			ev.StatusIcons.Add(item);
		}
	}
}
