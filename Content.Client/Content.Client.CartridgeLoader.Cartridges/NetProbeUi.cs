using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NetProbeUi : UIFragment, ISerializationGenerated<NetProbeUi>, ISerializationGenerated
{
	private NetProbeUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		_fragment = new NetProbeUiFragment();
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is NetProbeUiState netProbeUiState)
		{
			_fragment?.UpdateState(netProbeUiState.ProbedDevices);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NetProbeUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (NetProbeUi)target2;
		serialization.TryCustomCopy<NetProbeUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NetProbeUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NetProbeUi target2 = (NetProbeUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NetProbeUi target2 = (NetProbeUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NetProbeUi Instantiate()
	{
		return new NetProbeUi();
	}
}
