using System;
using System.Collections.Generic;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Damage.Systems;

public sealed class DamageExamineSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private IPrototypeManager _prototype;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DamageExaminableComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<DamageExaminableComponent, GetVerbsEvent<ExamineVerb>>)OnGetExamineVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetExamineVerbs(EntityUid uid, DamageExaminableComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanInteract && args.CanAccess)
		{
			DamageExamineEvent ev = new DamageExamineEvent(new FormattedMessage(), args.User);
			((EntitySystem)this).RaiseLocalEvent<DamageExamineEvent>(uid, ref ev, false);
			if (!ev.Message.IsEmpty)
			{
				_examine.AddDetailedExamineVerb(args, (Component)(object)component, ev.Message, base.Loc.GetString("damage-examinable-verb-text"), "/Textures/Interface/VerbIcons/smite.svg.192dpi.png", base.Loc.GetString("damage-examinable-verb-message"));
			}
		}
	}

	public void AddDamageExamine(FormattedMessage message, DamageSpecifier damageSpecifier, string? type = null)
	{
		FormattedMessage markup = GetDamageExamine(damageSpecifier, type);
		if (!message.IsEmpty)
		{
			message.PushNewline();
		}
		message.AddMessage(markup);
	}

	private FormattedMessage GetDamageExamine(DamageSpecifier damageSpecifier, string? type = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FormattedMessage msg = new FormattedMessage();
		if (string.IsNullOrEmpty(type))
		{
			msg.AddMarkupOrThrow(base.Loc.GetString("damage-examine"));
		}
		else
		{
			if (damageSpecifier.GetTotal() == FixedPoint2.Zero && !damageSpecifier.AnyPositive())
			{
				msg.AddMarkupOrThrow(base.Loc.GetString("damage-none"));
				return msg;
			}
			msg.AddMarkupOrThrow(base.Loc.GetString("damage-examine-type", (ValueTuple<string, object>)("type", type)));
		}
		foreach (KeyValuePair<string, FixedPoint2> damage in damageSpecifier.DamageDict)
		{
			if (damage.Value != FixedPoint2.Zero)
			{
				msg.PushNewline();
				msg.AddMarkupOrThrow(base.Loc.GetString("damage-value", (ValueTuple<string, object>)("type", _prototype.Index<DamageTypePrototype>(damage.Key).LocalizedName), (ValueTuple<string, object>)("amount", damage.Value)));
			}
		}
		return msg;
	}
}
