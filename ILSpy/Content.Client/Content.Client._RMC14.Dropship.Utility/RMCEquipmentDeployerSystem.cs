using System;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Content.Shared._RMC14.Dropship.Utility.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Dropship.Utility;

public sealed class RMCEquipmentDeployerSystem : SharedRMCEquipmentDeployerSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCEquipmentDeployerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<RMCEquipmentDeployerComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCEquipmentDeployerComponent, AppearanceChangeEvent>((EntityEventRefHandler<RMCEquipmentDeployerComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnHandleState(Entity<RMCEquipmentDeployerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(ent);
	}

	private void OnAppearanceChange(Entity<RMCEquipmentDeployerComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(ent);
	}

	private void UpdateVisuals(Entity<RMCEquipmentDeployerComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = ent.Owner;
		int num = default(int);
		if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(owner), (Enum)EquipmentDeployState.UnDeployed, ref num, false))
		{
			int num2 = default(int);
			if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit(owner), (Enum)EquipmentDeployState.Deployed, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(owner), num, true);
				return;
			}
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(owner), num, !ent.Comp.IsDeployed);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit(owner), num2, ent.Comp.IsDeployed);
		}
	}
}
