using System;
using Content.Shared.Access.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.Access.Systems;

public sealed class IdExaminableSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examineSystem;

	[Dependency]
	private InventorySystem _inventorySystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<IdExaminableComponent, GetVerbsEvent<ExamineVerb>>((ComponentEventHandler<IdExaminableComponent, GetVerbsEvent<ExamineVerb>>)OnGetExamineVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetExamineVerbs(EntityUid uid, IdExaminableComponent component, GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		bool detailsRange = _examineSystem.IsInDetailsRange(args.User, uid);
		string info = GetMessage(uid);
		ExamineVerb verb = new ExamineVerb
		{
			Act = delegate
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				FormattedMessage message = FormattedMessage.FromMarkupOrThrow(info);
				_examineSystem.SendExamineTooltip(args.User, uid, message, getVerbs: false, centerAtCursor: false);
			},
			Text = base.Loc.GetString("id-examinable-component-verb-text"),
			Category = VerbCategory.Examine,
			Disabled = !detailsRange,
			Message = (detailsRange ? null : base.Loc.GetString("id-examinable-component-verb-disabled")),
			Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/character.svg.192dpi.png"))
		};
		args.Verbs.Add(verb);
	}

	public string GetMessage(EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetInfo(uid) ?? base.Loc.GetString("id-examinable-component-verb-no-id");
	}

	public string? GetInfo(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (_inventorySystem.TryGetSlotEntity(uid, "id", out var idUid))
		{
			PdaComponent pda = default(PdaComponent);
			IdCardComponent id = default(IdCardComponent);
			if (((EntitySystem)this).TryComp<PdaComponent>(idUid, ref pda) && ((EntitySystem)this).TryComp<IdCardComponent>(pda.ContainedId, ref id))
			{
				return GetNameAndJob(id);
			}
			if (((EntitySystem)this).TryComp<IdCardComponent>(idUid, ref id))
			{
				return GetNameAndJob(id);
			}
		}
		return null;
	}

	private string GetNameAndJob(IdCardComponent id)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		string jobSuffix = (string.IsNullOrWhiteSpace(id.LocalizedJobTitle) ? string.Empty : (" (" + id.LocalizedJobTitle + ")"));
		if (!string.IsNullOrWhiteSpace(id.FullName))
		{
			return base.Loc.GetString(LocId.op_Implicit(id.FullNameLocId), (ValueTuple<string, object>)("fullName", id.FullName), (ValueTuple<string, object>)("jobSuffix", jobSuffix));
		}
		return base.Loc.GetString(LocId.op_Implicit(id.NameLocId), (ValueTuple<string, object>)("jobSuffix", jobSuffix));
	}
}
