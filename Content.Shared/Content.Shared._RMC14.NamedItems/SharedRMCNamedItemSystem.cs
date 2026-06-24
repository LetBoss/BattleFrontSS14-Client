using System;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.NamedItems;

public abstract class SharedRMCNamedItemSystem : EntitySystem
{
	public static readonly int TypeCount = Enum.GetValues<RMCNamedItemType>().Length;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCNameItemOnVendComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCNameItemOnVendComponent, GetVerbsEvent<AlternativeVerb>>)OnNameItemGetVerbs, (Type[])null, (Type[])null);
	}

	private void OnNameItemGetVerbs(Entity<RMCNameItemOnVendComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		RMCUserNamedItemsComponent named = default(RMCUserNamedItemsComponent);
		if (args.CanAccess && args.CanInteract && ((EntitySystem)this).TryComp<RMCUserNamedItemsComponent>(args.User, ref named))
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = "Reapply custom name",
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryNameItem(Entity<RMCUserNamedItemsComponent>.op_Implicit((user, named)), Entity<RMCNameItemOnVendComponent>.op_Implicit(ent), ent.Comp.Item);
				},
				Priority = -100
			});
		}
	}

	protected virtual bool TryNameItem(Entity<RMCUserNamedItemsComponent> user, EntityUid item, RMCNamedItemType type)
	{
		return false;
	}
}
