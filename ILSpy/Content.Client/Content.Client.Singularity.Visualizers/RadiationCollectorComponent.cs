using System;
using Content.Shared.Singularity.Components;
using Robust.Client.Animations;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.Singularity.Visualizers;

[RegisterComponent]
[Access(new Type[] { typeof(RadiationCollectorSystem) })]
public sealed class RadiationCollectorComponent : Component, ISerializationGenerated<RadiationCollectorComponent>, ISerializationGenerated
{
	[ViewVariables]
	public const string AnimationKey = "radiationcollector_animation";

	[ViewVariables]
	public RadiationCollectorVisualState CurrentState;

	[DataField("activeState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ActiveState = "ca_on";

	[DataField("inactiveState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string InactiveState = "ca_off";

	[DataField("activatingState", false, 1, false, false, null)]
	public string ActivatingState = "ca_active";

	[DataField("deactivatingState", false, 1, false, false, null)]
	public string DeactivatingState = "ca_deactive";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Animation ActivateAnimation;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Animation DeactiveAnimation;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RadiationCollectorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (RadiationCollectorComponent)(object)val;
		if (!serialization.TryCustomCopy<RadiationCollectorComponent>(this, ref target, hookCtx, false, context))
		{
			string activeState = null;
			if (ActiveState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ActiveState, ref activeState, hookCtx, false, context))
			{
				activeState = ActiveState;
			}
			target.ActiveState = activeState;
			string inactiveState = null;
			if (InactiveState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(InactiveState, ref inactiveState, hookCtx, false, context))
			{
				inactiveState = InactiveState;
			}
			target.InactiveState = inactiveState;
			string activatingState = null;
			if (ActivatingState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ActivatingState, ref activatingState, hookCtx, false, context))
			{
				activatingState = ActivatingState;
			}
			target.ActivatingState = activatingState;
			string deactivatingState = null;
			if (DeactivatingState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DeactivatingState, ref deactivatingState, hookCtx, false, context))
			{
				deactivatingState = DeactivatingState;
			}
			target.DeactivatingState = deactivatingState;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RadiationCollectorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadiationCollectorComponent target2 = (RadiationCollectorComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadiationCollectorComponent target2 = (RadiationCollectorComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadiationCollectorComponent target2 = (RadiationCollectorComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RadiationCollectorComponent Instantiate()
	{
		return new RadiationCollectorComponent();
	}
}
