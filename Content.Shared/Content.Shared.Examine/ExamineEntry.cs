using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared.Examine;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class ExamineEntry : ISerializationGenerated<ExamineEntry>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Component;

	[DataField(null, false, 1, false, false, null)]
	public float Priority;

	[DataField(null, false, 1, true, false, null)]
	public FormattedMessage Message;

	public ExamineEntry(string component, float priority, FormattedMessage message)
	{
		Component = component;
		Priority = priority;
		Message = message;
	}

	private ExamineEntry()
	{
		Message = null;
		Component = null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExamineEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ExamineEntry>(this, ref target, hookCtx, false, context))
		{
			string ComponentTemp = null;
			if (Component == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Component, ref ComponentTemp, hookCtx, false, context))
			{
				ComponentTemp = Component;
			}
			target.Component = ComponentTemp;
			float PriorityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Priority, ref PriorityTemp, hookCtx, false, context))
			{
				PriorityTemp = Priority;
			}
			target.Priority = PriorityTemp;
			FormattedMessage MessageTemp = null;
			if (Message == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<FormattedMessage>(Message, ref MessageTemp, hookCtx, false, context))
			{
				MessageTemp = serialization.CreateCopy<FormattedMessage>(Message, hookCtx, context, false);
			}
			target.Message = MessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExamineEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExamineEntry cast = (ExamineEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ExamineEntry Instantiate()
	{
		return new ExamineEntry();
	}
}
