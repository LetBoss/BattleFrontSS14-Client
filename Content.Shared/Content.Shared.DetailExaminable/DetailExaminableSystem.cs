using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Shared.DetailExaminable;

public sealed class DetailExaminableSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examine;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<DetailExaminableComponent, GetVerbsEvent<ExamineVerb>>)OnGetExamineVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetExamineVerbs(Entity<DetailExaminableComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.User) && !(Identity.Name(args.Target, (IEntityManager)(object)base.EntityManager) != ((EntitySystem)this).MetaData(args.Target).EntityName))
		{
			bool detailsRange = _examine.IsInDetailsRange(args.User, Entity<DetailExaminableComponent>.op_Implicit(ent));
			EntityUid user = args.User;
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Expected O, but got Unknown
					//IL_0028: Unknown result type (might be due to invalid IL or missing references)
					//IL_002e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					FormattedMessage val = new FormattedMessage();
					val.AddMarkupPermissive(ent.Comp.Content);
					_examine.SendExamineTooltip(user, Entity<DetailExaminableComponent>.op_Implicit(ent), val, getVerbs: false, centerAtCursor: false);
				},
				Text = base.Loc.GetString("detail-examinable-verb-text"),
				Category = VerbCategory.Examine,
				Disabled = !detailsRange,
				Message = (detailsRange ? null : base.Loc.GetString("detail-examinable-verb-disabled")),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"))
			};
			args.Verbs.Add(verb);
		}
	}
}
