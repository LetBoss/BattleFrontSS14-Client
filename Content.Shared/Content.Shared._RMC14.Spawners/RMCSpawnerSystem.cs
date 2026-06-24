using System;
using Content.Shared._RMC14.Evacuation;
using Content.Shared.Coordinates;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Spawners;

public sealed class RMCSpawnerSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedEvacuationSystem _evacuation;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SpawnOnInteractComponent, InteractHandEvent>((EntityEventRefHandler<SpawnOnInteractComponent, InteractHandEvent>)OnSpawnOnInteractHand, (Type[])null, (Type[])null);
	}

	private void OnSpawnOnInteractHand(Entity<SpawnOnInteractComponent> ent, ref InteractHandEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		EntityUid user = args.User;
		if (((EntitySystem)this).TerminatingOrDeleted(Entity<SpawnOnInteractComponent>.op_Implicit(ent), (MetaDataComponent)null) || base.EntityManager.IsQueuedForDeletion(Entity<SpawnOnInteractComponent>.op_Implicit(ent)) || _entityWhitelist.IsBlacklistPass(ent.Comp.Blacklist, user))
		{
			return;
		}
		if (ent.Comp.RequireEvacuation && !_evacuation.IsEvacuationInProgress())
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-sentry-not-emergency", (ValueTuple<string, object>)("deployer", ent)), Entity<SpawnOnInteractComponent>.op_Implicit(ent), user);
			return;
		}
		EntityUid spawned = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(ent.Comp.Spawn), ent.Owner.ToCoordinates(), (ComponentRegistry)null);
		LocId? popup = ent.Comp.Popup;
		if (popup.HasValue)
		{
			LocId popup2 = popup.GetValueOrDefault();
			_popup.PopupEntity(base.Loc.GetString(LocId.op_Implicit(popup2), (ValueTuple<string, object>)("spawned", spawned)), Entity<SpawnOnInteractComponent>.op_Implicit(ent), user);
		}
		_audio.PlayPvs(ent.Comp.Sound, spawned, (AudioParams?)null);
		((EntitySystem)this).QueueDel((EntityUid?)Entity<SpawnOnInteractComponent>.op_Implicit(ent));
	}
}
