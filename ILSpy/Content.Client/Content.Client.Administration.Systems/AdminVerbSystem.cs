using System;
using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Mind.Components;
using Content.Shared.Verbs;
using Robust.Client.Console;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.Administration.Systems;

internal sealed class AdminVerbSystem : EntitySystem
{
	[Dependency]
	private IClientConGroupController _clientConGroupController;

	[Dependency]
	private IClientConsoleHost _clientConsoleHost;

	[Dependency]
	private ISharedAdminManager _admin;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GetVerbsEvent<Verb>>((EntityEventHandler<GetVerbsEvent<Verb>>)AddAdminVerbs, (Type[])null, (Type[])null);
	}

	private void AddAdminVerbs(GetVerbsEvent<Verb> args)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (((IClientConGroupImplementation)_clientConGroupController).CanViewVar())
		{
			VvVerb item = new VvVerb
			{
				Text = base.Loc.GetString("view-variables"),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/vv.svg.192dpi.png")),
				Act = delegate
				{
					//IL_002e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					((IConsoleHost)_clientConsoleHost).ExecuteCommand($"vv {((EntitySystem)this).GetNetEntity(args.Target, (MetaDataComponent)null)}");
				},
				ClientExclusive = true
			};
			args.Verbs.Add(item);
		}
		if (_admin.IsAdmin(args.User))
		{
			if (_admin.HasAdminFlag(args.User, AdminFlags.Admin))
			{
				args.ExtraCategories.Add(VerbCategory.Admin);
			}
			if (_admin.HasAdminFlag(args.User, AdminFlags.Fun) && ((EntitySystem)this).HasComp<MindContainerComponent>(args.Target))
			{
				args.ExtraCategories.Add(VerbCategory.Antag);
			}
			if (_admin.HasAdminFlag(args.User, AdminFlags.Debug))
			{
				args.ExtraCategories.Add(VerbCategory.Debug);
			}
			if (_admin.HasAdminFlag(args.User, AdminFlags.Fun))
			{
				args.ExtraCategories.Add(VerbCategory.Smite);
			}
			if (_admin.HasAdminFlag(args.User, AdminFlags.Admin))
			{
				args.ExtraCategories.Add(VerbCategory.Tricks);
			}
		}
	}
}
