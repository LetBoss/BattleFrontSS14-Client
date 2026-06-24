using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Emplacements;
using Content.Shared.Wieldable.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.CombatMode;

public sealed class RMCCombatModeSystem : EntitySystem
{
	public Rsi? GetCrosshair(Entity<WieldedCrosshairComponent?, WieldableComponent?> crosshair)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<WieldedCrosshairComponent>(Entity<WieldedCrosshairComponent, WieldableComponent>.op_Implicit(crosshair), ref crosshair.Comp1, false))
		{
			return null;
		}
		if (!((EntitySystem)this).Resolve<WieldableComponent>(Entity<WieldedCrosshairComponent, WieldableComponent>.op_Implicit(crosshair), ref crosshair.Comp2, false))
		{
			MountableWeaponComponent mountable = default(MountableWeaponComponent);
			if (((EntitySystem)this).TryComp<MountableWeaponComponent>(crosshair.Owner, ref mountable) && mountable.MountedTo.HasValue)
			{
				return crosshair.Comp1?.Rsi;
			}
			return null;
		}
		WieldableComponent comp = crosshair.Comp2;
		if (comp == null || !comp.Wielded)
		{
			return null;
		}
		EntityUid heldUid = crosshair.Owner;
		AttachableHolderComponent holder = default(AttachableHolderComponent);
		if (((EntitySystem)this).TryComp<AttachableHolderComponent>(heldUid, ref holder))
		{
			EntityUid? supercedingAttachable = holder.SupercedingAttachable;
			if (supercedingAttachable.HasValue)
			{
				EntityUid active = supercedingAttachable.GetValueOrDefault();
				WieldedCrosshairComponent ubXhair = default(WieldedCrosshairComponent);
				if (((EntitySystem)this).TryComp<WieldedCrosshairComponent>(active, ref ubXhair))
				{
					Rsi ubSpec = ubXhair.Rsi;
					if (ubSpec != null)
					{
						return ubSpec;
					}
				}
			}
		}
		return crosshair.Comp1?.Rsi;
	}
}
