using System;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Weapons.Ranged;

public sealed class CMAmmoBoxSystem : EntitySystem
{
	[Dependency]
	private AppearanceSystem _appearance;

	[Dependency]
	private SpriteSystem _sprite;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CMAmmoBoxComponent, ComponentStartup>((EntityEventRefHandler<CMAmmoBoxComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CMAmmoBoxComponent, AppearanceChangeEvent>((EntityEventRefHandler<CMAmmoBoxComponent, AppearanceChangeEvent>)OnAppearanceChange, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<CMAmmoBoxComponent> box, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		AppearanceComponent item2 = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<SpriteComponent>(Entity<CMAmmoBoxComponent>.op_Implicit(box), ref item) && ((EntitySystem)this).TryComp<AppearanceComponent>(Entity<CMAmmoBoxComponent>.op_Implicit(box), ref item2))
		{
			UpdateAppearance(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit((Entity<CMAmmoBoxComponent>.op_Implicit(box), Entity<CMAmmoBoxComponent>.op_Implicit(box), item, item2)));
		}
	}

	private void OnAppearanceChange(Entity<CMAmmoBoxComponent> box, ref AppearanceChangeEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (args.Sprite != null)
		{
			UpdateAppearance(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit((Entity<CMAmmoBoxComponent>.op_Implicit(box), Entity<CMAmmoBoxComponent>.op_Implicit(box), args.Sprite, args.Component)));
		}
	}

	private void UpdateAppearance(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent> box)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		((SharedAppearanceSystem)_appearance).TryGetData<int>(Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit(box), (Enum)AmmoVisuals.AmmoCount, ref num, Entity<CMAmmoBoxComponent, SpriteComponent, AppearanceComponent>.op_Implicit(box));
		_sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((box.Owner, box.Comp2)), box.Comp1.AmmoLayer, num > 0);
	}
}
