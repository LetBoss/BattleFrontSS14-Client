using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lightning.Components;

public abstract class SharedLightningComponent : Component, ISerializationGenerated<SharedLightningComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("canArc", false, 1, false, false, null)]
	public bool CanArc;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("maxTotalArc", false, 1, false, false, null)]
	public int MaxTotalArcs = 50;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("lightningPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string LightningPrototype = "Lightning";

	[DataField("arcTarget", false, 1, false, false, null)]
	public EntityUid? ArcTarget;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("maxLength", false, 1, false, false, null)]
	public float MaxLength = 5f;

	[DataField("arcTargets", false, 1, false, false, null)]
	public HashSet<EntityUid> ArcTargets = new HashSet<EntityUid>();

	[DataField("collisionMask", false, 1, false, false, null)]
	public int CollisionMask = 30;

	public SharedLightningComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedLightningComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedLightningComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedLightningComponent>(this, ref target, hookCtx, false, context))
		{
			bool CanArcTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanArc, ref CanArcTemp, hookCtx, false, context))
			{
				CanArcTemp = CanArc;
			}
			target.CanArc = CanArcTemp;
			int MaxTotalArcsTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxTotalArcs, ref MaxTotalArcsTemp, hookCtx, false, context))
			{
				MaxTotalArcsTemp = MaxTotalArcs;
			}
			target.MaxTotalArcs = MaxTotalArcsTemp;
			string LightningPrototypeTemp = null;
			if (LightningPrototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LightningPrototype, ref LightningPrototypeTemp, hookCtx, false, context))
			{
				LightningPrototypeTemp = LightningPrototype;
			}
			target.LightningPrototype = LightningPrototypeTemp;
			EntityUid? ArcTargetTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ArcTarget, ref ArcTargetTemp, hookCtx, false, context))
			{
				ArcTargetTemp = serialization.CreateCopy<EntityUid?>(ArcTarget, hookCtx, context, false);
			}
			target.ArcTarget = ArcTargetTemp;
			float MaxLengthTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxLength, ref MaxLengthTemp, hookCtx, false, context))
			{
				MaxLengthTemp = MaxLength;
			}
			target.MaxLength = MaxLengthTemp;
			HashSet<EntityUid> ArcTargetsTemp = null;
			if (ArcTargets == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<EntityUid>>(ArcTargets, ref ArcTargetsTemp, hookCtx, true, context))
			{
				ArcTargetsTemp = serialization.CreateCopy<HashSet<EntityUid>>(ArcTargets, hookCtx, context, false);
			}
			target.ArcTargets = ArcTargetsTemp;
			int CollisionMaskTemp = 0;
			if (!serialization.TryCustomCopy<int>(CollisionMask, ref CollisionMaskTemp, hookCtx, false, context))
			{
				CollisionMaskTemp = CollisionMask;
			}
			target.CollisionMask = CollisionMaskTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedLightningComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedLightningComponent cast = (SharedLightningComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedLightningComponent cast = (SharedLightningComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedLightningComponent def = (SharedLightningComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedLightningComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
