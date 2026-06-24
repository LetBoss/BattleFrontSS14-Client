using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Rend;

public sealed class XenoRendSystem : EntitySystem
{
	[Dependency]
	private SharedRMCActionsSystem _actions;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedInteractionSystem _interact;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoRendComponent, XenoRendActionEvent>((EntityEventRefHandler<XenoRendComponent, XenoRendActionEvent>)OnXenoRendAction, (Type[])null, (Type[])null);
	}

	private void OnXenoRendAction(Entity<XenoRendComponent> xeno, ref XenoRendActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_actions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoRendComponent>.op_Implicit(xeno));
		_emote.TryEmoteWithChat(Entity<XenoRendComponent>.op_Implicit(xeno), xeno.Comp.HissEmote);
		foreach (Entity<MobStateComponent> ent in _entityLookup.GetEntitiesInRange<MobStateComponent>(_transform.GetMapCoordinates(Entity<XenoRendComponent>.op_Implicit(xeno), (TransformComponent)null), xeno.Comp.Range, (LookupFlags)110))
		{
			if (!_xeno.CanAbilityAttackTarget(Entity<XenoRendComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(ent)) || !_interact.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(ent.Owner), xeno.Comp.Range))
			{
				continue;
			}
			if (_damage.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(ent), xeno.Comp.Damage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoRendComponent>.op_Implicit(xeno), Entity<XenoRendComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(ent), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(ent) }, filter);
			}
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), ent.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
			_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoRendComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoRendComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
	}
}
