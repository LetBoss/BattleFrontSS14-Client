using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Teleporter;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.GameTicking;
using Content.Shared.Ghost;
using Content.Shared.Interaction;
using Content.Shared.Movement.Events;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Ladder;

public abstract class SharedLadderSystem : EntitySystem
{
	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedPlayerManager _player;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCTeleporterSystem _rmcTeleporter;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private readonly HashSet<Entity<LadderComponent>> _toUpdate = new HashSet<Entity<LadderComponent>>();

	private readonly Dictionary<string, HashSet<Entity<LadderComponent>>> _toUpdateIds = new Dictionary<string, HashSet<Entity<LadderComponent>>>();

	private EntityQuery<ActorComponent> _actorQuery;

	private EntityQuery<LadderComponent> _ladderQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_actorQuery = ((EntitySystem)this).GetEntityQuery<ActorComponent>();
		_ladderQuery = ((EntitySystem)this).GetEntityQuery<LadderComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RoundRestartCleanupEvent>((EntityEventHandler<RoundRestartCleanupEvent>)OnRoundRestartCleanup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, MapInitEvent>((EntityEventRefHandler<LadderComponent, MapInitEvent>)OnLadderMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, ComponentRemove>((EntityEventRefHandler<LadderComponent, ComponentRemove>)OnLadderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, EntityTerminatingEvent>((EntityEventRefHandler<LadderComponent, EntityTerminatingEvent>)OnLadderRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, ActivateInWorldEvent>((EntityEventRefHandler<LadderComponent, ActivateInWorldEvent>)OnLadderActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, DoAfterAttemptEvent<LadderDoAfterEvent>>((EntityEventRefHandler<LadderComponent, DoAfterAttemptEvent<LadderDoAfterEvent>>)OnLadderDoAfterAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, LadderDoAfterEvent>((EntityEventRefHandler<LadderComponent, LadderDoAfterEvent>)OnLadderDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<LadderComponent, GetVerbsEvent<AlternativeVerb>>)OnLadderGetAltVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, CanDropDraggedEvent>((EntityEventRefHandler<LadderComponent, CanDropDraggedEvent>)OnLadderCanDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, CanDragEvent>((EntityEventRefHandler<LadderComponent, CanDragEvent>)OnLadderCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderComponent, DragDropDraggedEvent>((EntityEventRefHandler<LadderComponent, DragDropDraggedEvent>)OnLadderDragDropDragged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LadderWatchingComponent, MoveInputEvent>((EntityEventRefHandler<LadderWatchingComponent, MoveInputEvent>)OnWatchingMoveInput, (Type[])null, (Type[])null);
	}

	private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
	{
		_toUpdate.Clear();
		_toUpdateIds.Clear();
	}

	private void OnLadderMapInit(Entity<LadderComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		_toUpdate.Add(ent);
	}

	public bool LadderIdInUse(string id)
	{
		if (_toUpdateIds.ContainsKey(id))
		{
			return true;
		}
		return false;
	}

	public void ReassignLadderId(Entity<LadderComponent> ent, string? newId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		LadderComponent ladder = default(LadderComponent);
		if (ent.Comp.Other.HasValue && ((EntitySystem)this).TryComp<LadderComponent>(ent.Comp.Other, ref ladder))
		{
			EntityUid other = ent.Comp.Other.Value;
			ent.Comp.Other = null;
			ladder.Id = null;
			ladder.Other = null;
			((EntitySystem)this).Dirty(other, (IComponent)(object)ladder, (MetaDataComponent)null);
		}
		if (ent.Comp.Id != null)
		{
			_toUpdateIds.Remove(ent.Comp.Id);
		}
		ent.Comp.Id = newId;
		((EntitySystem)this).Dirty<LadderComponent>(ent, (MetaDataComponent)null);
		_toUpdate.Add(ent);
	}

	private void OnLadderRemove<T>(Entity<LadderComponent> ent, ref T args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid watching in ent.Comp.Watching)
		{
			if (!((EntitySystem)this).TerminatingOrDeleted(watching, (MetaDataComponent)null))
			{
				((EntitySystem)this).RemCompDeferred<LadderWatchingComponent>(watching);
			}
		}
		LadderComponent otherLadder = default(LadderComponent);
		if (!((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Other, (MetaDataComponent)null) && _ladderQuery.TryComp(ent.Comp.Other, ref otherLadder))
		{
			otherLadder.Other = null;
			((EntitySystem)this).Dirty(ent.Comp.Other.Value, (IComponent)(object)otherLadder, (MetaDataComponent)null);
		}
		ent.Comp.Other = null;
	}

	private void OnLadderActivateInWorld(Entity<LadderComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		if (!ent.Comp.Other.HasValue)
		{
			string msg = base.Loc.GetString("rmc-ladder-leads-nowhere");
			_popup.PopupClient(msg, Entity<LadderComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityUid? lastDoAfterEnt = ent.Comp.LastDoAfterEnt;
		if (lastDoAfterEnt.HasValue)
		{
			EntityUid lastEnt = lastDoAfterEnt.GetValueOrDefault();
			ushort? lastDoAfterId = ent.Comp.LastDoAfterId;
			if (lastDoAfterId.HasValue)
			{
				ushort lastId = lastDoAfterId.GetValueOrDefault();
				if (time - ent.Comp.LastDoAfterTime < ent.Comp.Delay * 5.0 && _doAfter.GetStatus(new DoAfterId(lastEnt, lastId)) == DoAfterStatus.Running)
				{
					lastDoAfterEnt = ent.Comp.LastDoAfterEnt;
					EntityUid val = user;
					if (!lastDoAfterEnt.HasValue || lastDoAfterEnt.GetValueOrDefault() != val)
					{
						string msg2 = base.Loc.GetString("rmc-ladder-someone-else-climbing");
						_popup.PopupClient(msg2, Entity<LadderComponent>.op_Implicit(ent), user, PopupType.SmallCaution);
					}
					return;
				}
			}
		}
		LadderDoAfterEvent ev = new LadderDoAfterEvent();
		TimeSpan delay = ent.Comp.Delay;
		if (((EntitySystem)this).HasComp<GhostComponent>(args.User))
		{
			delay = TimeSpan.Zero;
		}
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, delay, ev, Entity<LadderComponent>.op_Implicit(ent), Entity<LadderComponent>.op_Implicit(ent), Entity<LadderComponent>.op_Implicit(ent))
		{
			AttemptFrequency = ((!(delay == TimeSpan.Zero)) ? AttemptFrequency.EveryTick : AttemptFrequency.Never)
		};
		if (_doAfter.TryStartDoAfter(doAfter, out var doAfterId))
		{
			ent.Comp.LastDoAfterEnt = doAfterId.Value.Uid;
			ent.Comp.LastDoAfterId = doAfterId.Value.Index;
			ent.Comp.LastDoAfterTime = time;
			((EntitySystem)this).Dirty<LadderComponent>(ent, (MetaDataComponent)null);
			if (ent.Comp.Delay > TimeSpan.Zero)
			{
				string selfMessage = base.Loc.GetString("rmc-ladder-start-climbing-self");
				string othersMessage = base.Loc.GetString("rmc-ladder-start-climbing-others", (ValueTuple<string, object>)("user", user));
				_popup.PopupPredicted(selfMessage, othersMessage, user, user);
			}
			ActorComponent actor = default(ActorComponent);
			if (_actorQuery.TryComp(user, ref actor))
			{
				AddViewer(ent, actor.PlayerSession);
			}
		}
	}

	private void OnLadderDoAfterAttempt(Entity<LadderComponent> ent, ref DoAfterAttemptEvent<LadderDoAfterEvent> args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled)
		{
			EntityUid user = args.DoAfter.Args.User;
			EntityCoordinates target = ent.Owner.ToCoordinates();
			EntityCoordinates val = user.ToCoordinates();
			float distance = default(float);
			if (((EntityCoordinates)(ref val)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, target, ref distance) && distance > ent.Comp.Range)
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnLadderDoAfter(Entity<LadderComponent> ent, ref LadderDoAfterEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityUid? localEntity;
		if (_net.IsClient)
		{
			EntityUid val = user;
			localEntity = _player.LocalEntity;
			if (!localEntity.HasValue || val != localEntity.GetValueOrDefault())
			{
				return;
			}
		}
		ActorComponent actor = default(ActorComponent);
		if (_actorQuery.TryComp(user, ref actor))
		{
			RemoveViewer(ent, actor.PlayerSession);
		}
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		localEntity = ent.Comp.Other;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid other = localEntity.GetValueOrDefault();
		if (!((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Other, (MetaDataComponent)null))
		{
			MapCoordinates coordinates = _transform.GetMapCoordinates(other, (TransformComponent)null);
			if (!(coordinates.MapId == MapId.Nullspace))
			{
				_transform.SetMapCoordinates(user, coordinates);
				string selfMessage = base.Loc.GetString("rmc-ladder-finish-climbing-self");
				string othersMessage = base.Loc.GetString("rmc-ladder-finish-climbing-others", (ValueTuple<string, object>)("user", user));
				_popup.PopupPredicted(selfMessage, othersMessage, user, user);
				ent.Comp.LastDoAfterEnt = null;
				ent.Comp.LastDoAfterId = null;
				((EntitySystem)this).Dirty<LadderComponent>(ent, (MetaDataComponent)null);
				_rmcTeleporter.HandlePulling(user, coordinates);
			}
		}
	}

	private void OnLadderGetAltVerbs(Entity<LadderComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? other = ent.Comp.Other;
		if (!other.HasValue)
		{
			return;
		}
		EntityUid other2 = other.GetValueOrDefault();
		EntityUid user = args.User;
		if (!CanWatchPopup(ent, user))
		{
			return;
		}
		args.Verbs.Add(new AlternativeVerb
		{
			Priority = 100,
			Act = delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				if (CanWatchPopup(ent, user))
				{
					Watch(Entity<ActorComponent, EyeComponent>.op_Implicit(user), Entity<LadderComponent>.op_Implicit(other2));
				}
			},
			Text = base.Loc.GetString("rmc-ladder-look-through")
		});
	}

	private void OnLadderCanDropDragged(Entity<LadderComponent> ent, ref CanDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != args.Target))
		{
			args.Handled = true;
			args.CanDrop = true;
		}
	}

	private void OnLadderCanDrag(Entity<LadderComponent> ent, ref CanDragEvent args)
	{
		args.Handled = true;
	}

	private void OnLadderDragDropDragged(Entity<LadderComponent> ent, ref DragDropDraggedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid user = args.User;
		EntityUid? other = ent.Comp.Other;
		if (other.HasValue)
		{
			EntityUid other2 = other.GetValueOrDefault();
			if (!(user != args.Target) && CanWatchPopup(ent, user))
			{
				args.Handled = true;
				Watch(Entity<ActorComponent, EyeComponent>.op_Implicit(user), Entity<LadderComponent>.op_Implicit(other2));
			}
		}
	}

	private void OnWatchingMoveInput(Entity<LadderWatchingComponent> ent, ref MoveInputEvent args)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!args.HasDirectionalMovement)
		{
			return;
		}
		if (_net.IsClient)
		{
			EntityUid? localEntity = _player.LocalEntity;
			EntityUid owner = ent.Owner;
			if (localEntity.HasValue && localEntity.GetValueOrDefault() == owner && _player.LocalSession != null)
			{
				Unwatch(Entity<EyeComponent>.op_Implicit(ent.Owner), _player.LocalSession);
				return;
			}
		}
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(Entity<LadderWatchingComponent>.op_Implicit(ent), ref actor))
		{
			Unwatch(Entity<EyeComponent>.op_Implicit(ent.Owner), actor.PlayerSession);
		}
	}

	protected virtual void AddViewer(Entity<LadderComponent> ent, ICommonSession player)
	{
	}

	protected virtual void RemoveViewer(Entity<LadderComponent> ent, ICommonSession player)
	{
	}

	protected virtual void Watch(Entity<ActorComponent?, EyeComponent?> watcher, Entity<LadderComponent?> toWatch)
	{
	}

	protected virtual void Unwatch(Entity<EyeComponent?> watcher, ICommonSession player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<EyeComponent>(Entity<EyeComponent>.op_Implicit(watcher), ref watcher.Comp, true))
		{
			_eye.SetTarget(Entity<EyeComponent>.op_Implicit(watcher), (EntityUid?)null, (EyeComponent)null);
		}
	}

	protected bool CanWatchPopup(Entity<LadderComponent> ladder, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(ladder.Owner), 1.5f, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: true))
		{
			return false;
		}
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		if (_toUpdate.Count == 0)
		{
			return;
		}
		if (_net.IsClient)
		{
			_toUpdateIds.Clear();
			_toUpdate.Clear();
			return;
		}
		_toUpdateIds.Clear();
		foreach (Entity<LadderComponent> entity in _toUpdate)
		{
			string id = entity.Comp.Id;
			if (id == null)
			{
				continue;
			}
			HashSet<Entity<LadderComponent>> ids = Extensions.GetOrNew<string, HashSet<Entity<LadderComponent>>>(_toUpdateIds, id);
			if (ids.Count > 2)
			{
				string idsString = string.Join(",", ids.Select((Entity<LadderComponent> e) => ((EntitySystem)this).ToPrettyString((EntityUid?)Entity<LadderComponent>.op_Implicit(e), (MetaDataComponent)null)));
				((EntitySystem)this).Log.Error($"Found more than 2 ladders with id {id}, current ladder: {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<LadderComponent>.op_Implicit(entity), (MetaDataComponent)null)}, previous ladders: {idsString}");
			}
			ids.Add(entity);
		}
		_toUpdate.Clear();
		EntityQueryEnumerator<LadderComponent> ladders = ((EntitySystem)this).EntityQueryEnumerator<LadderComponent>();
		EntityUid uid = default(EntityUid);
		LadderComponent ladder = default(LadderComponent);
		while (true)
		{
			if (!ladders.MoveNext(ref uid, ref ladder))
			{
				break;
			}
			if (ladder.Id == null || !_toUpdateIds.TryGetValue(ladder.Id, out HashSet<Entity<LadderComponent>> ids2))
			{
				continue;
			}
			Entity<LadderComponent>? val = Extensions.FirstOrNull<Entity<LadderComponent>>((IEnumerable<Entity<LadderComponent>>)ids2, (Func<Entity<LadderComponent>, bool>)((Entity<LadderComponent> e) => e.Owner != uid));
			if (!val.HasValue)
			{
				continue;
			}
			Entity<LadderComponent> toUpdate = val.GetValueOrDefault();
			if (toUpdate.Owner == uid)
			{
				continue;
			}
			EntityUid? other = ladder.Other;
			if (other.HasValue)
			{
				EntityUid old = other.GetValueOrDefault();
				if (old != toUpdate.Owner)
				{
					((EntitySystem)this).Log.Error($"Found {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<LadderComponent>.op_Implicit(toUpdate), (MetaDataComponent)null)} with duplicate ID {toUpdate.Comp.Id}, previous ladder: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(old))}");
				}
			}
			ladder.Other = Entity<LadderComponent>.op_Implicit(toUpdate);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)ladder, (MetaDataComponent)null);
			toUpdate.Comp.Other = uid;
			((EntitySystem)this).Dirty<LadderComponent>(toUpdate, (MetaDataComponent)null);
		}
	}
}
