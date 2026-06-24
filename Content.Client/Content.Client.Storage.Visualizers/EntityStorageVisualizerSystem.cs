using System;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;

namespace Content.Client.Storage.Visualizers;

public sealed class EntityStorageVisualizerSystem : VisualizerSystem<EntityStorageVisualsComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<EntityStorageVisualsComponent, ComponentInit>((ComponentEventHandler<EntityStorageVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, EntityStorageVisualsComponent comp, ComponentInit args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (comp.StateBaseClosed != null)
		{
			if (comp.StateBaseOpen == null)
			{
				comp.StateBaseOpen = comp.StateBaseClosed;
			}
			SpriteComponent item = default(SpriteComponent);
			if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)StorageVisualLayers.Base, StateId.op_Implicit(comp.StateBaseClosed));
			}
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, EntityStorageVisualsComponent comp, ref AppearanceChangeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		int num = default(int);
		if (args.Sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)StorageVisuals.Open, ref flag, args.Component) || !base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, ref num, false))
		{
			return;
		}
		if (flag)
		{
			if (comp.OpenDrawDepth.HasValue)
			{
				base.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), comp.OpenDrawDepth.Value);
			}
			if (comp.StateDoorOpen != null)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, StateId.op_Implicit(comp.StateDoorOpen));
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, true);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, false);
			}
			if (comp.StateBaseOpen != null)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Base, StateId.op_Implicit(comp.StateBaseOpen));
			}
		}
		else
		{
			if (comp.ClosedDrawDepth.HasValue)
			{
				base.SpriteSystem.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), comp.ClosedDrawDepth.Value);
			}
			if (comp.StateDoorClosed != null)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, StateId.op_Implicit(comp.StateDoorClosed));
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, true);
			}
			else
			{
				base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Door, false);
			}
			if (comp.StateBaseClosed != null)
			{
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)StorageVisualLayers.Base, StateId.op_Implicit(comp.StateBaseClosed));
			}
		}
	}
}
