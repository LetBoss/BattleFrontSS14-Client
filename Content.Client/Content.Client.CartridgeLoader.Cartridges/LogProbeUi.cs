using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class LogProbeUi : UIFragment, ISerializationGenerated<LogProbeUi>, ISerializationGenerated
{
	private LogProbeUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface ui, EntityUid? fragmentOwner)
	{
		_fragment = new LogProbeUiFragment();
		LogProbeUiFragment? fragment = _fragment;
		fragment.OnPrintPressed = (Action)Delegate.Combine(fragment.OnPrintPressed, (Action)delegate
		{
			CartridgeUiMessage cartridgeUiMessage = new CartridgeUiMessage(new LogProbePrintMessage());
			ui.SendMessage((BoundUserInterfaceMessage)(object)cartridgeUiMessage);
		});
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is LogProbeUiState logProbeUiState)
		{
			_fragment?.UpdateState(logProbeUiState.EntityName, logProbeUiState.PulledLogs);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LogProbeUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (LogProbeUi)target2;
		serialization.TryCustomCopy<LogProbeUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LogProbeUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LogProbeUi target2 = (LogProbeUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LogProbeUi target2 = (LogProbeUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LogProbeUi Instantiate()
	{
		return new LogProbeUi();
	}
}
