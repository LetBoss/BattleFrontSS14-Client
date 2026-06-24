using System;
using Content.Shared._RMC14.Dropship.AttachmentPoint;
using Content.Shared._RMC14.Dropship.Utility.Components;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Dropship.Utility;

public sealed class DropshipPointVisualizerSystem : VisualizerSystem<DropshipPointVisualsComponent>
{
	[Dependency]
	private SpriteSystem _sprite;

	protected override void OnAppearanceChange(EntityUid uid, DropshipPointVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		base.OnAppearanceChange(uid, component, ref args);
		SpriteComponent sprite = args.Sprite;
		string text = default(string);
		string text2 = default(string);
		int num = default(int);
		if (sprite != null && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)DropshipUtilityVisuals.Sprite, ref text, args.Component) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<string>(uid, (Enum)DropshipUtilityVisuals.State, ref text2, args.Component) && _sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)DropshipPointVisualsLayers.AttachmentBase, ref num, false))
		{
			int num2 = default(int);
			if (!_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum)DropshipPointVisualsLayers.AttachedUtility, ref num2, false))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
			}
			else if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(text2))
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, true);
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, false);
			}
			else
			{
				_sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, (SpriteSpecifier)new Rsi(new ResPath(text), text2));
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num, false);
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), num2, true);
			}
		}
	}
}
