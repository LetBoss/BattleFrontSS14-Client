using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Mech.Ui.Equipment;

public sealed class MechSoundboardUi : UIFragment, ISerializationGenerated<MechSoundboardUi>, ISerializationGenerated
{
	private MechSoundboardUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		if (fragmentOwner.HasValue)
		{
			_fragment = new MechSoundboardUiFragment();
			_fragment.OnPlayAction += delegate(int sound)
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				userInterface.SendMessage((BoundUserInterfaceMessage)(object)new MechSoundboardPlayMessage(IoCManager.Resolve<IEntityManager>().GetNetEntity(fragmentOwner.Value, (MetaDataComponent)null), sound));
			};
		}
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is MechSoundboardUiState state2)
		{
			_fragment?.UpdateContents(state2);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MechSoundboardUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (MechSoundboardUi)target2;
		serialization.TryCustomCopy<MechSoundboardUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MechSoundboardUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechSoundboardUi target2 = (MechSoundboardUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechSoundboardUi target2 = (MechSoundboardUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MechSoundboardUi Instantiate()
	{
		return new MechSoundboardUi();
	}
}
