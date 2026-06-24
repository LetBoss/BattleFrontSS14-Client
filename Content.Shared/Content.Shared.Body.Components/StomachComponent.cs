using System;
using System.Collections.Generic;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Body.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[]
{
	typeof(StomachSystem),
	typeof(FoodSystem)
})]
public sealed class StomachComponent : Component, ISerializationGenerated<StomachComponent>, ISerializationGenerated
{
	public sealed class ReagentDelta
	{
		public readonly ReagentQuantity ReagentQuantity;

		public TimeSpan Lifetime { get; private set; }

		public ReagentDelta(ReagentQuantity reagentQuantity)
		{
			ReagentQuantity = reagentQuantity;
			Lifetime = TimeSpan.Zero;
		}

		public void Increment(TimeSpan delta)
		{
			Lifetime += delta;
		}
	}

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan NextUpdate;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1L);

	[DataField(null, false, 1, false, false, null)]
	public float UpdateIntervalMultiplier = 1f;

	[ViewVariables]
	public Entity<SolutionComponent>? Solution;

	[DataField(null, false, 1, false, false, null)]
	public string BodySolutionName = "chemicals";

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DigestionDelay = TimeSpan.FromSeconds(10L);

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist? SpecialDigestible;

	[DataField(null, false, 1, false, false, null)]
	public bool IsSpecialDigestibleExclusive = true;

	[ViewVariables]
	public readonly List<ReagentDelta> ReagentDeltas = new List<ReagentDelta>();

	[ViewVariables]
	public TimeSpan AdjustedUpdateInterval => UpdateInterval * UpdateIntervalMultiplier;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StomachComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StomachComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<StomachComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		TimeSpan NextUpdateTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(NextUpdate, ref NextUpdateTemp, hookCtx, false, context))
		{
			NextUpdateTemp = serialization.CreateCopy<TimeSpan>(NextUpdate, hookCtx, context, false);
		}
		target.NextUpdate = NextUpdateTemp;
		TimeSpan UpdateIntervalTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(UpdateInterval, ref UpdateIntervalTemp, hookCtx, false, context))
		{
			UpdateIntervalTemp = serialization.CreateCopy<TimeSpan>(UpdateInterval, hookCtx, context, false);
		}
		target.UpdateInterval = UpdateIntervalTemp;
		float UpdateIntervalMultiplierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(UpdateIntervalMultiplier, ref UpdateIntervalMultiplierTemp, hookCtx, false, context))
		{
			UpdateIntervalMultiplierTemp = UpdateIntervalMultiplier;
		}
		target.UpdateIntervalMultiplier = UpdateIntervalMultiplierTemp;
		string BodySolutionNameTemp = null;
		if (BodySolutionName == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(BodySolutionName, ref BodySolutionNameTemp, hookCtx, false, context))
		{
			BodySolutionNameTemp = BodySolutionName;
		}
		target.BodySolutionName = BodySolutionNameTemp;
		TimeSpan DigestionDelayTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(DigestionDelay, ref DigestionDelayTemp, hookCtx, false, context))
		{
			DigestionDelayTemp = serialization.CreateCopy<TimeSpan>(DigestionDelay, hookCtx, context, false);
		}
		target.DigestionDelay = DigestionDelayTemp;
		EntityWhitelist SpecialDigestibleTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(SpecialDigestible, ref SpecialDigestibleTemp, hookCtx, false, context))
		{
			if (SpecialDigestible == null)
			{
				SpecialDigestibleTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(SpecialDigestible, ref SpecialDigestibleTemp, hookCtx, context, false);
			}
		}
		target.SpecialDigestible = SpecialDigestibleTemp;
		bool IsSpecialDigestibleExclusiveTemp = false;
		if (!serialization.TryCustomCopy<bool>(IsSpecialDigestibleExclusive, ref IsSpecialDigestibleExclusiveTemp, hookCtx, false, context))
		{
			IsSpecialDigestibleExclusiveTemp = IsSpecialDigestibleExclusive;
		}
		target.IsSpecialDigestibleExclusive = IsSpecialDigestibleExclusiveTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StomachComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StomachComponent cast = (StomachComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StomachComponent cast = (StomachComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StomachComponent def = (StomachComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StomachComponent Instantiate()
	{
		return new StomachComponent();
	}
}
