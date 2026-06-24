using System;
using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Teleportation.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Shared.Teleportation.Systems;

public sealed class SwapTeleporterSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	private EntityQuery<TransformComponent> _xformQuery;

	public override void Initialize()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).SubscribeLocalEvent<SwapTeleporterComponent, AfterInteractEvent>((EntityEventRefHandler<SwapTeleporterComponent, AfterInteractEvent>)OnInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SwapTeleporterComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<SwapTeleporterComponent, GetVerbsEvent<AlternativeVerb>>)OnGetAltVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SwapTeleporterComponent, ActivateInWorldEvent>((EntityEventRefHandler<SwapTeleporterComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SwapTeleporterComponent, ExaminedEvent>((EntityEventRefHandler<SwapTeleporterComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SwapTeleporterComponent, ComponentShutdown>((EntityEventRefHandler<SwapTeleporterComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
	}

	private void OnInteract(Entity<SwapTeleporterComponent> ent, ref AfterInteractEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Entity<SwapTeleporterComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SwapTeleporterComponent swapTeleporterComponent = default(SwapTeleporterComponent);
		val.Deconstruct(ref val2, ref swapTeleporterComponent);
		EntityUid uid = val2;
		SwapTeleporterComponent comp = swapTeleporterComponent;
		if (!args.Target.HasValue || !args.CanReach)
		{
			return;
		}
		EntityUid target = args.Target.Value;
		SwapTeleporterComponent targetComp = default(SwapTeleporterComponent);
		if (((EntitySystem)this).TryComp<SwapTeleporterComponent>(target, ref targetComp) && !_whitelistSystem.IsWhitelistFail(comp.TeleporterWhitelist, target) && !_whitelistSystem.IsWhitelistFail(targetComp.TeleporterWhitelist, uid))
		{
			if (comp.LinkedEnt.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-link-fail-already"), uid, args.User);
				return;
			}
			if (targetComp.LinkedEnt.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-link-fail-already-other"), uid, args.User);
				return;
			}
			comp.LinkedEnt = target;
			targetComp.LinkedEnt = uid;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			((EntitySystem)this).Dirty(target, (IComponent)(object)targetComp, (MetaDataComponent)null);
			_appearance.SetData(uid, (Enum)SwapTeleporterVisuals.Linked, (object)true, (AppearanceComponent)null);
			_appearance.SetData(target, (Enum)SwapTeleporterVisuals.Linked, (object)true, (AppearanceComponent)null);
			_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-link-create"), uid, args.User);
		}
	}

	private void OnGetAltVerb(Entity<SwapTeleporterComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Entity<SwapTeleporterComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SwapTeleporterComponent swapTeleporterComponent = default(SwapTeleporterComponent);
		val.Deconstruct(ref val2, ref swapTeleporterComponent);
		EntityUid uid = val2;
		SwapTeleporterComponent comp = swapTeleporterComponent;
		SwapTeleporterComponent otherComp = default(SwapTeleporterComponent);
		if (args.CanAccess && args.CanInteract && args.Hands != null && !comp.TeleportTime.HasValue && ((EntitySystem)this).TryComp<SwapTeleporterComponent>(comp.LinkedEnt, ref otherComp) && !otherComp.TeleportTime.HasValue)
		{
			EntityUid user = args.User;
			args.Verbs.Add(new AlternativeVerb
			{
				Text = base.Loc.GetString("swap-teleporter-verb-destroy-link"),
				Priority = 1,
				Act = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					DestroyLink(Entity<SwapTeleporterComponent>.op_Implicit((uid, comp)), user);
				}
			});
		}
	}

	private void OnActivateInWorld(Entity<SwapTeleporterComponent> ent, ref ActivateInWorldEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !args.Complex)
		{
			return;
		}
		Entity<SwapTeleporterComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SwapTeleporterComponent swapTeleporterComponent = default(SwapTeleporterComponent);
		val.Deconstruct(ref val2, ref swapTeleporterComponent);
		EntityUid uid = val2;
		SwapTeleporterComponent comp = swapTeleporterComponent;
		EntityUid user = args.User;
		if (comp.TeleportTime.HasValue)
		{
			return;
		}
		SwapTeleporterComponent otherComp = default(SwapTeleporterComponent);
		if (!comp.LinkedEnt.HasValue)
		{
			_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-teleport-cancel-link"), Entity<SwapTeleporterComponent>.op_Implicit(ent), user);
		}
		else if (((EntitySystem)this).TryComp<SwapTeleporterComponent>(comp.LinkedEnt, ref otherComp) && !otherComp.TeleportTime.HasValue)
		{
			if (_timing.CurTime < comp.NextTeleportUse)
			{
				_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-teleport-cancel-time"), Entity<SwapTeleporterComponent>.op_Implicit(ent), user);
				return;
			}
			_audio.PlayPredicted(comp.TeleportSound, uid, (EntityUid?)user, (AudioParams?)null);
			_audio.PlayPredicted(otherComp.TeleportSound, comp.LinkedEnt.Value, (EntityUid?)user, (AudioParams?)null);
			comp.NextTeleportUse = _timing.CurTime + comp.Cooldown;
			comp.TeleportTime = _timing.CurTime + comp.TeleportDelay;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public void DoTeleport(Entity<SwapTeleporterComponent, TransformComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		Entity<SwapTeleporterComponent, TransformComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SwapTeleporterComponent swapTeleporterComponent = default(SwapTeleporterComponent);
		TransformComponent val3 = default(TransformComponent);
		val.Deconstruct(ref val2, ref swapTeleporterComponent, ref val3);
		EntityUid uid = val2;
		SwapTeleporterComponent comp = swapTeleporterComponent;
		TransformComponent xform = val3;
		comp.TeleportTime = null;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
		if (_net.IsClient)
		{
			return;
		}
		EntityUid? linkedEnt = comp.LinkedEnt;
		if (linkedEnt.HasValue)
		{
			EntityUid linkedEnt2 = linkedEnt.GetValueOrDefault();
			EntityUid teleEnt = GetTeleportingEntity(Entity<TransformComponent>.op_Implicit((uid, xform)));
			EntityUid otherTeleEnt = GetTeleportingEntity(Entity<TransformComponent>.op_Implicit((linkedEnt2, ((EntitySystem)this).Transform(linkedEnt2))));
			TransformComponent teleXform = ((EntitySystem)this).Transform(teleEnt);
			TransformComponent otherTeleXform = ((EntitySystem)this).Transform(otherTeleEnt);
			if (!CanSwapTeleport(Entity<TransformComponent>.op_Implicit((teleEnt, teleXform)), Entity<TransformComponent>.op_Implicit((otherTeleEnt, otherTeleXform))))
			{
				_popup.PopupEntity(base.Loc.GetString("swap-teleporter-popup-teleport-fail", (ValueTuple<string, object>)("entity", Identity.Entity(linkedEnt2, (IEntityManager)(object)base.EntityManager))), teleEnt, teleEnt, PopupType.MediumCaution);
				return;
			}
			_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-teleport-other", (ValueTuple<string, object>)("entity", Identity.Entity(linkedEnt2, (IEntityManager)(object)base.EntityManager))), teleEnt, otherTeleEnt, PopupType.MediumCaution);
			_transform.SwapPositions(Entity<TransformComponent>.op_Implicit(teleEnt), Entity<TransformComponent>.op_Implicit(otherTeleEnt));
		}
	}

	private bool CanSwapTeleport(Entity<TransformComponent> entity1, Entity<TransformComponent> entity2)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container1 = default(BaseContainer);
		_container.TryGetOuterContainer(Entity<TransformComponent>.op_Implicit(entity1), Entity<TransformComponent>.op_Implicit(entity1), ref container1);
		BaseContainer container2 = default(BaseContainer);
		_container.TryGetOuterContainer(Entity<TransformComponent>.op_Implicit(entity2), Entity<TransformComponent>.op_Implicit(entity2), ref container2);
		if ((container2 != null && !_container.CanInsert(Entity<TransformComponent>.op_Implicit(entity1), container2, false, (TransformComponent)null)) || (container1 != null && !_container.CanInsert(Entity<TransformComponent>.op_Implicit(entity2), container1, false, (TransformComponent)null)))
		{
			return false;
		}
		if (((EntitySystem)this).IsPaused((EntityUid?)Entity<TransformComponent>.op_Implicit(entity1), (MetaDataComponent)null) || ((EntitySystem)this).IsPaused((EntityUid?)Entity<TransformComponent>.op_Implicit(entity2), (MetaDataComponent)null))
		{
			return false;
		}
		return true;
	}

	public void DestroyLink(Entity<SwapTeleporterComponent?> ent, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<SwapTeleporterComponent>(Entity<SwapTeleporterComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			EntityUid? linkedNullable = ent.Comp.LinkedEnt;
			ent.Comp.LinkedEnt = null;
			ent.Comp.TeleportTime = null;
			_appearance.SetData(Entity<SwapTeleporterComponent>.op_Implicit(ent), (Enum)SwapTeleporterVisuals.Linked, (object)false, (AppearanceComponent)null);
			((EntitySystem)this).Dirty(Entity<SwapTeleporterComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
			if (user.HasValue)
			{
				_popup.PopupClient(base.Loc.GetString("swap-teleporter-popup-link-destroyed"), Entity<SwapTeleporterComponent>.op_Implicit(ent), user.Value);
			}
			else
			{
				_popup.PopupEntity(base.Loc.GetString("swap-teleporter-popup-link-destroyed"), Entity<SwapTeleporterComponent>.op_Implicit(ent));
			}
			if (linkedNullable.HasValue)
			{
				EntityUid linked = linkedNullable.GetValueOrDefault();
				DestroyLink(Entity<SwapTeleporterComponent>.op_Implicit(linked), user);
			}
		}
	}

	private EntityUid GetTeleportingEntity(Entity<TransformComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Invalid comparison between Unknown and I4
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid parent = ent.Comp.ParentUid;
		if (((EntitySystem)this).HasComp<MapGridComponent>(parent) || ((EntitySystem)this).HasComp<MapComponent>(parent))
		{
			return Entity<TransformComponent>.op_Implicit(ent);
		}
		TransformComponent parentXform = default(TransformComponent);
		if (!_xformQuery.TryGetComponent(parent, ref parentXform) || parentXform.Anchored)
		{
			return Entity<TransformComponent>.op_Implicit(ent);
		}
		PhysicsComponent body = default(PhysicsComponent);
		if (!((EntitySystem)this).TryComp<PhysicsComponent>(parent, ref body) || (int)body.BodyType == 4)
		{
			return Entity<TransformComponent>.op_Implicit(ent);
		}
		return GetTeleportingEntity(Entity<TransformComponent>.op_Implicit((parent, parentXform)));
	}

	private void OnExamined(Entity<SwapTeleporterComponent> ent, ref ExaminedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Entity<SwapTeleporterComponent> val = ent;
		EntityUid val2 = default(EntityUid);
		SwapTeleporterComponent swapTeleporterComponent = default(SwapTeleporterComponent);
		val.Deconstruct(ref val2, ref swapTeleporterComponent);
		SwapTeleporterComponent comp = swapTeleporterComponent;
		using (args.PushGroup("SwapTeleporterComponent"))
		{
			string locale = ((!comp.LinkedEnt.HasValue) ? "swap-teleporter-examine-link-absent" : "swap-teleporter-examine-link-present");
			args.PushMarkup(base.Loc.GetString(locale));
			if (_timing.CurTime < comp.NextTeleportUse)
			{
				args.PushMarkup(base.Loc.GetString("swap-teleporter-examine-time-remaining", (ValueTuple<string, object>)("second", (int)((comp.NextTeleportUse - _timing.CurTime).TotalSeconds + 0.5))));
			}
		}
	}

	private void OnShutdown(Entity<SwapTeleporterComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		DestroyLink(Entity<SwapTeleporterComponent>.op_Implicit((Entity<SwapTeleporterComponent>.op_Implicit(ent), Entity<SwapTeleporterComponent>.op_Implicit(ent))), null);
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<SwapTeleporterComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<SwapTeleporterComponent, TransformComponent>();
		EntityUid uid = default(EntityUid);
		SwapTeleporterComponent comp = default(SwapTeleporterComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid, ref comp, ref xform))
		{
			if (comp.TeleportTime.HasValue)
			{
				TimeSpan curTime = _timing.CurTime;
				TimeSpan? teleportTime = comp.TeleportTime;
				if (!(curTime < teleportTime))
				{
					DoTeleport(Entity<SwapTeleporterComponent, TransformComponent>.op_Implicit((uid, comp, xform)));
				}
			}
		}
	}
}
