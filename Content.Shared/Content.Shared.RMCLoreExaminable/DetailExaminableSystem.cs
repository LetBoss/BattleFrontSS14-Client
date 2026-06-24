using System;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared.RMCLoreExaminable;

public sealed class DetailExaminableSystem : EntitySystem
{
	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private NpcFactionSystem _npcFaction;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCLoreExaminableComponent, GetVerbsEvent<ExamineVerb>>((EntityEventRefHandler<RMCLoreExaminableComponent, GetVerbsEvent<ExamineVerb>>)OnGetExamineVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetExamineVerbs(Entity<RMCLoreExaminableComponent> ent, ref GetVerbsEvent<ExamineVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.User) && !(Identity.Name(args.Target, (IEntityManager)(object)base.EntityManager) != ((EntitySystem)this).MetaData(args.Target).EntityName) && (ent.Comp.Factions == null || ent.Comp.Factions.Count <= 0 || _npcFaction.IsMemberOfAny(Entity<NpcFactionMemberComponent>.op_Implicit(args.User), ent.Comp.Factions)))
		{
			bool detailsRange = _examine.IsInDetailsRange(args.User, Entity<RMCLoreExaminableComponent>.op_Implicit(ent));
			EntityUid user = args.User;
			ExamineVerb verb = new ExamineVerb
			{
				Act = delegate
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Expected O, but got Unknown
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					//IL_003d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0043: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					FormattedMessage val = new FormattedMessage();
					val.AddMarkupPermissive(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Content)));
					_examine.SendExamineTooltip(user, Entity<RMCLoreExaminableComponent>.op_Implicit(ent), val, getVerbs: false, centerAtCursor: false);
				},
				Text = base.Loc.GetString("lore-examinable-verb-text"),
				Category = VerbCategory.Examine,
				Disabled = !detailsRange,
				Message = (detailsRange ? null : base.Loc.GetString("lore-examinable-verb-disabled")),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/examine.svg.192dpi.png"))
			};
			args.Verbs.Add(verb);
		}
	}
}
