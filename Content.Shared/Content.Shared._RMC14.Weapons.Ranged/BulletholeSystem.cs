using System;
using Content.Shared._RMC14.Weapons.Ranged.Ammo;
using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class BulletholeSystem : EntitySystem
{
	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private IRobustRandom _random;

	private const int MaxBulletholeState = 10;

	private const int MaxBulletholeCount = 24;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<BulletholeComponent, DamageChangedEvent>((EntityEventRefHandler<BulletholeComponent, DamageChangedEvent>)OnVisualsDamageChangedEvent, (Type[])null, (Type[])null);
	}

	private void OnVisualsDamageChangedEvent(Entity<BulletholeComponent> ent, ref DamageChangedEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		BulletholeGeneratorComponent bulletholeGeneratorComponent = default(BulletholeGeneratorComponent);
		if (!((EntitySystem)this).TryComp<BulletholeGeneratorComponent>(args.Tool, ref bulletholeGeneratorComponent))
		{
			return;
		}
		ent.Comp.BulletholeCount++;
		AppearanceComponent app = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<BulletholeComponent>.op_Implicit(ent), ref app))
		{
			if (ent.Comp.BulletholeState < 1 || ent.Comp.BulletholeState > 10)
			{
				ent.Comp.BulletholeState = _random.Next(1, 11);
			}
			int displayState = ent.Comp.BulletholeState;
			int displayCount = ((ent.Comp.BulletholeCount >= 24) ? 24 : ent.Comp.BulletholeCount);
			string stateString = $"bhole_{displayState}_{displayCount}";
			_appearance.SetData(Entity<BulletholeComponent>.op_Implicit(ent), (Enum)BulletholeVisuals.State, (object)stateString, app);
		}
	}
}
