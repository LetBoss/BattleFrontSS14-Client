using System;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.HealthExaminable;

public sealed class HealthExaminableSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examineSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HealthExaminableComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<HealthExaminableComponent, GetVerbsEvent<ExamineVerb>>)OnGetExamineVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetExamineVerbs(EntityUid uid, HealthExaminableComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		DamageableComponent damage = default(DamageableComponent);
		if (((EntitySystem)this).TryComp<DamageableComponent>(uid, ref damage))
		{
			bool detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					FormattedMessage message = CreateMarkup(uid, component, damage);
					_examineSystem.SendExamineTooltip(args.User, uid, message, getVerbs: false, centerAtCursor: false);
				},
				Text = base.Loc.GetString("health-examinable-verb-text"),
				Category = VerbCategory.Examine,
				Disabled = !detailsRange,
				Message = (detailsRange ? null : base.Loc.GetString("health-examinable-verb-disabled")),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/rejuvenate.svg.192dpi.png"))
			};
			args.Verbs.Add(verb);
		}
	}

	public FormattedMessage CreateMarkup(EntityUid uid, HealthExaminableComponent component, DamageableComponent damage)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		FormattedMessage msg = new FormattedMessage();
		bool first = true;
		foreach (ProtoId<DamageTypePrototype> type in component.ExaminableTypes)
		{
			if (!damage.Damage.DamageDict.TryGetValue(ProtoId<DamageTypePrototype>.op_Implicit(type), out var dmg) || dmg == FixedPoint2.Zero)
			{
				continue;
			}
			FixedPoint2 closest = FixedPoint2.Zero;
			string chosenLocStr = string.Empty;
			foreach (FixedPoint2 threshold in component.Thresholds)
			{
				string str = $"health-examinable-{component.LocPrefix}-{type}-{threshold}";
				string tempLocStr = base.Loc.GetString($"health-examinable-{component.LocPrefix}-{type}-{threshold}", (ValueTuple<string, object>)("target", Identity.Entity(uid, (IEntityManager)(object)base.EntityManager)));
				if (!(tempLocStr == str) && dmg > threshold && threshold > closest)
				{
					chosenLocStr = tempLocStr;
					closest = threshold;
				}
			}
			if (!(closest == FixedPoint2.Zero))
			{
				if (!first)
				{
					msg.PushNewline();
				}
				else
				{
					first = false;
				}
				msg.AddMarkupOrThrow(chosenLocStr);
			}
		}
		if (msg.IsEmpty && component.ExamineShowEmpty)
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("health-examinable-" + component.LocPrefix + "-none"));
		}
		((EntitySystem)this).RaiseLocalEvent<HealthBeingExaminedEvent>(uid, new HealthBeingExaminedEvent(msg), true);
		return msg;
	}
}
