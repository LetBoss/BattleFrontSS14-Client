using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._RMC14.Medical.HUD;
using Content.Shared.Atmos.Rotting;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Overlays;
using Content.Shared.StatusIcon;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Overlays;

public sealed class ShowHealthIconsSystem : EquipmentHudSystem<ShowHealthIconsComponent>
{
	[Dependency]
	private IPrototypeManager _prototypeMan;

	[Dependency]
	private CMHealthIconsSystem _healthIcons;

	[ViewVariables]
	public HashSet<string> DamageContainers = new HashSet<string>();

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageableComponent, GetStatusIconsEvent>((EntityEventRefHandler<DamageableComponent, GetStatusIconsEvent>)OnGetStatusIconsEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShowHealthIconsComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ShowHealthIconsComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
	}

	protected override void UpdateInternal(RefreshEquipmentHudEvent<ShowHealthIconsComponent> component)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateInternal(component);
		foreach (ProtoId<DamageContainerPrototype> item in component.Components.SelectMany((ShowHealthIconsComponent x) => x.DamageContainers))
		{
			DamageContainers.Add(ProtoId<DamageContainerPrototype>.op_Implicit(item));
		}
	}

	protected override void DeactivateInternal()
	{
		base.DeactivateInternal();
		DamageContainers.Clear();
	}

	private void OnHandleState(Entity<ShowHealthIconsComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		RefreshOverlay();
	}

	private void OnGetStatusIconsEvent(Entity<DamageableComponent> entity, ref GetStatusIconsEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (IsActive)
		{
			IReadOnlyList<StatusIconData> icons = _healthIcons.GetIcons(entity);
			args.StatusIcons.AddRange(icons);
		}
	}

	private IReadOnlyList<HealthIconPrototype> DecideHealthIcons(Entity<DamageableComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent comp = entity.Comp;
		if (comp.DamageContainerID.HasValue)
		{
			HashSet<string> damageContainers = DamageContainers;
			ProtoId<DamageContainerPrototype>? damageContainerID = comp.DamageContainerID;
			if (damageContainers.Contains(damageContainerID.HasValue ? ProtoId<DamageContainerPrototype>.op_Implicit(damageContainerID.GetValueOrDefault()) : null))
			{
				List<HealthIconPrototype> list = new List<HealthIconPrototype>();
				damageContainerID = comp?.DamageContainerID;
				ProtoId<DamageContainerPrototype>? val = ProtoId<DamageContainerPrototype>.op_Implicit("Biological");
				MobStateComponent mobStateComponent = default(MobStateComponent);
				if (damageContainerID.HasValue == val.HasValue && (!damageContainerID.HasValue || damageContainerID.GetValueOrDefault() == val.GetValueOrDefault()) && ((EntitySystem)this).TryComp<MobStateComponent>(Entity<DamageableComponent>.op_Implicit(entity), ref mobStateComponent))
				{
					HealthIconPrototype item = default(HealthIconPrototype);
					ProtoId<HealthIconPrototype> value;
					HealthIconPrototype item2 = default(HealthIconPrototype);
					if (((EntitySystem)this).HasComp<RottingComponent>(Entity<DamageableComponent>.op_Implicit(entity)) && _prototypeMan.TryIndex<HealthIconPrototype>(comp.RottingIcon, ref item))
					{
						list.Add(item);
					}
					else if (comp.HealthIcons.TryGetValue(mobStateComponent.CurrentState, out value) && _prototypeMan.TryIndex<HealthIconPrototype>(value, ref item2))
					{
						list.Add(item2);
					}
				}
				return list;
			}
		}
		return Array.Empty<HealthIconPrototype>();
	}
}
