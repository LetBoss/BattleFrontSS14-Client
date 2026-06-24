using System;
using Content.Shared._PUBG.Loadout;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;

namespace Content.Client._PUBG.Loadout;

public sealed class PubgWeaponModuleVisualsSystem : VisualizerSystem<PubgWeaponModuleVisualsComponent>
{
	protected override void OnAppearanceChange(EntityUid uid, PubgWeaponModuleVisualsComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateLayer(uid, args, component.OpticLayer, PubgWeaponModuleVisuals.Optic);
			UpdateLayer(uid, args, component.MuzzleLayer, PubgWeaponModuleVisuals.Muzzle);
			UpdateLayer(uid, args, component.MagazineLayer, PubgWeaponModuleVisuals.Magazine);
		}
	}

	private void UpdateLayer(EntityUid uid, AppearanceChangeEvent args, string layerKey, PubgWeaponModuleVisuals visualKey)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		bool flag = default(bool);
		if (!string.IsNullOrWhiteSpace(layerKey) && args.Sprite != null && base.SpriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), layerKey, ref num, false) && ((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)visualKey, ref flag, args.Component))
		{
			base.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), layerKey, flag);
		}
	}
}
