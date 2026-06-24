using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Interaction.Events;
using Content.Shared.Magic.Components;
using Content.Shared.Mind;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared.Magic;

public sealed class SpellbookSystem : EntitySystem
{
	[Dependency]
	private SharedChargesSystem _sharedCharges;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	[Dependency]
	private INetManager _netManager;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<SpellbookComponent, MapInitEvent>((EntityEventRefHandler<SpellbookComponent, MapInitEvent>)OnInit, new Type[1] { typeof(SharedMagicSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpellbookComponent, UseInHandEvent>((EntityEventRefHandler<SpellbookComponent, UseInHandEvent>)OnUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<SpellbookComponent, SpellbookDoAfterEvent>((EntityEventRefHandler<SpellbookComponent, SpellbookDoAfterEvent>)OnDoAfter, (Type[])null, (Type[])null);
	}

	private void OnInit(Entity<SpellbookComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<EntProtoId, int?> spellAction in ent.Comp.SpellActions)
		{
			spellAction.Deconstruct(out var key, out var value);
			EntProtoId id = key;
			int? charges = value;
			EntityUid? spell = _actionContainer.AddAction(Entity<SpellbookComponent>.op_Implicit(ent), EntProtoId.op_Implicit(id));
			if (spell.HasValue)
			{
				if (charges.HasValue)
				{
					int count = charges.GetValueOrDefault();
					_sharedCharges.SetCharges(Entity<LimitedChargesComponent>.op_Implicit(spell.Value), count);
				}
				ent.Comp.Spells.Add(spell.Value);
			}
		}
	}

	private void OnUse(Entity<SpellbookComponent> ent, ref UseInHandEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			AttemptLearn(ent, args);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnDoAfter<T>(Entity<SpellbookComponent> ent, ref T args) where T : DoAfterEvent
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!ent.Comp.LearnPermanently)
		{
			_actions.GrantActions(Entity<ActionsComponent>.op_Implicit(args.Args.User), ent.Comp.Spells, Entity<ActionsContainerComponent>.op_Implicit(ent.Owner));
			return;
		}
		if (_mind.TryGetMind(args.Args.User, out EntityUid mindId, out MindComponent _))
		{
			ActionsContainerComponent mindActionContainerComp = ((EntitySystem)this).EnsureComp<ActionsContainerComponent>(mindId);
			if (_netManager.IsServer)
			{
				_actionContainer.TransferAllActionsWithNewAttached(Entity<SpellbookComponent>.op_Implicit(ent), mindId, args.Args.User, null, mindActionContainerComp);
			}
		}
		else
		{
			foreach (KeyValuePair<EntProtoId, int?> spellAction in ent.Comp.SpellActions)
			{
				spellAction.Deconstruct(out var key, out var value);
				EntProtoId id = key;
				int? charges = value;
				EntityUid? actionId = null;
				if (_actions.AddAction(args.Args.User, ref actionId, EntProtoId.op_Implicit(id)) && charges.HasValue)
				{
					int count = charges.GetValueOrDefault();
					_sharedCharges.SetCharges(Entity<LimitedChargesComponent>.op_Implicit(actionId.Value), count);
				}
			}
		}
		ent.Comp.SpellActions.Clear();
	}

	private void AttemptLearn(Entity<SpellbookComponent> ent, UseInHandEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.User, ent.Comp.LearnTime, new SpellbookDoAfterEvent(), Entity<SpellbookComponent>.op_Implicit(ent), Entity<SpellbookComponent>.op_Implicit(ent))
		{
			BreakOnMove = true,
			BreakOnDamage = true,
			NeedHand = true
		};
		_doAfter.TryStartDoAfter(doAfterEventArgs);
	}
}
