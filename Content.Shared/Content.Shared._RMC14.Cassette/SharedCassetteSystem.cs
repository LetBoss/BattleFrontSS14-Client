using System;
using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Cassette;

public abstract class SharedCassetteSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private INetConfigurationManager _netConfig;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PlayerDetachedEvent>((EntityEventHandler<PlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, GetItemActionsEvent>((EntityEventRefHandler<CassettePlayerComponent, GetItemActionsEvent>)OnPlayerGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, CassettePlayPauseActionEvent>((EntityEventRefHandler<CassettePlayerComponent, CassettePlayPauseActionEvent>)OnPlayerPlayPause, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, CassetteNextActionEvent>((EntityEventRefHandler<CassettePlayerComponent, CassetteNextActionEvent>)OnPlayerNext, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, CassetteRestartActionEvent>((EntityEventRefHandler<CassettePlayerComponent, CassetteRestartActionEvent>)OnPlayerRestart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, InteractUsingEvent>((EntityEventRefHandler<CassettePlayerComponent, InteractUsingEvent>)OnPlayerInteractUsing, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, RMCStorageEjectHandItemEvent>((EntityEventRefHandler<CassettePlayerComponent, RMCStorageEjectHandItemEvent>)OnPlayerEjectHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, GetEquipmentVisualsEvent>((EntityEventRefHandler<CassettePlayerComponent, GetEquipmentVisualsEvent>)OnPlayerGetEquipmentVisuals, (Type[])null, new Type[1] { typeof(ClothingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, GotUnequippedEvent>((EntityEventRefHandler<CassettePlayerComponent, GotUnequippedEvent>)OnPlayerUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, ExaminedEvent>((EntityEventRefHandler<CassettePlayerComponent, ExaminedEvent>)OnPlayerExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<CassettePlayerComponent, AfterAutoHandleStateEvent>)OnPlayerState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<CassettePlayerComponent, EntRemovedFromContainerMessage>)OnPlayerRemovedFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassettePlayerComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<CassettePlayerComponent, GetVerbsEvent<AlternativeVerb>>)OnPlayerGetVerbsAlternative, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassetteTapeComponent, ExaminedEvent>((EntityEventRefHandler<CassetteTapeComponent, ExaminedEvent>)OnTapeExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassetteTapeComponent, UseInHandEvent>((EntityEventRefHandler<CassetteTapeComponent, UseInHandEvent>)OnPlayerUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CassetteTapeComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<CassetteTapeComponent, GetVerbsEvent<AlternativeVerb>>)OnTapeGetVerbsAlternative, (Type[])null, (Type[])null);
	}

	private void OnPlayerDetached(PlayerDetachedEvent ev)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(ev.Entity));
		ContainerSlot slot;
		CassettePlayerComponent player = default(CassettePlayerComponent);
		while (slots.MoveNext(out slot))
		{
			if (((EntitySystem)this).TryComp<CassettePlayerComponent>(slot.ContainedEntity, ref player))
			{
				StopAllAudio(Entity<CassettePlayerComponent>.op_Implicit((slot.ContainedEntity.Value, player)));
			}
		}
	}

	private void OnPlayerGetItemActions(Entity<CassettePlayerComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || args.InHands)
		{
			return;
		}
		SlotFlags? slotFlags = args.SlotFlags;
		if (slotFlags.HasValue)
		{
			SlotFlags slots = slotFlags.GetValueOrDefault();
			if (ent.Comp.Slots.HasFlag(slots))
			{
				args.AddAction(ref ent.Comp.PlayPauseAction, EntProtoId.op_Implicit(ent.Comp.PlayPauseActionId));
				args.AddAction(ref ent.Comp.NextAction, EntProtoId.op_Implicit(ent.Comp.NextActionId));
				args.AddAction(ref ent.Comp.RestartAction, EntProtoId.op_Implicit(ent.Comp.RestartActionId));
				((EntitySystem)this).Dirty<CassettePlayerComponent>(ent, (MetaDataComponent)null);
			}
		}
	}

	private void OnPlayerPlayPause(Entity<CassettePlayerComponent> ent, ref CassettePlayPauseActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected I4, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		int total = GetTotalSongs(ent);
		Entity<CassetteTapeComponent>? tape = GetTape(ent);
		AudioState state = ent.Comp.State;
		switch ((int)state)
		{
		case 0:
		{
			PlaySong(ent, args.Performer);
			string msg2 = base.Loc.GetString("rmc-cassette-playing", new(string, object)[3]
			{
				("player", ent),
				("current", GetCurrentSongCount(ent)),
				("total", total)
			});
			_popup.PopupClient(msg2, Entity<CassettePlayerComponent>.op_Implicit(ent), args.Performer);
			break;
		}
		case 1:
			if (_net.IsServer)
			{
				_audio.SetState(ent.Comp.AudioStream, (AudioState)2, false, (AudioComponent)null);
			}
			else if (tape.HasValue)
			{
				CassetteTapeComponent comp = tape.GetValueOrDefault().Comp;
				if (comp != null && comp.Custom)
				{
					_audio.SetState(ent.Comp.CustomAudioStream, (AudioState)2, false, (AudioComponent)null);
				}
			}
			_popup.PopupClient(base.Loc.GetString("rmc-cassette-pause", (ValueTuple<string, object>)("player", ent)), Entity<CassettePlayerComponent>.op_Implicit(ent), args.Performer);
			ent.Comp.State = (AudioState)2;
			break;
		case 2:
		{
			if (_net.IsServer)
			{
				_audio.SetState(ent.Comp.AudioStream, (AudioState)1, false, (AudioComponent)null);
			}
			else if (tape.HasValue)
			{
				CassetteTapeComponent comp = tape.GetValueOrDefault().Comp;
				if (comp != null && comp.Custom)
				{
					_audio.SetState(ent.Comp.CustomAudioStream, (AudioState)1, false, (AudioComponent)null);
				}
			}
			string msg = base.Loc.GetString("rmc-cassette-resume", new(string, object)[3]
			{
				("player", ent),
				("current", GetCurrentSongCount(ent)),
				("total", total)
			});
			_popup.PopupClient(msg, Entity<CassettePlayerComponent>.op_Implicit(ent), args.Performer);
			ent.Comp.State = (AudioState)1;
			break;
		}
		}
		_audio.PlayLocal(ent.Comp.PlayPauseSound, Entity<CassettePlayerComponent>.op_Implicit(ent), (EntityUid?)args.Performer, (AudioParams?)null);
		((EntitySystem)this).Dirty<CassettePlayerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPlayerNext(Entity<CassettePlayerComponent> ent, ref CassetteNextActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		PlaySong(ent, args.Performer, ent.Comp.Tape + 1);
		string msg = base.Loc.GetString("rmc-cassette-change", (ValueTuple<string, object>)("current", GetCurrentSongCount(ent)), (ValueTuple<string, object>)("total", GetTotalSongs(ent)));
		_popup.PopupClient(msg, Entity<CassettePlayerComponent>.op_Implicit(ent), args.Performer);
	}

	private void OnPlayerRestart(Entity<CassettePlayerComponent> ent, ref CassetteRestartActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		PlaySong(ent, args.Performer);
		string msg = base.Loc.GetString("rmc-cassette-restart", (ValueTuple<string, object>)("current", GetCurrentSongCount(ent)), (ValueTuple<string, object>)("total", GetTotalSongs(ent)));
		_popup.PopupClient(msg, Entity<CassettePlayerComponent>.op_Implicit(ent), args.Performer);
	}

	private void PlaySong(Entity<CassettePlayerComponent> player, EntityUid actor, int? tape = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		EntityUid? tapeId = default(EntityUid?);
		CassetteTapeComponent tapeComp = default(CassetteTapeComponent);
		if (!_container.TryGetContainer(Entity<CassettePlayerComponent>.op_Implicit(player), player.Comp.ContainerId, ref container, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref tapeId) || !((EntitySystem)this).TryComp<CassetteTapeComponent>(tapeId, ref tapeComp))
		{
			return;
		}
		StopAllAudio(player);
		int valueOrDefault = tape.GetValueOrDefault();
		if (!tape.HasValue)
		{
			valueOrDefault = player.Comp.Tape;
			tape = valueOrDefault;
		}
		if (tape < 0 || tape >= tapeComp.Songs.Count)
		{
			tape = 0;
		}
		if (tapeComp.Custom)
		{
			EntityUid? val = PlayCustomTrack(player, Entity<CassetteTapeComponent>.op_Implicit((tapeId.Value, tapeComp)));
			if (val.HasValue)
			{
				EntityUid custom = val.GetValueOrDefault();
				player.Comp.CustomAudioStream = custom;
			}
			player.Comp.Tape = 0;
		}
		else if (_net.IsServer)
		{
			SoundSpecifier song = tapeComp.Songs[tape.Value];
			AudioParams audioParams = player.Comp.AudioParams;
			ActorComponent actorComp = default(ActorComponent);
			if (((EntitySystem)this).TryComp<ActorComponent>(actor, ref actorComp))
			{
				float gain = _netConfig.GetClientCVar<float>(actorComp.PlayerSession.Channel, RMCCVars.VolumeGainCassettes);
				audioParams = ((AudioParams)(ref audioParams)).WithVolume(SharedAudioSystem.GainToVolume(gain));
			}
			player.Comp.AudioStream = _audio.PlayGlobal(song, actor, (AudioParams?)audioParams)?.Item1;
		}
		player.Comp.Tape = tape.Value;
		player.Comp.State = (AudioState)1;
		((EntitySystem)this).Dirty<CassettePlayerComponent>(player, (MetaDataComponent)null);
		_item.VisualsChanged(Entity<CassettePlayerComponent>.op_Implicit(player));
	}

	private void OnPlayerInteractUsing(Entity<CassettePlayerComponent> ent, ref InteractUsingEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<CassetteTapeComponent>(args.Used))
		{
			ContainerSlot slot = _container.EnsureContainer<ContainerSlot>(Entity<CassettePlayerComponent>.op_Implicit(ent), ent.Comp.ContainerId, (ContainerManagerComponent)null);
			EntityUid? contained = slot.ContainedEntity;
			if (contained.HasValue)
			{
				_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(contained.Value), (BaseContainer)(object)slot, true, false, (EntityCoordinates?)null, (Angle?)null);
			}
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(args.Used), (BaseContainer)(object)slot, (TransformComponent)null, false);
			if (contained.HasValue)
			{
				_hands.TryPickupAnyHand(args.User, contained.Value);
			}
			ent.Comp.Tape = 0;
			((EntitySystem)this).Dirty<CassettePlayerComponent>(ent, (MetaDataComponent)null);
			_audio.PlayLocal(ent.Comp.InsertEjectSound, Entity<CassettePlayerComponent>.op_Implicit(ent), (EntityUid?)args.User, (AudioParams?)null);
		}
	}

	private void OnPlayerEjectHand(Entity<CassettePlayerComponent> ent, ref RMCStorageEjectHandItemEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && _hands.IsHolding(Entity<HandsComponent>.op_Implicit(args.User), Entity<CassettePlayerComponent>.op_Implicit(ent)) && EjectTape(ent, args.User))
		{
			args.Handled = true;
		}
	}

	private void OnPlayerGetEquipmentVisuals(Entity<CassettePlayerComponent> ent, ref GetEquipmentVisualsEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Invalid comparison between Unknown and I4
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		args.Layers.Add(("cassette", new PrototypeLayerData
		{
			RsiPath = ((object)ent.Comp.WornSprite.RsiPath/*cast due to constrained. prefix*/).ToString(),
			State = ent.Comp.WornSprite.RsiState
		}));
		if ((int)ent.Comp.State == 1)
		{
			args.Layers.Add(("cassette_music", new PrototypeLayerData
			{
				RsiPath = ((object)ent.Comp.MusicSprite.RsiPath/*cast due to constrained. prefix*/).ToString(),
				State = ent.Comp.MusicSprite.RsiState
			}));
		}
	}

	private void OnPlayerUnequipped(Entity<CassettePlayerComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopAllAudio(ent);
	}

	private void OnPlayerExamined(Entity<CassettePlayerComponent> ent, ref ExaminedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("CassettePlayerComponent"))
		{
			if (TryGetTape(ent, out Entity<CassetteTapeComponent> tape))
			{
				args.PushMarkup(base.Loc.GetString("rmc-cassette-player-examine-tape", (ValueTuple<string, object>)("tape", tape)));
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("rmc-cassette-player-examine-none"));
			}
		}
	}

	private void OnPlayerState(Entity<CassettePlayerComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_item.VisualsChanged(Entity<CassettePlayerComponent>.op_Implicit(ent));
	}

	private void OnPlayerRemovedFromContainer(Entity<CassettePlayerComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_audio.Stop(_net.IsServer ? ent.Comp.AudioStream : ent.Comp.CustomAudioStream, (AudioComponent)null);
		ent.Comp.State = (AudioState)0;
		ent.Comp.Tape = 0;
		((EntitySystem)this).Dirty<CassettePlayerComponent>(ent, (MetaDataComponent)null);
	}

	private void OnPlayerGetVerbsAlternative(Entity<CassettePlayerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanAccess || !args.CanInteract)
		{
			return;
		}
		EntityUid user = args.User;
		if (!((EntitySystem)this).HasComp<XenoComponent>(user))
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("rmc-cassette-player-eject"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					EjectTape(ent, user);
				}
			});
		}
	}

	private void OnTapeExamined(Entity<CassetteTapeComponent> ent, ref ExaminedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		using (args.PushGroup("CassetteTapeComponent"))
		{
			if (ent.Comp.Custom)
			{
				args.PushMarkup(base.Loc.GetString("rmc-cassette-tape-custom"));
			}
			else
			{
				args.PushMarkup(base.Loc.GetString("rmc-cassette-tape-examine", (ValueTuple<string, object>)("total", ent.Comp.Songs.Count)));
			}
		}
	}

	protected virtual void OnPlayerUseInHand(Entity<CassetteTapeComponent> tape, ref UseInHandEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (tape.Comp.Custom)
		{
			((HandledEntityEventArgs)args).Handled = true;
			ChooseCustomTrack(tape);
		}
	}

	private void OnTapeGetVerbsAlternative(Entity<CassetteTapeComponent> tape, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (tape.Comp.Custom)
		{
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("rmc-cassette-tape-custom-choose"),
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					ChooseCustomTrack(tape);
				}
			});
		}
	}

	private bool TryGetTape(Entity<CassettePlayerComponent> player, out Entity<CassetteTapeComponent> tape)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		tape = default(Entity<CassetteTapeComponent>);
		BaseContainer container = default(BaseContainer);
		EntityUid? first = default(EntityUid?);
		CassetteTapeComponent tapeComp = default(CassetteTapeComponent);
		if (!_container.TryGetContainer(Entity<CassettePlayerComponent>.op_Implicit(player), player.Comp.ContainerId, ref container, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref first) || !((EntitySystem)this).TryComp<CassetteTapeComponent>(first, ref tapeComp))
		{
			return false;
		}
		tape = Entity<CassetteTapeComponent>.op_Implicit((first.Value, tapeComp));
		return true;
	}

	private Entity<CassetteTapeComponent>? GetTape(Entity<CassettePlayerComponent> player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetTape(player, out Entity<CassetteTapeComponent> tape))
		{
			return null;
		}
		return tape;
	}

	private int GetCurrentSongCount(Entity<CassettePlayerComponent> player)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return player.Comp.Tape + 1;
	}

	private int GetTotalSongs(Entity<CassettePlayerComponent> player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetTape(player, out Entity<CassetteTapeComponent> tape))
		{
			return 0;
		}
		int total = tape.Comp.Songs.Count;
		CassetteTapeComponent comp = tape.Comp;
		if (comp != null && comp.CustomTrack != null)
		{
			total++;
		}
		return total;
	}

	protected virtual EntityUid? PlayCustomTrack(Entity<CassettePlayerComponent> player, Entity<CassetteTapeComponent> tape)
	{
		return null;
	}

	protected virtual void ChooseCustomTrack(Entity<CassetteTapeComponent> tape)
	{
	}

	private void StopAllAudio(Entity<CassettePlayerComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_audio.Stop(ent.Comp.AudioStream, (AudioComponent)null);
		_audio.Stop(ent.Comp.CustomAudioStream, (AudioComponent)null);
		ent.Comp.State = (AudioState)0;
		((EntitySystem)this).Dirty<CassettePlayerComponent>(ent, (MetaDataComponent)null);
		_item.VisualsChanged(Entity<CassettePlayerComponent>.op_Implicit(ent));
	}

	private bool EjectTape(Entity<CassettePlayerComponent> ent, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		EntityUid? first = default(EntityUid?);
		if (!_container.TryGetContainer(Entity<CassettePlayerComponent>.op_Implicit(ent), ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null) || !Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>)container.ContainedEntities, ref first) || !_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(first.Value), container, true, false, (EntityCoordinates?)null, (Angle?)null))
		{
			return false;
		}
		_hands.TryPickupAnyHand(user, first.Value);
		_audio.PlayLocal(ent.Comp.InsertEjectSound, Entity<CassettePlayerComponent>.op_Implicit(ent), (EntityUid?)user, (AudioParams?)null);
		return true;
	}
}
