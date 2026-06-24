using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition;

[Serializable]
[NetSerializable]
public sealed class VapeDoAfterEvent : DoAfterEvent, ISerializationGenerated<VapeDoAfterEvent>, ISerializationGenerated
{
	[DataField("solution", false, 1, true, false, null)]
	public Solution Solution;

	[DataField("forced", false, 1, true, false, null)]
	public bool Forced;

	private VapeDoAfterEvent()
	{
	}

	public VapeDoAfterEvent(Solution solution, bool forced)
	{
		Solution = solution;
		Forced = forced;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VapeDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VapeDoAfterEvent)definitionCast;
		if (serialization.TryCustomCopy<VapeDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Solution SolutionTemp = null;
		if (Solution == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<Solution>(Solution, ref SolutionTemp, hookCtx, true, context))
		{
			if (Solution == null)
			{
				SolutionTemp = null;
			}
			else
			{
				serialization.CopyTo<Solution>(Solution, ref SolutionTemp, hookCtx, context, true);
			}
		}
		target.Solution = SolutionTemp;
		bool ForcedTemp = false;
		if (!serialization.TryCustomCopy<bool>(Forced, ref ForcedTemp, hookCtx, false, context))
		{
			ForcedTemp = Forced;
		}
		target.Forced = ForcedTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VapeDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VapeDoAfterEvent cast = (VapeDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VapeDoAfterEvent cast = (VapeDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VapeDoAfterEvent Instantiate()
	{
		return new VapeDoAfterEvent();
	}
}
