using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Client.TextScreen;

[RegisterComponent]
public sealed class TextScreenTimerComponent : Component, ISerializationGenerated<TextScreenTimerComponent>, ISerializationGenerated
{
	[DataField("targetTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan Target = TimeSpan.Zero;

	public Dictionary<string, string?> LayerStatesToDraw = new Dictionary<string, string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TextScreenTimerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (TextScreenTimerComponent)(object)val;
		if (!serialization.TryCustomCopy<TextScreenTimerComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan target2 = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Target, ref target2, hookCtx, false, context))
			{
				target2 = serialization.CreateCopy<TimeSpan>(Target, hookCtx, context, false);
			}
			target.Target = target2;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TextScreenTimerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TextScreenTimerComponent target2 = (TextScreenTimerComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TextScreenTimerComponent target2 = (TextScreenTimerComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TextScreenTimerComponent target2 = (TextScreenTimerComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TextScreenTimerComponent Instantiate()
	{
		return new TextScreenTimerComponent();
	}
}
