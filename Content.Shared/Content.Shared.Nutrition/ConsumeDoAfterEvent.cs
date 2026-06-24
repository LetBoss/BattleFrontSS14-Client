using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition;

[Serializable]
[NetSerializable]
public sealed class ConsumeDoAfterEvent : DoAfterEvent, ISerializationGenerated<ConsumeDoAfterEvent>, ISerializationGenerated
{
	[DataField("solution", false, 1, true, false, null)]
	public string Solution;

	[DataField("flavorMessage", false, 1, true, false, null)]
	public string FlavorMessage;

	private ConsumeDoAfterEvent()
	{
	}

	public ConsumeDoAfterEvent(string solution, string flavorMessage)
	{
		Solution = solution;
		FlavorMessage = flavorMessage;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ConsumeDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ConsumeDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<ConsumeDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
			string FlavorMessageTemp = null;
			if (FlavorMessage == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(FlavorMessage, ref FlavorMessageTemp, hookCtx, false, context))
			{
				FlavorMessageTemp = FlavorMessage;
			}
			target.FlavorMessage = FlavorMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ConsumeDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConsumeDoAfterEvent cast = (ConsumeDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConsumeDoAfterEvent cast = (ConsumeDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ConsumeDoAfterEvent Instantiate()
	{
		return new ConsumeDoAfterEvent();
	}
}
