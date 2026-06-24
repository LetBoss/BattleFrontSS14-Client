using System;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Storage;
using Content.Shared.Storage;
using Robust.Client.GameObjects;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Storage;

public sealed class CMStorageVisualizerSystem : VisualizerSystem<CMStorageVisualizerComponent>
{
	[Dependency]
	private SpriteSystem _sprite;

	protected override void OnAppearanceChange(EntityUid uid, CMStorageVisualizerComponent component, ref AppearanceChangeEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		if (args.Sprite == null || !((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<int>(uid, (Enum)StorageVisuals.StorageUsed, ref num, args.Component))
		{
			return;
		}
		StorageComponent storageComponent = default(StorageComponent);
		CMHolsterComponent cMHolsterComponent = default(CMHolsterComponent);
		if (((EntitySystem)this).TryComp<StorageComponent>(uid, ref storageComponent) && ((EntitySystem)this).TryComp<CMHolsterComponent>(uid, ref cMHolsterComponent) && ((BaseContainer)storageComponent.Container).ContainedEntities.Count == cMHolsterComponent.Contents.Count)
		{
			num = 0;
		}
		if (num == 0)
		{
			if (component.StorageOpen != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageOpen, false);
			}
			if (component.StorageClosed != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageClosed, false);
			}
			if (component.StorageEmpty != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageEmpty, true);
			}
			return;
		}
		if (component.StorageEmpty != null)
		{
			_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageEmpty, false);
		}
		bool flag = default(bool);
		if (!((SharedAppearanceSystem)base.AppearanceSystem).TryGetData<bool>(uid, (Enum)StorageVisuals.Open, ref flag, args.Component))
		{
			return;
		}
		if (flag)
		{
			if (component.StorageOpen != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageOpen, true);
			}
			if (component.StorageClosed != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageClosed, false);
			}
		}
		else
		{
			if (component.StorageOpen != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageOpen, false);
			}
			if (component.StorageClosed != null)
			{
				_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), component.StorageClosed, true);
			}
		}
	}
}
