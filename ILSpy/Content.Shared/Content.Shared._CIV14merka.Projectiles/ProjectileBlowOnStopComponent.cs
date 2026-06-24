using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared._CIV14merka.Projectiles;

[RegisterComponent]
[NetworkedComponent]
public sealed class ProjectileBlowOnStopComponent : Component, ISerializationGenerated<ProjectileBlowOnStopComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float StopThreshold = 0.05f;

	[DataField(null, false, 1, false, false, null)]
	public int StallTicks = 3;

	[DataField(null, false, 1, false, false, null)]
	public float ArmDelay = 0.2f;

	[ViewVariables]
	[Access(new Type[] { typeof(EntitySystem) })]
	public TimeSpan ArmAt;

	[ViewVariables]
	[Access(new Type[] { typeof(EntitySystem) })]
	public int StallCounter;

	[ViewVariables]
	[Access(new Type[] { typeof(EntitySystem) })]
	public Vector2 PrevPos;

	[ViewVariables]
	[Access(new Type[] { typeof(EntitySystem) })]
	public bool PrevPosSet;

	[ViewVariables]
	[Access(new Type[] { typeof(EntitySystem) })]
	public bool Triggered;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ProjectileBlowOnStopComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ProjectileBlowOnStopComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ProjectileBlowOnStopComponent>(this, ref target, hookCtx, false, context))
		{
			float StopThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(StopThreshold, ref StopThresholdTemp, hookCtx, false, context))
			{
				StopThresholdTemp = StopThreshold;
			}
			target.StopThreshold = StopThresholdTemp;
			int StallTicksTemp = 0;
			if (!serialization.TryCustomCopy<int>(StallTicks, ref StallTicksTemp, hookCtx, false, context))
			{
				StallTicksTemp = StallTicks;
			}
			target.StallTicks = StallTicksTemp;
			float ArmDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ArmDelay, ref ArmDelayTemp, hookCtx, false, context))
			{
				ArmDelayTemp = ArmDelay;
			}
			target.ArmDelay = ArmDelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ProjectileBlowOnStopComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileBlowOnStopComponent cast = (ProjectileBlowOnStopComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileBlowOnStopComponent cast = (ProjectileBlowOnStopComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ProjectileBlowOnStopComponent def = (ProjectileBlowOnStopComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ProjectileBlowOnStopComponent Instantiate()
	{
		return new ProjectileBlowOnStopComponent();
	}
}
