using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedKitchenSpikeSystem) })]
public sealed class KitchenSpikeComponent : Component, ISerializationGenerated<KitchenSpikeComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public enum KitchenSpikeVisuals : byte
	{
		Status
	}

	[Serializable]
	[NetSerializable]
	public enum KitchenSpikeStatus : byte
	{
		Empty,
		Bloody
	}

	[DataField("delay", false, 1, false, false, null)]
	public float SpikeDelay = 7f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sound", false, 1, false, false, null)]
	public SoundSpecifier SpikeSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg", (AudioParams?)null);

	public List<string>? PrototypesToSpawn;

	public string MeatSource1p = "?";

	public string MeatSource0 = "?";

	public string Victim = "?";

	public bool InUse;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref KitchenSpikeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (KitchenSpikeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<KitchenSpikeComponent>(this, ref target, hookCtx, false, context))
		{
			float SpikeDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpikeDelay, ref SpikeDelayTemp, hookCtx, false, context))
			{
				SpikeDelayTemp = SpikeDelay;
			}
			target.SpikeDelay = SpikeDelayTemp;
			SoundSpecifier SpikeSoundTemp = null;
			if (SpikeSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(SpikeSound, ref SpikeSoundTemp, hookCtx, true, context))
			{
				SpikeSoundTemp = serialization.CreateCopy<SoundSpecifier>(SpikeSound, hookCtx, context, false);
			}
			target.SpikeSound = SpikeSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref KitchenSpikeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KitchenSpikeComponent cast = (KitchenSpikeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KitchenSpikeComponent cast = (KitchenSpikeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		KitchenSpikeComponent def = (KitchenSpikeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override KitchenSpikeComponent Instantiate()
	{
		return new KitchenSpikeComponent();
	}
}
