using System;
using Content.Shared._CIV14merka.GlobalMap;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._CIV14merka.GlobalMap;

public sealed class CivGlobalMapVerbSystem : EntitySystem
{
	[Dependency]
	private CivGlobalMapSystem _globalMap;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CivGlobalMapAlertComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<CivGlobalMapAlertComponent, GetVerbsEvent<ActivationVerb>>)OnGetVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CivTeamMemberComponent, GetVerbsEvent<ActivationVerb>>((EntityEventRefHandler<CivTeamMemberComponent, GetVerbsEvent<ActivationVerb>>)OnGetCommanderVerbs, (Type[])null, (Type[])null);
	}

	private void OnGetVerbs(Entity<CivGlobalMapAlertComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Target != ent.Owner) && !(args.User != ent.Owner))
		{
			args.Verbs.Add(new ActivationVerb
			{
				Text = base.Loc.GetString("civ-gmap-verb-open-map"),
				ClientExclusive = true,
				CloseMenu = true,
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					((EntitySystem)this).RaiseLocalEvent<OpenCivGlobalMapAlertEvent>(ent.Owner, new OpenCivGlobalMapAlertEvent(), false);
				}
			});
		}
	}

	private void OnGetCommanderVerbs(Entity<CivTeamMemberComponent> ent, ref GetVerbsEvent<ActivationVerb> args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Target != ent.Owner) && !(args.User != ent.Owner) && ent.Comp.IsCommander)
		{
			args.Verbs.Add(new ActivationVerb
			{
				Text = base.Loc.GetString("civ-gmap-verb-open-hq"),
				ClientExclusive = true,
				CloseMenu = true,
				Act = delegate
				{
					_globalMap.OpenCommanderWindow();
				}
			});
		}
	}
}
