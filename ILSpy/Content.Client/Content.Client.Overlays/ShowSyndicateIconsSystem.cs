using System;
using Content.Shared.NukeOps;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowSyndicateIconsSystem : EquipmentHudSystem<ShowSyndicateIconsComponent>
{
	[Dependency]
	private IPrototypeManager _prototype;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<NukeOperativeComponent, GetStatusIconsEvent>((ComponentEventRefHandler<NukeOperativeComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIconsEvent(EntityUid uid, NukeOperativeComponent component, ref GetStatusIconsEvent ev)
	{
		FactionIconPrototype item = default(FactionIconPrototype);
		if (IsActive && _prototype.TryIndex<FactionIconPrototype>(component.SyndStatusIcon, ref item))
		{
			ev.StatusIcons.Add(item);
		}
	}
}
