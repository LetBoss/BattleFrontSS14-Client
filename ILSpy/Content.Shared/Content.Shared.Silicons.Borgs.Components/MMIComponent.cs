using System;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedBorgSystem) })]
public sealed class MMIComponent : Component, ISerializationGenerated<MMIComponent>, ISerializationGenerated
{
	[DataField("brainSlotId", false, 1, false, false, null)]
	public string BrainSlotId = "brain_slot";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ItemSlot BrainSlot;

	[DataField("hasMindState", false, 1, false, false, null)]
	public string HasMindState = "mmi_alive";

	[DataField("noMindState", false, 1, false, false, null)]
	public string NoMindState = "mmi_dead";

	[DataField("noBrainState", false, 1, false, false, null)]
	public string NoBrainState = "mmi_off";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MMIComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MMIComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MMIComponent>(this, ref target, hookCtx, false, context))
		{
			string BrainSlotIdTemp = null;
			if (BrainSlotId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BrainSlotId, ref BrainSlotIdTemp, hookCtx, false, context))
			{
				BrainSlotIdTemp = BrainSlotId;
			}
			target.BrainSlotId = BrainSlotIdTemp;
			string HasMindStateTemp = null;
			if (HasMindState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(HasMindState, ref HasMindStateTemp, hookCtx, false, context))
			{
				HasMindStateTemp = HasMindState;
			}
			target.HasMindState = HasMindStateTemp;
			string NoMindStateTemp = null;
			if (NoMindState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(NoMindState, ref NoMindStateTemp, hookCtx, false, context))
			{
				NoMindStateTemp = NoMindState;
			}
			target.NoMindState = NoMindStateTemp;
			string NoBrainStateTemp = null;
			if (NoBrainState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(NoBrainState, ref NoBrainStateTemp, hookCtx, false, context))
			{
				NoBrainStateTemp = NoBrainState;
			}
			target.NoBrainState = NoBrainStateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MMIComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MMIComponent cast = (MMIComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MMIComponent cast = (MMIComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MMIComponent def = (MMIComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MMIComponent Instantiate()
	{
		return new MMIComponent();
	}
}
