using System;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Inventory.Events;
using Content.Shared.Overlays;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client.Overlays;

public sealed class ShowHealthBarsSystem : EquipmentHudSystem<ShowHealthBarsComponent>
{
	[Dependency]
	private IOverlayManager _overlayMan;

	[Dependency]
	private IPrototypeManager _prototype;

	private EntityHealthBarOverlay _overlay;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ShowHealthBarsComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ShowHealthBarsComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		_overlay = new EntityHealthBarOverlay((IEntityManager)(object)((EntitySystem)this).EntityManager, _prototype);
	}

	private void OnHandleState(Entity<ShowHealthBarsComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		RefreshOverlay();
	}

	protected override void UpdateInternal(RefreshEquipmentHudEvent<ShowHealthBarsComponent> component)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateInternal(component);
		foreach (ShowHealthBarsComponent component2 in component.Components)
		{
			foreach (ProtoId<DamageContainerPrototype> damageContainer in component2.DamageContainers)
			{
				_overlay.DamageContainers.Add(ProtoId<DamageContainerPrototype>.op_Implicit(damageContainer));
			}
			_overlay.StatusIcon = component2.HealthStatusIcon;
		}
		if (!_overlayMan.HasOverlay<EntityHealthBarOverlay>())
		{
			_overlayMan.AddOverlay((Overlay)(object)_overlay);
		}
	}

	protected override void DeactivateInternal()
	{
		base.DeactivateInternal();
		_overlay.DamageContainers.Clear();
		_overlayMan.RemoveOverlay((Overlay)(object)_overlay);
	}
}
