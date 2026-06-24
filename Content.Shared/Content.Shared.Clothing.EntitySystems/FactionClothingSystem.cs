using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class FactionClothingSystem : EntitySystem
{
	[Dependency]
	private NpcFactionSystem _faction;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FactionClothingComponent, GotEquippedEvent>((EntityEventRefHandler<FactionClothingComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FactionClothingComponent, GotUnequippedEvent>((EntityEventRefHandler<FactionClothingComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnEquipped(Entity<FactionClothingComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		NpcFactionMemberComponent factionComp = default(NpcFactionMemberComponent);
		((EntitySystem)this).TryComp<NpcFactionMemberComponent>(args.Equipee, ref factionComp);
		(EntityUid, NpcFactionMemberComponent) faction = (args.Equipee, factionComp);
		ent.Comp.AlreadyMember = _faction.IsMember(Entity<NpcFactionMemberComponent>.op_Implicit(faction), ProtoId<NpcFactionPrototype>.op_Implicit(ent.Comp.Faction));
		_faction.AddFaction(Entity<NpcFactionMemberComponent>.op_Implicit(faction), ProtoId<NpcFactionPrototype>.op_Implicit(ent.Comp.Faction));
	}

	private void OnUnequipped(Entity<FactionClothingComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.AlreadyMember)
		{
			ent.Comp.AlreadyMember = false;
		}
		else
		{
			_faction.RemoveFaction(Entity<NpcFactionMemberComponent>.op_Implicit(args.Equipee), ProtoId<NpcFactionPrototype>.op_Implicit(ent.Comp.Faction));
		}
	}
}
