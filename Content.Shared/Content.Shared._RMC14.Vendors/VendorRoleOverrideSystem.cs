using System;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Squads;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Vendors;

public sealed class VendorRoleOverrideSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCVendorRoleOverrideComponent, GetMarineIconEvent>((EntityEventRefHandler<RMCVendorRoleOverrideComponent, GetMarineIconEvent>)OnGetMarineIcon, (Type[])null, new Type[2]
		{
			typeof(SharedMarineSystem),
			typeof(SquadSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<RMCVendorRoleOverrideComponent, GetMarineSquadNameEvent>((EntityEventRefHandler<RMCVendorRoleOverrideComponent, GetMarineSquadNameEvent>)OnGetSquadTitle, (Type[])null, new Type[1] { typeof(SquadSystem) });
	}

	private void OnGetMarineIcon(Entity<RMCVendorRoleOverrideComponent> ent, ref GetMarineIconEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<SquadLeaderComponent>(Entity<RMCVendorRoleOverrideComponent>.op_Implicit(ent)) && ent.Comp.GiveIcon != null)
		{
			args.Icon = (SpriteSpecifier?)(object)ent.Comp.GiveIcon;
		}
	}

	private void OnGetSquadTitle(Entity<RMCVendorRoleOverrideComponent> ent, ref GetMarineSquadNameEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.GiveSquadRoleName.HasValue)
		{
			if (ent.Comp.IsAppendSquadRoleName)
			{
				string roleName = args.RoleName;
				ILocalizationManager loc = base.Loc;
				LocId? giveSquadRoleName = ent.Comp.GiveSquadRoleName;
				args.RoleName = roleName + " " + loc.GetString(giveSquadRoleName.HasValue ? LocId.op_Implicit(giveSquadRoleName.GetValueOrDefault()) : null);
			}
			else
			{
				ILocalizationManager loc2 = base.Loc;
				LocId? giveSquadRoleName = ent.Comp.GiveSquadRoleName;
				args.RoleName = loc2.GetString(giveSquadRoleName.HasValue ? LocId.op_Implicit(giveSquadRoleName.GetValueOrDefault()) : null);
			}
		}
	}
}
