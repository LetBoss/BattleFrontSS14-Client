using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Access.Components;

[Serializable]
[DataDefinition]
[NetSerializable]
public readonly record struct AccessRecord([property: DataField(null, false, 1, false, false, null), ViewVariables(/*Could not decode attribute arguments.*/)] TimeSpan AccessTime, [property: DataField(null, false, 1, false, false, null), ViewVariables(/*Could not decode attribute arguments.*/)] string Accessor) : ISerializationGenerated<AccessRecord>, ISerializationGenerated
{
	public AccessRecord()
		: this(TimeSpan.Zero, string.Empty)
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AccessRecord target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<AccessRecord>(this, ref target, hookCtx, false, context))
		{
			TimeSpan AccessTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(AccessTime, ref AccessTimeTemp, hookCtx, false, context))
			{
				AccessTimeTemp = serialization.CreateCopy<TimeSpan>(AccessTime, hookCtx, context, false);
			}
			string AccessorTemp = null;
			if (Accessor == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Accessor, ref AccessorTemp, hookCtx, false, context))
			{
				AccessorTemp = Accessor;
			}
			target = target with
			{
				AccessTime = AccessTimeTemp,
				Accessor = AccessorTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AccessRecord target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AccessRecord cast = (AccessRecord)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AccessRecord Instantiate()
	{
		return new AccessRecord();
	}
}
