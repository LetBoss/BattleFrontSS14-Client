using System;
using System.Collections.Immutable;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.HealthExaminable;

public sealed class RMCHealthExaminableSystem : EntitySystem
{
	private readonly ImmutableArray<FixedPoint2> _thresholds = ImmutableArray.Create((FixedPoint2)25, (FixedPoint2)50, (FixedPoint2)75);

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCHealthExaminableComponent, ExaminedEvent>((EntityEventRefHandler<RMCHealthExaminableComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnExamined(Entity<RMCHealthExaminableComponent> ent, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(Entity<RMCHealthExaminableComponent>.op_Implicit(ent), ref damageable))
		{
			return;
		}
		using (args.PushGroup("RMCHealthExaminableSystem", -1))
		{
			string msg = default(string);
			foreach (ProtoId<DamageGroupPrototype> group in ent.Comp.Groups)
			{
				if (!damageable.DamagePerGroup.TryGetValue(ProtoId<DamageGroupPrototype>.op_Implicit(group), out var groupDamage))
				{
					continue;
				}
				for (int i = _thresholds.Length - 1; i >= 0; i--)
				{
					FixedPoint2 threshold = _thresholds[i];
					if (!(groupDamage < threshold))
					{
						string id = $"rmc-health-examinable-{group}-{threshold.Int()}";
						if (base.Loc.TryGetString(id, ref msg, (ValueTuple<string, object>)("target", Identity.Entity(Entity<RMCHealthExaminableComponent>.op_Implicit(ent), (IEntityManager)(object)base.EntityManager, args.Examiner))))
						{
							args.PushMarkup(msg);
							break;
						}
					}
				}
			}
		}
	}
}
