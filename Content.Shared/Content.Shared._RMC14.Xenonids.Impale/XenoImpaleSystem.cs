using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Finesse;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Impale;

public sealed class XenoImpaleSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedColorFlashEffectSystem _flash;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private XenoSystem _xeno;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoImpaleComponent, XenoImpaleActionEvent>((EntityEventRefHandler<XenoImpaleComponent, XenoImpaleActionEvent>)OnXenoImpaleAction, (Type[])null, (Type[])null);
	}

	private void OnXenoImpaleAction(Entity<XenoImpaleComponent> xeno, ref XenoImpaleActionEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (((EntitySystem)this).HasComp<XenoMarkedComponent>(args.Target))
		{
			ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
			if (emote.HasValue)
			{
				ProtoId<EmotePrototype> emote2 = emote.GetValueOrDefault();
				_emote.TryEmoteWithChat(Entity<XenoImpaleComponent>.op_Implicit(xeno), emote2, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, xeno.Comp.EmoteCooldown);
			}
			XenoSecondImpaleComponent xenoSecondImpaleComponent = ((EntitySystem)this).EnsureComp<XenoSecondImpaleComponent>(args.Target);
			xenoSecondImpaleComponent.Damage = xeno.Comp.Damage;
			xenoSecondImpaleComponent.ImpaleAt = _timing.CurTime + xeno.Comp.SecondImpaleTime;
			xenoSecondImpaleComponent.Origin = Entity<XenoImpaleComponent>.op_Implicit(xeno);
			((EntitySystem)this).RemCompDeferred<XenoMarkedComponent>(args.Target);
		}
		Impale(xeno.Comp.Damage, xeno.Comp.AP, xeno.Comp.Animation, xeno.Comp.Sound, args.Target, Entity<XenoImpaleComponent>.op_Implicit(xeno));
	}

	private void Impale(DamageSpecifier damage, int aP, EntProtoId animation, SoundSpecifier sound, EntityUid target, EntityUid xeno)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (_damage.TryChangeDamage(target, _xeno.TryApplyXenoSlashDamageMultiplier(target, damage), ignoreResistances: false, interruptsDoAfters: true, null, xeno, xeno, aP)?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno));
			_flash.RaiseEffect(Color.Red, new List<EntityUid> { target }, filter);
		}
		_rmcMelee.DoLunge(xeno, target);
		if (!_net.IsClient)
		{
			_audio.PlayPvs(sound, xeno, (AudioParams?)null);
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(animation), target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoSecondImpaleComponent> impaleQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoSecondImpaleComponent>();
		EntityUid uid = default(EntityUid);
		XenoSecondImpaleComponent impale = default(XenoSecondImpaleComponent);
		while (impaleQuery.MoveNext(ref uid, ref impale))
		{
			if (!(impale.ImpaleAt > time))
			{
				Impale(impale.Damage, impale.AP, impale.Animation, impale.Sound, uid, impale.Origin);
				((EntitySystem)this).RemCompDeferred<XenoSecondImpaleComponent>(uid);
			}
		}
	}
}
