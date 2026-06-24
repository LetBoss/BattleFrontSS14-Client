using System;
using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Client.Mech.Ui.Equipment;

public sealed class MechGrabberUi : UIFragment, ISerializationGenerated<MechGrabberUi>, ISerializationGenerated
{
	private MechGrabberUiFragment? _fragment;

	public override Control GetUIFragmentRoot()
	{
		return (Control)(object)_fragment;
	}

	public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
	{
		if (fragmentOwner.HasValue)
		{
			_fragment = new MechGrabberUiFragment();
			_fragment.OnEjectAction += delegate(EntityUid e)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				IEntityManager val = IoCManager.Resolve<IEntityManager>();
				userInterface.SendMessage((BoundUserInterfaceMessage)(object)new MechGrabberEjectMessage(val.GetNetEntity(fragmentOwner.Value, (MetaDataComponent)null), val.GetNetEntity(e, (MetaDataComponent)null)));
			};
		}
	}

	public override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is MechGrabberUiState state2)
		{
			_fragment?.UpdateContents(state2);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MechGrabberUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		UIFragment target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (MechGrabberUi)target2;
		serialization.TryCustomCopy<MechGrabberUi>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MechGrabberUi target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref UIFragment target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechGrabberUi target2 = (MechGrabberUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MechGrabberUi target2 = (MechGrabberUi)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MechGrabberUi Instantiate()
	{
		return new MechGrabberUi();
	}
}
