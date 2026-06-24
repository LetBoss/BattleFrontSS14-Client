using System;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Fluids;

[Serializable]
[NetSerializable]
public sealed class AbsorbantDoAfterEvent : DoAfterEvent, ISerializationGenerated<AbsorbantDoAfterEvent>, ISerializationGenerated
{
	[DataField("solution", false, 1, true, false, null)]
	public string TargetSolution;

	[DataField("message", false, 1, true, false, null)]
	public string Message;

	[DataField("sound", false, 1, true, false, null)]
	public SoundSpecifier Sound;

	[DataField("transferAmount", false, 1, true, false, null)]
	public FixedPoint2 TransferAmount;

	private AbsorbantDoAfterEvent()
	{
	}

	public AbsorbantDoAfterEvent(string targetSolution, string message, SoundSpecifier sound, FixedPoint2 transferAmount)
	{
		TargetSolution = targetSolution;
		Message = message;
		Sound = sound;
		TransferAmount = transferAmount;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AbsorbantDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AbsorbantDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<AbsorbantDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			string TargetSolutionTemp = null;
			if (TargetSolution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(TargetSolution, ref TargetSolutionTemp, hookCtx, false, context))
			{
				TargetSolutionTemp = TargetSolution;
			}
			target.TargetSolution = TargetSolutionTemp;
			string MessageTemp = null;
			if (Message == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Message, ref MessageTemp, hookCtx, false, context))
			{
				MessageTemp = Message;
			}
			target.Message = MessageTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
			FixedPoint2 TransferAmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(TransferAmount, ref TransferAmountTemp, hookCtx, false, context))
			{
				TransferAmountTemp = serialization.CreateCopy<FixedPoint2>(TransferAmount, hookCtx, context, false);
			}
			target.TransferAmount = TransferAmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AbsorbantDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AbsorbantDoAfterEvent cast = (AbsorbantDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AbsorbantDoAfterEvent cast = (AbsorbantDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AbsorbantDoAfterEvent Instantiate()
	{
		return new AbsorbantDoAfterEvent();
	}
}
