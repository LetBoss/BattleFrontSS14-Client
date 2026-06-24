using System;
using Content.Shared._RMC14.Weapons.Ranged;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Weapons.Ranged;

public sealed class BulletholeVisualizerSystem : VisualizerSystem<BulletholeComponent>
{
	private const string BulletholeRsiPath = "/Textures/_RMC14/Effects/bulletholes.rsi";

	protected override void OnAppearanceChange(EntityUid uid, BulletholeComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent sprite = args.Sprite;
		string text = default(string);
		if (sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)BulletholeVisuals.State, ref text, args.Component))
		{
			int num = default(int);
			if (!base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BulletholeVisualsLayers.Bullethole, ref num, false))
			{
				num = base.SpriteSystem.LayerMapReserve(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BulletholeVisualsLayers.Bullethole);
			}
			bool flag = !string.IsNullOrWhiteSpace(text);
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BulletholeVisualsLayers.Bullethole, flag);
			if (flag)
			{
				base.SpriteSystem.LayerSetRsi(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BulletholeVisualsLayers.Bullethole, new ResPath("/Textures/_RMC14/Effects/bulletholes.rsi"), (StateId?)null);
				base.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)BulletholeVisualsLayers.Bullethole, StateId.op_Implicit(text));
			}
		}
	}
}
