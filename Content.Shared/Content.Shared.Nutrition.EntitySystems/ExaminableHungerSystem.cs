using System;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class ExaminableHungerSystem : EntitySystem
{
	[Dependency]
	private HungerSystem _hunger;

	private EntityQuery<HungerComponent> _hungerQuery;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_hungerQuery = ((EntitySystem)this).GetEntityQuery<HungerComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ExaminableHungerComponent, ExaminedEvent>((EntityEventRefHandler<ExaminableHungerComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
	}

	private void OnExamine(Entity<ExaminableHungerComponent> entity, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		EntityUid identity = Identity.Entity(Entity<ExaminableHungerComponent>.op_Implicit(entity), (IEntityManager)(object)base.EntityManager);
		HungerComponent hungerComp = default(HungerComponent);
		if (!_hungerQuery.TryComp(Entity<ExaminableHungerComponent>.op_Implicit(entity), ref hungerComp) || !entity.Comp.Descriptions.TryGetValue(_hunger.GetHungerThreshold(hungerComp), out var locId))
		{
			locId = entity.Comp.NoHungerDescription;
		}
		string msg = base.Loc.GetString(LocId.op_Implicit(locId), (ValueTuple<string, object>)("entity", identity));
		args.PushMarkup(msg);
	}
}
