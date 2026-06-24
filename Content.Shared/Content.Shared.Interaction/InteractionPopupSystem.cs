using System;
using Content.Shared.Bed.Sleep;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Interaction;

public sealed class InteractionPopupSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private MobStateSystem _mobStateSystem;

	[Dependency]
	private SharedPopupSystem _popupSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private INetManager _netMan;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<InteractionPopupComponent, InteractHandEvent>((ComponentEventHandler<InteractionPopupComponent, InteractHandEvent>)OnInteractHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InteractionPopupComponent, ActivateInWorldEvent>((ComponentEventHandler<InteractionPopupComponent, ActivateInWorldEvent>)OnActivateInWorld, (Type[])null, (Type[])null);
	}

	private void OnActivateInWorld(EntityUid uid, InteractionPopupComponent component, ActivateInWorldEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (args.Complex && component.OnActivate)
		{
			SharedInteract(uid, component, (HandledEntityEventArgs)(object)args, args.Target, args.User);
		}
	}

	private void OnInteractHand(EntityUid uid, InteractionPopupComponent component, InteractHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		SharedInteract(uid, component, (HandledEntityEventArgs)(object)args, args.Target, args.User);
	}

	private void SharedInteract(EntityUid uid, InteractionPopupComponent component, HandledEntityEventArgs args, EntityUid target, EntityUid user)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent state = default(MobStateComponent);
		if (args.Handled || user == target || ((EntitySystem)this).HasComp<SleepingComponent>(uid) || (((EntitySystem)this).TryComp<MobStateComponent>(uid, ref state) && !_mobStateSystem.IsAlive(uid, state)))
		{
			return;
		}
		args.Handled = true;
		TimeSpan curTime = _gameTiming.CurTime;
		if (curTime < component.LastInteractTime + component.InteractDelay)
		{
			return;
		}
		component.LastInteractTime = curTime;
		string msg = "";
		SoundSpecifier sfx = null;
		float successChance = component.SuccessChance;
		bool flag = ((successChance == 0f || successChance == 1f) ? true : false);
		bool predict = flag && !component.InteractSuccessSpawn.HasValue && !component.InteractFailureSpawn.HasValue;
		if (_netMan.IsClient && !predict)
		{
			return;
		}
		if (RandomExtensions.Prob(_random, component.SuccessChance))
		{
			if (component.InteractSuccessString != null)
			{
				msg = base.Loc.GetString(component.InteractSuccessString, (ValueTuple<string, object>)("target", Identity.Name(uid, (IEntityManager)(object)base.EntityManager, user)));
			}
			if (component.InteractSuccessSound != null)
			{
				sfx = component.InteractSuccessSound;
			}
			if (component.InteractSuccessSpawn.HasValue)
			{
				EntProtoId? interactSuccessSpawn = component.InteractSuccessSpawn;
				((EntitySystem)this).Spawn(interactSuccessSpawn.HasValue ? EntProtoId.op_Implicit(interactSuccessSpawn.GetValueOrDefault()) : null, _transform.GetMapCoordinates(uid, (TransformComponent)null), (ComponentRegistry)null, default(Angle));
			}
			InteractionSuccessEvent ev = new InteractionSuccessEvent(user);
			((EntitySystem)this).RaiseLocalEvent<InteractionSuccessEvent>(target, ref ev, false);
		}
		else
		{
			if (component.InteractFailureString != null)
			{
				msg = base.Loc.GetString(component.InteractFailureString, (ValueTuple<string, object>)("target", Identity.Name(uid, (IEntityManager)(object)base.EntityManager, user)));
			}
			if (component.InteractFailureSound != null)
			{
				sfx = component.InteractFailureSound;
			}
			if (component.InteractFailureSpawn.HasValue)
			{
				EntProtoId? interactSuccessSpawn = component.InteractFailureSpawn;
				((EntitySystem)this).Spawn(interactSuccessSpawn.HasValue ? EntProtoId.op_Implicit(interactSuccessSpawn.GetValueOrDefault()) : null, _transform.GetMapCoordinates(uid, (TransformComponent)null), (ComponentRegistry)null, default(Angle));
			}
			InteractionFailureEvent ev2 = new InteractionFailureEvent(user);
			((EntitySystem)this).RaiseLocalEvent<InteractionFailureEvent>(target, ref ev2, false);
		}
		if (!string.IsNullOrEmpty(component.MessagePerceivedByOthers))
		{
			foreach (ICommonSession recipient in Filter.PvsExcept(user, 2f, (IEntityManager)(object)base.EntityManager).Recipients)
			{
				EntityUid? attachedEntity = recipient.AttachedEntity;
				if (attachedEntity.HasValue)
				{
					EntityUid otherEnt = attachedEntity.GetValueOrDefault();
					string msgOther = base.Loc.GetString(component.MessagePerceivedByOthers, (ValueTuple<string, object>)("user", Identity.Entity(user, (IEntityManager)(object)base.EntityManager, otherEnt)), (ValueTuple<string, object>)("target", Identity.Name(uid, (IEntityManager)(object)base.EntityManager, otherEnt)));
					_popupSystem.PopupEntity(msgOther, uid, otherEnt);
				}
			}
		}
		if (!predict)
		{
			_popupSystem.PopupEntity(msg, uid, user);
			if (component.SoundPerceivedByOthers)
			{
				_audio.PlayPvs(sfx, target, (AudioParams?)null);
				return;
			}
			_audio.PlayEntity(sfx, Filter.Entities((EntityUid[])(object)new EntityUid[2] { user, target }), target, false, (AudioParams?)null);
			return;
		}
		_popupSystem.PopupClient(msg, uid, user);
		if (sfx == null)
		{
			return;
		}
		if (component.SoundPerceivedByOthers)
		{
			_audio.PlayPredicted(sfx, target, (EntityUid?)user, (AudioParams?)null);
		}
		else if (_netMan.IsClient)
		{
			if (_gameTiming.IsFirstTimePredicted)
			{
				_audio.PlayEntity(sfx, Filter.Local(), target, true, (AudioParams?)null);
			}
		}
		else
		{
			_audio.PlayEntity(sfx, Filter.Empty().FromEntities((EntityUid[])(object)new EntityUid[1] { target }), target, false, (AudioParams?)null);
		}
	}

	public void SetInteractSuccessString(Entity<InteractionPopupComponent> ent, string str)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InteractSuccessString = str;
	}

	public void SetInteractFailureString(Entity<InteractionPopupComponent> ent, string str)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.InteractFailureString = str;
	}
}
