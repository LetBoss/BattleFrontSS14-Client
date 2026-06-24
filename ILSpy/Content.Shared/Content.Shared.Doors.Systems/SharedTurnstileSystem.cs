using System;
using System.Numerics;
using Content.Shared.Access.Systems;
using Content.Shared.Doors.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared.Doors.Systems;

public abstract class SharedTurnstileSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private AccessReaderSystem _accessReader;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private PullingSystem _pulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<TurnstileComponent, PreventCollideEvent>((EntityEventRefHandler<TurnstileComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TurnstileComponent, StartCollideEvent>((EntityEventRefHandler<TurnstileComponent, StartCollideEvent>)OnStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TurnstileComponent, EndCollideEvent>((EntityEventRefHandler<TurnstileComponent, EndCollideEvent>)OnEndCollide, (Type[])null, (Type[])null);
	}

	private void OnPreventCollide(Entity<TurnstileComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || !args.OurFixture.Hard || !args.OtherFixture.Hard)
		{
			return;
		}
		if (ent.Comp.CollideExceptions.Contains(args.OtherEntity))
		{
			args.Cancelled = true;
			return;
		}
		EntityUid? puller = _pulling.GetPuller(args.OtherEntity);
		if (puller.HasValue)
		{
			EntityUid puller2 = puller.GetValueOrDefault();
			if (ent.Comp.CollideExceptions.Contains(puller2))
			{
				ent.Comp.CollideExceptions.Add(args.OtherEntity);
				((EntitySystem)this).Dirty<TurnstileComponent>(ent, (MetaDataComponent)null);
				args.Cancelled = true;
				return;
			}
		}
		if (_entityWhitelist.IsWhitelistFail(ent.Comp.ProcessWhitelist, args.OtherEntity))
		{
			args.Cancelled = true;
		}
		else if (CanPassDirection(ent, args.OtherEntity))
		{
			if (_accessReader.IsAllowed(args.OtherEntity, Entity<TurnstileComponent>.op_Implicit(ent)))
			{
				ent.Comp.CollideExceptions.Add(args.OtherEntity);
				puller = _pulling.GetPulling(args.OtherEntity);
				if (puller.HasValue)
				{
					EntityUid uid = puller.GetValueOrDefault();
					ent.Comp.CollideExceptions.Add(uid);
				}
				args.Cancelled = true;
				((EntitySystem)this).Dirty<TurnstileComponent>(ent, (MetaDataComponent)null);
			}
		}
		else if (_timing.CurTime >= ent.Comp.NextResistTime)
		{
			_popup.PopupClient(base.Loc.GetString("turnstile-component-popup-resist", (ValueTuple<string, object>)("turnstile", ent.Owner)), Entity<TurnstileComponent>.op_Implicit(ent), args.OtherEntity);
			ent.Comp.NextResistTime = _timing.CurTime + TimeSpan.FromSeconds(0.1);
			((EntitySystem)this).Dirty<TurnstileComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnStartCollide(Entity<TurnstileComponent> ent, ref StartCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.CollideExceptions.Contains(args.OtherEntity))
		{
			if (CanPassDirection(ent, args.OtherEntity) && !_accessReader.IsAllowed(args.OtherEntity, Entity<TurnstileComponent>.op_Implicit(ent)))
			{
				_audio.PlayPredicted(ent.Comp.DenySound, Entity<TurnstileComponent>.op_Implicit(ent), (EntityUid?)args.OtherEntity, (AudioParams?)null);
				PlayAnimation(Entity<TurnstileComponent>.op_Implicit(ent), ent.Comp.DenyState);
			}
		}
		else
		{
			PlayAnimation(Entity<TurnstileComponent>.op_Implicit(ent), ent.Comp.SpinState);
			_audio.PlayPredicted(ent.Comp.TurnSound, Entity<TurnstileComponent>.op_Implicit(ent), (EntityUid?)args.OtherEntity, (AudioParams?)null);
		}
	}

	private void OnEndCollide(Entity<TurnstileComponent> ent, ref EndCollideEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!args.OurFixture.Hard)
		{
			ent.Comp.CollideExceptions.Remove(args.OtherEntity);
			((EntitySystem)this).Dirty<TurnstileComponent>(ent, (MetaDataComponent)null);
		}
	}

	protected bool CanPassDirection(Entity<TurnstileComponent> ent, EntityUid other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(Entity<TurnstileComponent>.op_Implicit(ent));
		TransformComponent otherXform = ((EntitySystem)this).Transform(other);
		ValueTuple<Vector2, Angle> worldPositionRotation = _transform.GetWorldPositionRotation(xform);
		Vector2 pos = worldPositionRotation.Item1;
		Angle rot = worldPositionRotation.Item2;
		Vector2 otherPos = _transform.GetWorldPosition(otherXform);
		Angle val = DirectionExtensions.ToAngle(pos - otherPos);
		Angle rotateAngle = DirectionExtensions.ToAngle(((Angle)(ref rot)).ToWorldVec());
		double diff = Math.Abs(Angle.op_Implicit(val - rotateAngle));
		diff %= 6.2831854820251465;
		if (diff > Math.PI)
		{
			diff = 6.2831854820251465 - diff;
		}
		return diff < Math.PI / 4.0;
	}

	protected virtual void PlayAnimation(EntityUid uid, string stateId)
	{
	}
}
