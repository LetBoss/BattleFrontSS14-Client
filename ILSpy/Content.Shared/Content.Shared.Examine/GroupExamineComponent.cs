using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Examine;

[RegisterComponent]
public sealed class GroupExamineComponent : Component, ISerializationGenerated<GroupExamineComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<ExamineGroup> Group = new List<ExamineGroup>
	{
		new ExamineGroup
		{
			Components = new List<string> { "Armor", "ClothingSpeedModifier" }
		}
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GroupExamineComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GroupExamineComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GroupExamineComponent>(this, ref target, hookCtx, false, context))
		{
			List<ExamineGroup> GroupTemp = null;
			if (Group == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ExamineGroup>>(Group, ref GroupTemp, hookCtx, true, context))
			{
				GroupTemp = serialization.CreateCopy<List<ExamineGroup>>(Group, hookCtx, context, false);
			}
			target.Group = GroupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GroupExamineComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupExamineComponent cast = (GroupExamineComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupExamineComponent cast = (GroupExamineComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupExamineComponent def = (GroupExamineComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GroupExamineComponent Instantiate()
	{
		return new GroupExamineComponent();
	}
}
