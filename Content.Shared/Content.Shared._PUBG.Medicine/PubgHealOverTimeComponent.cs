using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Medicine;

[RegisterComponent]
public sealed class PubgHealOverTimeComponent : Component, ISerializationGenerated<PubgHealOverTimeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 HealPerSecond;

	[DataField(null, false, 1, false, false, null)]
	public float SecondsRemaining;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan NextHealTime;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Source;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgHealOverTimeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgHealOverTimeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgHealOverTimeComponent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 HealPerSecondTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(HealPerSecond, ref HealPerSecondTemp, hookCtx, false, context))
			{
				HealPerSecondTemp = serialization.CreateCopy<FixedPoint2>(HealPerSecond, hookCtx, context, false);
			}
			target.HealPerSecond = HealPerSecondTemp;
			float SecondsRemainingTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SecondsRemaining, ref SecondsRemainingTemp, hookCtx, false, context))
			{
				SecondsRemainingTemp = SecondsRemaining;
			}
			target.SecondsRemaining = SecondsRemainingTemp;
			TimeSpan NextHealTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NextHealTime, ref NextHealTimeTemp, hookCtx, false, context))
			{
				NextHealTimeTemp = serialization.CreateCopy<TimeSpan>(NextHealTime, hookCtx, context, false);
			}
			target.NextHealTime = NextHealTimeTemp;
			EntityUid? SourceTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Source, ref SourceTemp, hookCtx, false, context))
			{
				SourceTemp = serialization.CreateCopy<EntityUid?>(Source, hookCtx, context, false);
			}
			target.Source = SourceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgHealOverTimeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgHealOverTimeComponent cast = (PubgHealOverTimeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgHealOverTimeComponent cast = (PubgHealOverTimeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgHealOverTimeComponent def = (PubgHealOverTimeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgHealOverTimeComponent Instantiate()
	{
		return new PubgHealOverTimeComponent();
	}
}
