using System;
using Content.Shared._RMC14.Sound;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Timing;
using Content.Shared.Whistle;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Whistle;

public sealed class RMCWhistleSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private WhistleSystem _whistle;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCWhistleComponent, UseInHandEvent>((EntityEventRefHandler<RMCWhistleComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWhistleComponent, GetItemActionsEvent>((EntityEventRefHandler<RMCWhistleComponent, GetItemActionsEvent>)OnGetItemActions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWhistleComponent, SoundActionEvent>((EntityEventRefHandler<RMCWhistleComponent, SoundActionEvent>)OnWhistleAction, (Type[])null, (Type[])null);
	}

	private void OnGetItemActions(Entity<RMCWhistleComponent> ent, ref GetItemActionsEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (args.SlotFlags != SlotFlags.POCKET)
		{
			args.AddAction(ref ent.Comp.Action, EntProtoId.op_Implicit(ent.Comp.ActionId));
		}
	}

	public void OnWhistleAction(Entity<RMCWhistleComponent> ent, ref SoundActionEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.IsFirstTimePredicted && !((HandledEntityEventArgs)args).Handled)
		{
			TryWhistle(ent, args.Performer);
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	public void OnUseInHand(Entity<RMCWhistleComponent> ent, ref UseInHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		TryWhistle(ent, args.User);
		((HandledEntityEventArgs)args).Handled = true;
	}

	public void TryWhistle(Entity<RMCWhistleComponent> ent, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		_whistle.TryMakeLoudWhistle(Entity<RMCWhistleComponent>.op_Implicit(ent), user);
		UseDelayComponent useDelay = default(UseDelayComponent);
		if (((EntitySystem)this).TryComp<UseDelayComponent>(Entity<RMCWhistleComponent>.op_Implicit(ent), ref useDelay))
		{
			SharedActionsSystem actions = _actions;
			EntityUid? action = ent.Comp.Action;
			actions.SetCooldown(action.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(action.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), useDelay.Delay);
			_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit(ent.Owner), useDelay.Delay);
			_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((ent.Owner, useDelay)));
		}
	}
}
