using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ShakeableComponent : Component, ISerializationGenerated<ShakeableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan ShakeDuration = TimeSpan.FromSeconds(1.0);

	[DataField(null, false, 1, false, false, null)]
	public bool RequireInHand;

	[DataField(null, false, 1, false, false, null)]
	public LocId ShakeVerbText = LocId.op_Implicit("shakeable-verb");

	[DataField(null, false, 1, false, false, null)]
	public LocId ShakePopupMessageSelf = LocId.op_Implicit("shakeable-popup-message-self");

	[DataField(null, false, 1, false, false, null)]
	public LocId ShakePopupMessageOthers = LocId.op_Implicit("shakeable-popup-message-others");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier ShakeSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Items/soda_shake.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ShakeableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ShakeableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ShakeableComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan ShakeDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(ShakeDuration, ref ShakeDurationTemp, hookCtx, false, context))
			{
				ShakeDurationTemp = serialization.CreateCopy<TimeSpan>(ShakeDuration, hookCtx, context, false);
			}
			target.ShakeDuration = ShakeDurationTemp;
			bool RequireInHandTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequireInHand, ref RequireInHandTemp, hookCtx, false, context))
			{
				RequireInHandTemp = RequireInHand;
			}
			target.RequireInHand = RequireInHandTemp;
			LocId ShakeVerbTextTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ShakeVerbText, ref ShakeVerbTextTemp, hookCtx, false, context))
			{
				ShakeVerbTextTemp = serialization.CreateCopy<LocId>(ShakeVerbText, hookCtx, context, false);
			}
			target.ShakeVerbText = ShakeVerbTextTemp;
			LocId ShakePopupMessageSelfTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ShakePopupMessageSelf, ref ShakePopupMessageSelfTemp, hookCtx, false, context))
			{
				ShakePopupMessageSelfTemp = serialization.CreateCopy<LocId>(ShakePopupMessageSelf, hookCtx, context, false);
			}
			target.ShakePopupMessageSelf = ShakePopupMessageSelfTemp;
			LocId ShakePopupMessageOthersTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ShakePopupMessageOthers, ref ShakePopupMessageOthersTemp, hookCtx, false, context))
			{
				ShakePopupMessageOthersTemp = serialization.CreateCopy<LocId>(ShakePopupMessageOthers, hookCtx, context, false);
			}
			target.ShakePopupMessageOthers = ShakePopupMessageOthersTemp;
			SoundSpecifier ShakeSoundTemp = null;
			if (ShakeSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(ShakeSound, ref ShakeSoundTemp, hookCtx, true, context))
			{
				ShakeSoundTemp = serialization.CreateCopy<SoundSpecifier>(ShakeSound, hookCtx, context, false);
			}
			target.ShakeSound = ShakeSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ShakeableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShakeableComponent cast = (ShakeableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShakeableComponent cast = (ShakeableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShakeableComponent def = (ShakeableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ShakeableComponent Instantiate()
	{
		return new ShakeableComponent();
	}
}
