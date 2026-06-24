using System;
using Content.Shared.Destructible.Thresholds;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.GameTicking.Components;

[RegisterComponent]
[EntityCategory(new string[] { "GameRules" })]
public sealed class GameRuleComponent : Component, ISerializationGenerated<GameRuleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan ActivatedAt;

	[DataField(null, false, 1, false, false, null)]
	public int MinPlayers;

	[DataField(null, false, 1, false, false, null)]
	public bool CancelPresetOnTooFewPlayers = true;

	[DataField(null, false, 1, false, false, null)]
	public MinMax? Delay;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GameRuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GameRuleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GameRuleComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan ActivatedAtTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(ActivatedAt, ref ActivatedAtTemp, hookCtx, false, context))
			{
				ActivatedAtTemp = serialization.CreateCopy<TimeSpan>(ActivatedAt, hookCtx, context, false);
			}
			target.ActivatedAt = ActivatedAtTemp;
			int MinPlayersTemp = 0;
			if (!serialization.TryCustomCopy<int>(MinPlayers, ref MinPlayersTemp, hookCtx, false, context))
			{
				MinPlayersTemp = MinPlayers;
			}
			target.MinPlayers = MinPlayersTemp;
			bool CancelPresetOnTooFewPlayersTemp = false;
			if (!serialization.TryCustomCopy<bool>(CancelPresetOnTooFewPlayers, ref CancelPresetOnTooFewPlayersTemp, hookCtx, false, context))
			{
				CancelPresetOnTooFewPlayersTemp = CancelPresetOnTooFewPlayers;
			}
			target.CancelPresetOnTooFewPlayers = CancelPresetOnTooFewPlayersTemp;
			MinMax? DelayTemp = null;
			if (!serialization.TryCustomCopy<MinMax?>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<MinMax?>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GameRuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GameRuleComponent cast = (GameRuleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GameRuleComponent cast = (GameRuleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GameRuleComponent def = (GameRuleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GameRuleComponent Instantiate()
	{
		return new GameRuleComponent();
	}
}
