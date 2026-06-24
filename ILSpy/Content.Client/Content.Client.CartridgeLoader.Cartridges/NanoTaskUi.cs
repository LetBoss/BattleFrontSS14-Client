using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NanoTaskUi : UIFragment, ISerializationGenerated<NanoTaskUi>, ISerializationGenerated
{
	private NanoTaskUiFragment? _fragment;

	private NanoTaskItemPopup? _popup;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		_fragment = new NanoTaskUiFragment();
		_popup = new NanoTaskItemPopup();
		NanoTaskUiFragment? fragment = _fragment;
		fragment.NewTask = (Action)Delegate.Combine(fragment.NewTask, (Action)delegate
		{
			_popup.ResetInputs(null);
			_popup.SetEditingTaskId(null);
			((BaseWindow)_popup).OpenCentered();
		});
		NanoTaskUiFragment? fragment2 = _fragment;
		fragment2.OpenTask = (Action<int>)Delegate.Combine(fragment2.OpenTask, (Action<int>)delegate(int id)
		{
			NanoTaskItemAndId nanoTaskItemAndId = _fragment.Tasks.Find((NanoTaskItemAndId task) => task.Id == id);
			if (nanoTaskItemAndId != null)
			{
				_popup.ResetInputs(nanoTaskItemAndId.Data);
				_popup.SetEditingTaskId(nanoTaskItemAndId.Id);
				((BaseWindow)_popup).OpenCentered();
			}
		});
		NanoTaskUiFragment? fragment3 = _fragment;
		fragment3.ToggleTaskCompletion = (Action<int>)Delegate.Combine(fragment3.ToggleTaskCompletion, (Action<int>)delegate(int id)
		{
			NanoTaskItemAndId nanoTaskItemAndId = _fragment.Tasks.Find((NanoTaskItemAndId task) => task.Id == id);
			if (nanoTaskItemAndId != null)
			{
				userInterface.SendMessage((BoundUserInterfaceMessage)(object)new CartridgeUiMessage(new NanoTaskUiMessageEvent(new NanoTaskUpdateTask(new NanoTaskItemAndId(id, new NanoTaskItem(nanoTaskItemAndId.Data.Description, nanoTaskItemAndId.Data.TaskIsFor, !nanoTaskItemAndId.Data.IsTaskDone, nanoTaskItemAndId.Data.Priority))))));
			}
		});
		NanoTaskItemPopup? popup = _popup;
		popup.TaskSaved = (Action<int, NanoTaskItem>)Delegate.Combine(popup.TaskSaved, (Action<int, NanoTaskItem>)delegate(int id, NanoTaskItem data)
		{
			userInterface.SendMessage((BoundUserInterfaceMessage)(object)new CartridgeUiMessage(new NanoTaskUiMessageEvent(new NanoTaskUpdateTask(new NanoTaskItemAndId(id, data)))));
			((BaseWindow)_popup).Close();
		});
		NanoTaskItemPopup? popup2 = _popup;
		popup2.TaskDeleted = (Action<int>)Delegate.Combine(popup2.TaskDeleted, (Action<int>)delegate(int id)
		{
			userInterface.SendMessage((BoundUserInterfaceMessage)(object)new CartridgeUiMessage(new NanoTaskUiMessageEvent(new NanoTaskDeleteTask(id))));
			((BaseWindow)_popup).Close();
		});
		NanoTaskItemPopup? popup3 = _popup;
		popup3.TaskCreated = (Action<NanoTaskItem>)Delegate.Combine(popup3.TaskCreated, (Action<NanoTaskItem>)delegate(NanoTaskItem data)
		{
			userInterface.SendMessage((BoundUserInterfaceMessage)(object)new CartridgeUiMessage(new NanoTaskUiMessageEvent(new NanoTaskAddTask(data))));
			((BaseWindow)_popup).Close();
		});
		NanoTaskItemPopup? popup4 = _popup;
		popup4.TaskPrinted = (Action<NanoTaskItem>)Delegate.Combine(popup4.TaskPrinted, (Action<NanoTaskItem>)delegate(NanoTaskItem data)
		{
			userInterface.SendMessage((BoundUserInterfaceMessage)(object)new CartridgeUiMessage(new NanoTaskUiMessageEvent(new NanoTaskPrintTask(data))));
		});
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is NanoTaskUiState nanoTaskUiState)
		{
			_fragment?.UpdateState(nanoTaskUiState.Tasks);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NanoTaskUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (NanoTaskUi)target2;
		serialization.TryCustomCopy<NanoTaskUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NanoTaskUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NanoTaskUi target2 = (NanoTaskUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NanoTaskUi target2 = (NanoTaskUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NanoTaskUi Instantiate()
	{
		return new NanoTaskUi();
	}
}
