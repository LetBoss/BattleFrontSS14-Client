using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NotekeeperUi : UIFragment, ISerializationGenerated<NotekeeperUi>, ISerializationGenerated
{
	private NotekeeperUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		_fragment = new NotekeeperUiFragment();
		_fragment.OnNoteRemoved += delegate(string note)
		{
			SendNotekeeperMessage(NotekeeperUiAction.Remove, note, userInterface);
		};
		_fragment.OnNoteAdded += delegate(string note)
		{
			SendNotekeeperMessage(NotekeeperUiAction.Add, note, userInterface);
		};
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is NotekeeperUiState notekeeperUiState)
		{
			_fragment?.UpdateState(notekeeperUiState.Notes);
		}
	}

	private void SendNotekeeperMessage(NotekeeperUiAction action, string note, BoundUserInterface userInterface)
	{
		CartridgeUiMessage cartridgeUiMessage = new CartridgeUiMessage(new NotekeeperUiMessageEvent(action, note));
		userInterface.SendMessage((BoundUserInterfaceMessage)(object)cartridgeUiMessage);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NotekeeperUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (NotekeeperUi)target2;
		serialization.TryCustomCopy<NotekeeperUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NotekeeperUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NotekeeperUi target2 = (NotekeeperUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NotekeeperUi target2 = (NotekeeperUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NotekeeperUi Instantiate()
	{
		return new NotekeeperUi();
	}
}
