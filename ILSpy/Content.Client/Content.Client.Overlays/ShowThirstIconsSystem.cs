using System;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Overlays;

public sealed class ShowThirstIconsSystem : EquipmentHudSystem<ShowThirstIconsComponent>
{
	[Dependency]
	private ThirstSystem _thirst;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ThirstComponent, GetStatusIconsEvent>((ComponentEventRefHandler<ThirstComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIconsEvent(EntityUid uid, ThirstComponent component, ref GetStatusIconsEvent ev)
	{
		if (IsActive && _thirst.TryGetStatusIconPrototype(component, out SatiationIconPrototype prototype))
		{
			ev.StatusIcons.Add(prototype);
		}
	}
}
