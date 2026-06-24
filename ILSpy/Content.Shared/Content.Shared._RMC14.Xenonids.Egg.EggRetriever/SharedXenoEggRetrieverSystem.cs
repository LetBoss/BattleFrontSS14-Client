using System;
using Content.Shared._RMC14.Actions;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Egg.EggRetriever;

public abstract class SharedXenoEggRetrieverSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	protected SharedAppearanceSystem _appearance;

	[Dependency]
	private INetManager _net;

	[Dependency]
	protected SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoEggRetrieverComponent, ExaminedEvent>((EntityEventRefHandler<XenoEggRetrieverComponent, ExaminedEvent>)OnEggRetrieverExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoGenerateEggsComponent, XenoGenerateEggsActionEvent>((EntityEventRefHandler<XenoGenerateEggsComponent, XenoGenerateEggsActionEvent>)OnXenoProduceEggsAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoGenerateEggsComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoGenerateEggsComponent, MobStateChangedEvent>)OnXenoProduceEggsDeath, (Type[])null, (Type[])null);
	}

	private void OnEggRetrieverExamine(Entity<XenoEggRetrieverComponent> retriever, ref ExaminedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(args.Examiner))
		{
			return;
		}
		using (args.PushGroup("XenoEggRetrieverComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-xeno-retrieve-egg-current", new(string, object)[3]
			{
				("xeno", retriever),
				("cur_eggs", retriever.Comp.CurEggs),
				("max_eggs", retriever.Comp.MaxEggs)
			}));
		}
	}

	private void OnXenoProduceEggsAction(Entity<XenoGenerateEggsComponent> xeno, ref XenoGenerateEggsActionEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && _rmcActions.TryUseAction(args))
		{
			((HandledEntityEventArgs)args).Handled = true;
			ToggleProduceEggs(Entity<XenoGenerateEggsComponent>.op_Implicit(xeno), xeno.Comp);
			if (xeno.Comp.Active)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-xeno-produce-eggs-start"), Entity<XenoGenerateEggsComponent>.op_Implicit(xeno), Entity<XenoGenerateEggsComponent>.op_Implicit(xeno));
			}
		}
	}

	protected void ToggleProduceEggs(EntityUid xeno, XenoGenerateEggsComponent produce)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (produce.Active && _net.IsServer)
		{
			produce.NextDrain = null;
			produce.NextEgg = null;
		}
		produce.Active = !produce.Active;
		_appearance.SetData(xeno, (Enum)XenoEggStorageVisuals.Active, (object)produce.Active, (AppearanceComponent)null);
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoGenerateEggsActionEvent>(xeno))
		{
			_actions.SetToggled(action.AsNullable(), produce.Active);
		}
		((EntitySystem)this).Dirty(xeno, (IComponent)(object)produce, (MetaDataComponent)null);
	}

	private void OnXenoProduceEggsDeath(Entity<XenoGenerateEggsComponent> xeno, ref MobStateChangedEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && args.NewMobState == MobState.Dead && xeno.Comp.Active)
		{
			ToggleProduceEggs(Entity<XenoGenerateEggsComponent>.op_Implicit(xeno), xeno.Comp);
		}
	}
}
