using System;
using System.Collections;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Instruments;

[NetworkedComponent]
[Access(new Type[] { typeof(SharedInstrumentSystem) })]
public abstract class SharedInstrumentComponent : Component, ISerializationGenerated<SharedInstrumentComponent>, ISerializationGenerated
{
	[ViewVariables]
	public bool Playing { get; set; }

	[DataField("program", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public byte InstrumentProgram { get; set; }

	[DataField("bank", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public byte InstrumentBank { get; set; }

	[DataField("allowPercussion", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool AllowPercussion { get; set; }

	[DataField("allowProgramChange", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool AllowProgramChange { get; set; }

	[DataField("respectMidiLimits", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool RespectMidiLimits { get; set; } = true;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public EntityUid? Master { get; set; }

	[ViewVariables]
	public BitArray FilteredChannels { get; set; } = new BitArray(16, defaultValue: true);

	public SharedInstrumentComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedInstrumentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedInstrumentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedInstrumentComponent>(this, ref target, hookCtx, false, context))
		{
			byte InstrumentProgramTemp = 0;
			if (!serialization.TryCustomCopy<byte>(InstrumentProgram, ref InstrumentProgramTemp, hookCtx, false, context))
			{
				InstrumentProgramTemp = InstrumentProgram;
			}
			target.InstrumentProgram = InstrumentProgramTemp;
			byte InstrumentBankTemp = 0;
			if (!serialization.TryCustomCopy<byte>(InstrumentBank, ref InstrumentBankTemp, hookCtx, false, context))
			{
				InstrumentBankTemp = InstrumentBank;
			}
			target.InstrumentBank = InstrumentBankTemp;
			bool AllowPercussionTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowPercussion, ref AllowPercussionTemp, hookCtx, false, context))
			{
				AllowPercussionTemp = AllowPercussion;
			}
			target.AllowPercussion = AllowPercussionTemp;
			bool AllowProgramChangeTemp = false;
			if (!serialization.TryCustomCopy<bool>(AllowProgramChange, ref AllowProgramChangeTemp, hookCtx, false, context))
			{
				AllowProgramChangeTemp = AllowProgramChange;
			}
			target.AllowProgramChange = AllowProgramChangeTemp;
			bool RespectMidiLimitsTemp = false;
			if (!serialization.TryCustomCopy<bool>(RespectMidiLimits, ref RespectMidiLimitsTemp, hookCtx, false, context))
			{
				RespectMidiLimitsTemp = RespectMidiLimits;
			}
			target.RespectMidiLimits = RespectMidiLimitsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedInstrumentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedInstrumentComponent cast = (SharedInstrumentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedInstrumentComponent cast = (SharedInstrumentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedInstrumentComponent def = (SharedInstrumentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedInstrumentComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
