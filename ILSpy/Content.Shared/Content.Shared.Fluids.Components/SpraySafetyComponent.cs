using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Fluids.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SpraySafetySystem) })]
public sealed class SpraySafetyComponent : Component, ISerializationGenerated<SpraySafetyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId Popup = LocId.op_Implicit("fire-extinguisher-component-safety-on-message");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier RefillSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/refill.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpraySafetyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpraySafetyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpraySafetyComponent>(this, ref target, hookCtx, false, context))
		{
			LocId PopupTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Popup, ref PopupTemp, hookCtx, false, context))
			{
				PopupTemp = serialization.CreateCopy<LocId>(Popup, hookCtx, context, false);
			}
			target.Popup = PopupTemp;
			SoundSpecifier RefillSoundTemp = null;
			if (RefillSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(RefillSound, ref RefillSoundTemp, hookCtx, true, context))
			{
				RefillSoundTemp = serialization.CreateCopy<SoundSpecifier>(RefillSound, hookCtx, context, false);
			}
			target.RefillSound = RefillSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpraySafetyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpraySafetyComponent cast = (SpraySafetyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpraySafetyComponent cast = (SpraySafetyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpraySafetyComponent def = (SpraySafetyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpraySafetyComponent Instantiate()
	{
		return new SpraySafetyComponent();
	}
}
