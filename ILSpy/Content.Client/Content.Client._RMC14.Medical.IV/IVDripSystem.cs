using System;
using Content.Shared._RMC14.Medical.IV;
using Content.Shared.Rounding;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Medical.IV;

public sealed class IVDripSystem : SharedIVDripSystem
{
	[Dependency]
	private IOverlayManager _overlay;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		if (!_overlay.HasOverlay<IVDripOverlay>())
		{
			_overlay.AddOverlay((Overlay)(object)new IVDripOverlay());
		}
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		_overlay.RemoveOverlay<IVDripOverlay>();
	}

	protected override void UpdateIVAppearance(Entity<IVDripComponent> iv)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateIVAppearance(iv);
		SpriteComponent item = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<IVDripComponent>.op_Implicit(iv), ref item))
		{
			return;
		}
		string text = ((!iv.Comp.AttachedTo.HasValue) ? iv.Comp.UnattachedState : iv.Comp.AttachedState);
		_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((iv.Owner, item)), (Enum)IVDripVisualLayers.Base, StateId.op_Implicit(text));
		string text2 = null;
		for (int num = iv.Comp.ReagentStates.Count - 1; num >= 0; num--)
		{
			var (num2, text3) = iv.Comp.ReagentStates[num];
			if (num2 <= iv.Comp.FillPercentage)
			{
				text2 = text3;
				break;
			}
		}
		if (text2 == null)
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((iv.Owner, item)), (Enum)IVDripVisualLayers.Reagent, false);
			return;
		}
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((iv.Owner, item)), (Enum)IVDripVisualLayers.Reagent, true);
		_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((iv.Owner, item)), (Enum)IVDripVisualLayers.Reagent, StateId.op_Implicit(text2));
		_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((iv.Owner, item)), (Enum)IVDripVisualLayers.Reagent, iv.Comp.FillColor);
	}

	protected override void UpdatePackAppearance(Entity<BloodPackComponent> pack)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		base.UpdatePackAppearance(pack);
		SpriteComponent item = default(SpriteComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<BloodPackComponent>.op_Implicit(pack), ref item))
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((pack.Owner, item)), (Enum)BloodPackVisuals.Label, false);
			int num = default(int);
			if (_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((pack.Owner, item)), (Enum)BloodPackVisuals.Fill, ref num, false))
			{
				int num2 = ContentHelpers.RoundToLevels(pack.Comp.FillPercentage.Float(), 1.0, pack.Comp.MaxFillLevels + 1);
				string text = ((num2 > 0) ? $"{pack.Comp.FillBaseName}{num2}" : pack.Comp.FillBaseName);
				_sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((pack.Owner, item)), num, StateId.op_Implicit(text));
				_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((pack.Owner, item)), num, pack.Comp.FillColor);
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((pack.Owner, item)), num, true);
			}
		}
	}
}
