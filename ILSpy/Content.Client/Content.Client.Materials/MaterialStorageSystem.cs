using System;
using Content.Shared.Materials;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Materials;

public sealed class MaterialStorageSystem : SharedMaterialStorageSystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private TransformSystem _transform;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MaterialStorageComponent, AppearanceChangeEvent>((ComponentEventRefHandler<MaterialStorageComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnAppearanceChange(EntityUid uid, MaterialStorageComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		bool flag = default(bool);
		if (args.Sprite == null || !_sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum)MaterialStorageVisualLayers.Inserting, ref num, false) || !((SharedAppearanceSystem)_appearance).TryGetData<bool>(uid, (Enum)MaterialStorageVisuals.Inserting, ref flag, args.Component))
		{
			return;
		}
		InsertingMaterialStorageComponent insertingMaterialStorageComponent = default(InsertingMaterialStorageComponent);
		if (flag && ((EntitySystem)this).TryComp<InsertingMaterialStorageComponent>(uid, ref insertingMaterialStorageComponent))
		{
			_sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, 0f);
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, true);
			if (insertingMaterialStorageComponent.MaterialColor.HasValue)
			{
				_sprite.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, insertingMaterialStorageComponent.MaterialColor.Value);
			}
		}
		else
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, false);
		}
	}

	public override bool TryInsertMaterialEntity(EntityUid user, EntityUid toInsert, EntityUid receiver, MaterialStorageComponent? storage = null, MaterialComponent? material = null, PhysicalCompositionComponent? composition = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!base.TryInsertMaterialEntity(user, toInsert, receiver, storage, material, composition))
		{
			return false;
		}
		((SharedTransformSystem)_transform).DetachEntity(toInsert, ((EntitySystem)this).Transform(toInsert));
		return true;
	}
}
