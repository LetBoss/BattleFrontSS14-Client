using System;
using Content.Shared._RMC14.Emplacements;
using Content.Shared._RMC14.Weapons.Ranged.Overheat;
using Content.Shared.Foldable;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Emplacements;

public sealed class WeaponMountSystem : SharedWeaponMountSystem
{
	[Dependency]
	private SpriteSystem _sprite;

	private const string FoldedLayer = "foldedLayer";

	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<WeaponMountComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<WeaponMountComponent, AppearanceChangeEvent>((EntityEventRefHandler<WeaponMountComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnHandleState(Entity<WeaponMountComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(ent);
	}

	private void OnAppearanceChange(Entity<WeaponMountComponent> ent, ref AppearanceChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateVisuals(ent);
	}

	private void UpdateVisuals(Entity<WeaponMountComponent> mount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent val = default(SpriteComponent);
		if (!((EntitySystem)this).TryComp<SpriteComponent>(Entity<WeaponMountComponent>.op_Implicit(mount), ref val))
		{
			return;
		}
		FoldableComponent foldableComponent = default(FoldableComponent);
		((EntitySystem)this).TryComp<FoldableComponent>(Entity<WeaponMountComponent>.op_Implicit(mount), ref foldableComponent);
		(Entity<WeaponMountComponent>, SpriteComponent) tuple = (mount, val);
		SpriteSystem sprite = _sprite;
		(Entity<WeaponMountComponent>, SpriteComponent) tuple2 = tuple;
		int num = default(int);
		if (sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.Mounted, ref num, false))
		{
			bool flag = mount.Comp.MountedEntity.HasValue;
			if (foldableComponent != null)
			{
				flag = flag && !foldableComponent.IsFolded && !mount.Comp.Broken;
			}
			SpriteSystem sprite2 = _sprite;
			tuple2 = tuple;
			int num2 = default(int);
			OverheatComponent overheatComponent = default(OverheatComponent);
			if (sprite2.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.Overheated, ref num2, false) && ((EntitySystem)this).TryComp<OverheatComponent>(mount.Comp.MountedEntity, ref overheatComponent))
			{
				SpriteSystem sprite3 = _sprite;
				tuple2 = tuple;
				sprite3.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), num2, flag);
				float num3 = Math.Clamp(overheatComponent.Heat / (float)overheatComponent.MaxHeat, 0f, 1f);
				SpriteSystem sprite4 = _sprite;
				tuple2 = tuple;
				Entity<SpriteComponent> val2 = Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2));
				int num4 = num2;
				Color color = val.Color;
				sprite4.LayerSetColor(val2, num4, ((Color)(ref color)).WithAlpha(num3));
			}
			if (foldableComponent != null && foldableComponent.IsFolded)
			{
				SpriteSystem sprite5 = _sprite;
				tuple2 = tuple;
				sprite5.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.MountedAmmo, false);
			}
			if (foldableComponent == null || !foldableComponent.IsFolded)
			{
				Entity<WeaponMountComponent> mount2 = mount;
				tuple2 = tuple;
				UpdateAmmoVisual(mount2, Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), WeaponMountComponentVisualLayers.MountedAmmo);
			}
			SpriteSystem sprite6 = _sprite;
			tuple2 = tuple;
			sprite6.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), num, flag);
			SpriteSystem sprite7 = _sprite;
			tuple2 = tuple;
			int num5 = default(int);
			if (sprite7.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.Broken, ref num5, false))
			{
				SpriteSystem sprite8 = _sprite;
				tuple2 = tuple;
				sprite8.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), num5, mount.Comp.Broken);
			}
		}
		SpriteSystem sprite9 = _sprite;
		tuple2 = tuple;
		int num6 = default(int);
		if (sprite9.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.Folded, ref num6, false) && foldableComponent != null)
		{
			if (foldableComponent.IsFolded)
			{
				Entity<WeaponMountComponent> mount3 = mount;
				tuple2 = tuple;
				UpdateAmmoVisual(mount3, Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), WeaponMountComponentVisualLayers.FoldedAmmo);
			}
			else
			{
				SpriteSystem sprite10 = _sprite;
				tuple2 = tuple;
				sprite10.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.FoldedAmmo, false);
			}
			bool flag2 = foldableComponent.IsFolded && !mount.Comp.Broken;
			SpriteSystem sprite11 = _sprite;
			tuple2 = tuple;
			sprite11.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), num6, flag2 && mount.Comp.MountedEntity.HasValue);
			SpriteSystem sprite12 = _sprite;
			tuple2 = tuple;
			int num7 = default(int);
			if (sprite12.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), "foldedLayer", ref num7, false))
			{
				SpriteSystem sprite13 = _sprite;
				tuple2 = tuple;
				sprite13.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), num7, flag2 && !mount.Comp.MountedEntity.HasValue);
			}
			SpriteSystem sprite14 = _sprite;
			tuple2 = tuple;
			int num8 = default(int);
			if (sprite14.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), (Enum)WeaponMountComponentVisualLayers.Broken, ref num8, false))
			{
				SpriteSystem sprite15 = _sprite;
				tuple2 = tuple;
				sprite15.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), num8, mount.Comp.Broken);
			}
		}
		if (!mount.Comp.MountedEntity.HasValue || (foldableComponent != null && foldableComponent.IsFolded))
		{
			SpriteSystem sprite16 = _sprite;
			tuple2 = tuple;
			sprite16.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), 4);
		}
		else
		{
			SpriteSystem sprite17 = _sprite;
			tuple2 = tuple;
			sprite17.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((Entity<WeaponMountComponent>.op_Implicit(tuple2.Item1), tuple2.Item2)), 6);
		}
	}

	private void UpdateAmmoVisual(Entity<WeaponMountComponent> mount, Entity<SpriteComponent?> sprite, Enum mapKey)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		int num = default(int);
		if (!_sprite.LayerMapTryGet(sprite, mapKey, ref num, false))
		{
			return;
		}
		if (mount.Comp.MountedEntity.HasValue)
		{
			GetAmmoCountEvent getAmmoCountEvent = default(GetAmmoCountEvent);
			((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(mount.Comp.MountedEntity.Value, ref getAmmoCountEvent, false);
			if (getAmmoCountEvent.Count > 0)
			{
				flag = true;
			}
		}
		_sprite.LayerSetVisible(sprite, num, flag);
	}
}
