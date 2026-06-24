using System;
using System.Collections.Generic;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Overlays;
using Content.Shared.PDA;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowJobIconsSystem : EquipmentHudSystem<ShowJobIconsComponent>
{
	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private AccessReaderSystem _accessReader;

	private static readonly ProtoId<JobIconPrototype> JobIconForNoId = ProtoId<JobIconPrototype>.op_Implicit("JobIconNoId");

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StatusIconComponent, GetStatusIconsEvent>((ComponentEventRefHandler<StatusIconComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
	}

	private void OnGetStatusIconsEvent(EntityUid uid, StatusIconComponent _, ref GetStatusIconsEvent ev)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!IsActive)
		{
			return;
		}
		ProtoId<JobIconPrototype> val = JobIconForNoId;
		if (_accessReader.FindAccessItemsInventory(uid, out HashSet<EntityUid> items))
		{
			IdCardComponent idCardComponent = default(IdCardComponent);
			PdaComponent pdaComponent = default(PdaComponent);
			foreach (EntityUid item in items)
			{
				if (((EntitySystem)this).TryComp<IdCardComponent>(item, ref idCardComponent))
				{
					val = idCardComponent.JobIcon;
					break;
				}
				if (((EntitySystem)this).TryComp<PdaComponent>(item, ref pdaComponent) && pdaComponent.ContainedId.HasValue && ((EntitySystem)this).TryComp<IdCardComponent>(pdaComponent.ContainedId, ref idCardComponent))
				{
					val = idCardComponent.JobIcon;
					break;
				}
			}
		}
		JobIconPrototype jobIconPrototype = default(JobIconPrototype);
		if (_prototype.TryIndex<JobIconPrototype>(val, ref jobIconPrototype))
		{
			ev.StatusIcons.Add(jobIconPrototype);
			return;
		}
		((EntitySystem)this).Log.Error($"Invalid job icon prototype: {jobIconPrototype}");
	}
}
