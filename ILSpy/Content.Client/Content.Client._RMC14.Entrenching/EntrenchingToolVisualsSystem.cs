using System;
using Content.Shared._RMC14.Entrenching;
using Content.Shared.Toggleable;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Entrenching;

public sealed class EntrenchingToolVisualsSystem : EntitySystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<EntrenchingToolComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<EntrenchingToolComponent, AppearanceChangeEvent>((EntityEventRefHandler<EntrenchingToolComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnHandleState(Entity<EntrenchingToolComponent> tool, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(tool);
	}

	private void OnAppearanceChange(Entity<EntrenchingToolComponent> tool, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(tool);
	}

	private void UpdateVisuals(Entity<EntrenchingToolComponent> tool)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<EntrenchingToolComponent>.op_Implicit(tool), ref item))
		{
			return;
		}
		bool flag = default(bool);
		if (((SharedAppearanceSystem)_appearance).TryGetData<bool>(Entity<EntrenchingToolComponent>.op_Implicit(tool), (Enum)ToggleableVisuals.Enabled, ref flag, (AppearanceComponent)null) && flag)
		{
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Base, ref num, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num, true);
			}
			int num2 = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Folded, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num2, false);
			}
			int num4 = default(int);
			if (tool.Comp.TotalLayers > 0)
			{
				int num3 = default(int);
				if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Dirt, ref num3, false))
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num3, true);
					_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num3, Color.FromHex((ReadOnlySpan<char>)"#C04000", (Color?)null));
				}
				else
				{
					_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num3, false);
				}
			}
			else if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Dirt, ref num4, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num4, false);
			}
		}
		else
		{
			int num5 = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Base, ref num5, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num5, false);
			}
			int num6 = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Folded, ref num6, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num6, true);
			}
			int num7 = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), (Enum)EntrenchingToolComponentVisualLayers.Dirt, ref num7, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((tool.Owner, item)), num7, false);
			}
		}
	}
}
