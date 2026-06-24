using System;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Nutrition.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Nutrition.EntitySystems;

public sealed class ShakeableSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ShakeableComponent, GetVerbsEvent<Verb>>((ComponentEventHandler<ShakeableComponent, GetVerbsEvent<Verb>>)AddShakeVerb, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ShakeableComponent, ShakeDoAfterEvent>((EntityEventRefHandler<ShakeableComponent, ShakeDoAfterEvent>)OnShakeDoAfter, (Type[])null, (Type[])null);
	}

	private void AddShakeVerb(EntityUid uid, ShakeableComponent component, GetVerbsEvent<Verb> args)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (args.Hands != null && args.CanAccess && args.CanInteract && CanShake(Entity<ShakeableComponent>.op_Implicit((uid, component)), args.User))
		{
			Verb shakeVerb = new Verb
			{
				Text = base.Loc.GetString(LocId.op_Implicit(component.ShakeVerbText)),
				Act = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					TryStartShake(Entity<ShakeableComponent>.op_Implicit((args.Target, component)), args.User);
				}
			};
			args.Verbs.Add(shakeVerb);
		}
	}

	private void OnShakeDoAfter(Entity<ShakeableComponent> entity, ref ShakeDoAfterEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled)
		{
			TryShake(Entity<ShakeableComponent>.op_Implicit((Entity<ShakeableComponent>.op_Implicit(entity), entity.Comp)), args.User);
		}
	}

	public bool TryStartShake(Entity<ShakeableComponent?> entity, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ShakeableComponent>(Entity<ShakeableComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		if (!CanShake(entity, user))
		{
			return false;
		}
		DoAfterArgs doAfterArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, user, entity.Comp.ShakeDuration, new ShakeDoAfterEvent(), Entity<ShakeableComponent>.op_Implicit(entity), user, Entity<ShakeableComponent>.op_Implicit(entity))
		{
			NeedHand = true,
			BreakOnDamage = true,
			DistanceThreshold = 1f,
			MovementThreshold = 0.01f,
			BreakOnHandChange = entity.Comp.RequireInHand
		};
		if (entity.Comp.RequireInHand)
		{
			doAfterArgs.BreakOnHandChange = true;
		}
		if (!_doAfter.TryStartDoAfter(doAfterArgs))
		{
			return false;
		}
		EntityUid userName = Identity.Entity(user, (IEntityManager)(object)base.EntityManager);
		EntityUid shakeableName = Identity.Entity(Entity<ShakeableComponent>.op_Implicit(entity), (IEntityManager)(object)base.EntityManager);
		string selfMessage = base.Loc.GetString(LocId.op_Implicit(entity.Comp.ShakePopupMessageSelf), (ValueTuple<string, object>)("user", userName), (ValueTuple<string, object>)("shakeable", shakeableName));
		string othersMessage = base.Loc.GetString(LocId.op_Implicit(entity.Comp.ShakePopupMessageOthers), (ValueTuple<string, object>)("user", userName), (ValueTuple<string, object>)("shakeable", shakeableName));
		_popup.PopupPredicted(selfMessage, othersMessage, user, user);
		_audio.PlayPredicted(entity.Comp.ShakeSound, Entity<ShakeableComponent>.op_Implicit(entity), (EntityUid?)user, (AudioParams?)null);
		return true;
	}

	public bool TryShake(Entity<ShakeableComponent?> entity, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ShakeableComponent>(Entity<ShakeableComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		if (!CanShake(entity, user))
		{
			return false;
		}
		ShakeEvent ev = new ShakeEvent(user);
		((EntitySystem)this).RaiseLocalEvent<ShakeEvent>(Entity<ShakeableComponent>.op_Implicit(entity), ref ev, false);
		return true;
	}

	public bool CanShake(Entity<ShakeableComponent?> entity, EntityUid? user = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ShakeableComponent>(Entity<ShakeableComponent>.op_Implicit(entity), ref entity.Comp, false))
		{
			return false;
		}
		if (user.HasValue && entity.Comp.RequireInHand && !_hands.IsHolding(Entity<HandsComponent>.op_Implicit(user.Value), Entity<ShakeableComponent>.op_Implicit(entity), out string _))
		{
			return false;
		}
		AttemptShakeEvent attemptEv = new AttemptShakeEvent();
		((EntitySystem)this).RaiseLocalEvent<AttemptShakeEvent>(Entity<ShakeableComponent>.op_Implicit(entity), ref attemptEv, false);
		if (attemptEv.Cancelled)
		{
			return false;
		}
		return true;
	}
}
