using System;
using Content.Client.Botany.Components;
using Content.Shared.Botany;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client.Botany;

public sealed class PlantHolderVisualizerSystem : VisualizerSystem<PlantHolderVisualsComponent>
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PlantHolderVisualsComponent, ComponentInit>((ComponentEventHandler<PlantHolderVisualsComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
	}

	private void OnComponentInit(EntityUid uid, PlantHolderVisualsComponent component, ComponentInit args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(uid, ref item))
		{
			base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)PlantHolderLayers.Plant);
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, item)), (Enum)PlantHolderLayers.Plant, false);
		}
	}

	protected override void OnAppearanceChange(EntityUid uid, PlantHolderVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		string text2 = default(string);
		if (args.Sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)PlantHolderVisuals.PlantRsi, ref text, args.Component) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)PlantHolderVisuals.PlantState, ref text2, args.Component))
		{
			bool flag = !string.IsNullOrWhiteSpace(text2);
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PlantHolderLayers.Plant, flag);
			if (flag)
			{
				base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PlantHolderLayers.Plant, new ResPath(text), (StateId?)null);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)PlantHolderLayers.Plant, StateId.op_Implicit(text2));
			}
		}
	}
}
