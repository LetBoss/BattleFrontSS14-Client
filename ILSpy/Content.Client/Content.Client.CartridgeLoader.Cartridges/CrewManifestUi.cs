using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class CrewManifestUi : UIFragment, ISerializationGenerated<CrewManifestUi>, ISerializationGenerated
{
	private CrewManifestUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		_fragment = new CrewManifestUiFragment();
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is CrewManifestUiState crewManifestUiState)
		{
			_fragment?.UpdateState(crewManifestUiState.StationName, crewManifestUiState.Entries);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CrewManifestUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (CrewManifestUi)target2;
		serialization.TryCustomCopy<CrewManifestUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CrewManifestUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CrewManifestUi target2 = (CrewManifestUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CrewManifestUi target2 = (CrewManifestUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CrewManifestUi Instantiate()
	{
		return new CrewManifestUi();
	}
}
