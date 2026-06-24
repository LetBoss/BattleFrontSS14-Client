using System;
using Content.Shared.Overlays;
using Content.Shared.Security.Components;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowCriminalRecordIconsSystem : EquipmentHudSystem<ShowCriminalRecordIconsComponent>
{
	[Dependency]
	private IPrototypeManager _prototype;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CriminalRecordComponent, GetStatusIconsEvent>((ComponentEventRefHandler<CriminalRecordComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIconsEvent(EntityUid uid, CriminalRecordComponent component, ref GetStatusIconsEvent ev)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SecurityIconPrototype item = default(SecurityIconPrototype);
		if (IsActive && _prototype.TryIndex<SecurityIconPrototype>(component.StatusIcon, ref item))
		{
			ev.StatusIcons.Add(item);
		}
	}
}
