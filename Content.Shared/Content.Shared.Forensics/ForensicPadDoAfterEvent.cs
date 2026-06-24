using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Forensics;

[Serializable]
[NetSerializable]
public sealed class ForensicPadDoAfterEvent : DoAfterEvent, ISerializationGenerated<ForensicPadDoAfterEvent>, ISerializationGenerated
{
	[DataField("sample", false, 1, true, false, null)]
	public string Sample;

	private ForensicPadDoAfterEvent()
	{
	}

	public ForensicPadDoAfterEvent(string sample)
	{
		Sample = sample;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ForensicPadDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ForensicPadDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<ForensicPadDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			string SampleTemp = null;
			if (Sample == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Sample, ref SampleTemp, hookCtx, false, context))
			{
				SampleTemp = Sample;
			}
			target.Sample = SampleTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ForensicPadDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ForensicPadDoAfterEvent cast = (ForensicPadDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ForensicPadDoAfterEvent cast = (ForensicPadDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ForensicPadDoAfterEvent Instantiate()
	{
		return new ForensicPadDoAfterEvent();
	}
}
