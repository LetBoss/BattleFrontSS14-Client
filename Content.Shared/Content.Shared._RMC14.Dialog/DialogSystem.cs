using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Dialog;

public sealed class DialogSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	public override void Initialize()
	{
		BoundUserInterfaceRegisterExt.BuiEvents<DialogComponent>(((EntitySystem)this).Subs, (object)DialogUiKey.Key, (BuiEventSubscriber<DialogComponent>)delegate(Subscriber<DialogComponent> subs)
		{
			subs.Event<DialogOptionBuiMsg>((EntityEventRefHandler<DialogComponent, DialogOptionBuiMsg>)OnDialogOption);
			subs.Event<DialogInputBuiMsg>((EntityEventRefHandler<DialogComponent, DialogInputBuiMsg>)OnDialogInput);
			subs.Event<DialogConfirmBuiMsg>((EntityEventRefHandler<DialogComponent, DialogConfirmBuiMsg>)OnDialogConfirm);
			subs.Event<BoundUIClosedEvent>((EntityEventRefHandler<DialogComponent, BoundUIClosedEvent>)OnDialogClosed);
		});
	}

	private void OnDialogOption(Entity<DialogComponent> ent, ref DialogOptionBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)DialogUiKey.Key);
		int index = args.Index;
		DialogOption option = default(DialogOption);
		if (index >= 0 && Extensions.TryGetValue<DialogOption>((IList<DialogOption>)ent.Comp.Options, index, ref option))
		{
			DialogChosenEvent ev = new DialogChosenEvent(((BaseBoundUserInterfaceEvent)args).Actor, index);
			((EntitySystem)this).RaiseLocalEvent<DialogChosenEvent>(Entity<DialogComponent>.op_Implicit(ent), ref ev, false);
			if (option.Event != null)
			{
				((EntitySystem)this).RaiseLocalEvent(Entity<DialogComponent>.op_Implicit(ent), ref option.Event, true);
			}
		}
	}

	private void OnDialogInput(Entity<DialogComponent> ent, ref DialogInputBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)DialogUiKey.Key);
		if (!(ent.Comp.InputEvent == null))
		{
			string msg = args.Input;
			if (msg.Length > ent.Comp.CharacterLimit)
			{
				msg = msg.Substring(0, ent.Comp.CharacterLimit);
			}
			ent.Comp.InputEvent = ent.Comp.InputEvent with
			{
				Message = msg
			};
			((EntitySystem)this).RaiseLocalEvent(Entity<DialogComponent>.op_Implicit(ent), (object)ent.Comp.InputEvent, false);
		}
	}

	private void OnDialogConfirm(Entity<DialogComponent> ent, ref DialogConfirmBuiMsg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(ent.Owner), (Enum)DialogUiKey.Key);
		if (ent.Comp.ConfirmEvent != null)
		{
			((EntitySystem)this).RaiseLocalEvent(Entity<DialogComponent>.op_Implicit(ent), ent.Comp.ConfirmEvent, false);
		}
	}

	private void OnDialogClosed(Entity<DialogComponent> ent, ref BoundUIClosedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			((EntitySystem)this).RemComp<DialogComponent>(Entity<DialogComponent>.op_Implicit(ent));
		}
	}

	public void OpenOptions(EntityUid target, EntityUid actor, string title, List<DialogOption> options, string message = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		DialogComponent dialog = ((EntitySystem)this).EnsureComp<DialogComponent>(target);
		dialog.Title = title;
		dialog.Message = new DialogOption(message);
		dialog.DialogType = DialogType.Options;
		dialog.Options = options;
		((EntitySystem)this).Dirty(target, (IComponent)(object)dialog, (MetaDataComponent)null);
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(target), (Enum)DialogUiKey.Key, actor, false);
	}

	public void OpenOptions(EntityUid actor, string title, List<DialogOption> options, string message = "")
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		OpenOptions(actor, actor, title, options, message);
	}

	public void OpenInput(EntityUid target, EntityUid actor, string message, DialogInputEvent? ev, bool largeInput = false, int characterLimit = 200, bool autoFocus = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		DialogComponent dialog = ((EntitySystem)this).EnsureComp<DialogComponent>(target);
		dialog.DialogType = DialogType.Input;
		dialog.Message = new DialogOption(message, ev);
		dialog.InputEvent = ev;
		dialog.LargeInput = largeInput;
		dialog.CharacterLimit = characterLimit;
		dialog.AutoFocus = autoFocus;
		((EntitySystem)this).Dirty(target, (IComponent)(object)dialog, (MetaDataComponent)null);
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(target), (Enum)DialogUiKey.Key, actor, false);
	}

	public void OpenInput(EntityUid actor, string message, DialogInputEvent? ev, bool largeInput = false, int characterLimit = 200, bool autoFocus = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		OpenInput(actor, actor, message, ev, largeInput, characterLimit, autoFocus);
	}

	public void OpenConfirmation(EntityUid target, EntityUid actor, string title, string message, object ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		DialogComponent dialog = ((EntitySystem)this).EnsureComp<DialogComponent>(target);
		dialog.DialogType = DialogType.Confirm;
		dialog.Title = title;
		dialog.Message = new DialogOption(message, ev);
		dialog.ConfirmEvent = ev;
		((EntitySystem)this).Dirty(target, (IComponent)(object)dialog, (MetaDataComponent)null);
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(target), (Enum)DialogUiKey.Key, actor, false);
	}

	public void OpenConfirmation(EntityUid actor, string title, string message, object ev)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		OpenConfirmation(actor, actor, title, message, ev);
	}
}
