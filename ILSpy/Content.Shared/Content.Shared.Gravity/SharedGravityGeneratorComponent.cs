using System;
using System.Collections.Generic;
using Content.Shared.Power;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Gravity;

[NetworkedComponent]
[Virtual]
public class SharedGravityGeneratorComponent : Component, ISerializationGenerated<SharedGravityGeneratorComponent>, ISerializationGenerated
{
	[DataField("spriteMap", false, 1, false, false, null)]
	[Access(new Type[] { typeof(SharedGravitySystem) })]
	public Dictionary<PowerChargeStatus, string> SpriteMap = new Dictionary<PowerChargeStatus, string>();

	[DataField("coreStartupState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string CoreStartupState = "startup";

	[DataField("coreIdleState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string CoreIdleState = "idle";

	[DataField("coreActivatingState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string CoreActivatingState = "activating";

	[DataField("coreActivatedState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string CoreActivatedState = "activated";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedGravityGeneratorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedGravityGeneratorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedGravityGeneratorComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<PowerChargeStatus, string> SpriteMapTemp = null;
			if (SpriteMap == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<PowerChargeStatus, string>>(SpriteMap, ref SpriteMapTemp, hookCtx, true, context))
			{
				SpriteMapTemp = serialization.CreateCopy<Dictionary<PowerChargeStatus, string>>(SpriteMap, hookCtx, context, false);
			}
			target.SpriteMap = SpriteMapTemp;
			string CoreStartupStateTemp = null;
			if (CoreStartupState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CoreStartupState, ref CoreStartupStateTemp, hookCtx, false, context))
			{
				CoreStartupStateTemp = CoreStartupState;
			}
			target.CoreStartupState = CoreStartupStateTemp;
			string CoreIdleStateTemp = null;
			if (CoreIdleState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CoreIdleState, ref CoreIdleStateTemp, hookCtx, false, context))
			{
				CoreIdleStateTemp = CoreIdleState;
			}
			target.CoreIdleState = CoreIdleStateTemp;
			string CoreActivatingStateTemp = null;
			if (CoreActivatingState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CoreActivatingState, ref CoreActivatingStateTemp, hookCtx, false, context))
			{
				CoreActivatingStateTemp = CoreActivatingState;
			}
			target.CoreActivatingState = CoreActivatingStateTemp;
			string CoreActivatedStateTemp = null;
			if (CoreActivatedState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(CoreActivatedState, ref CoreActivatedStateTemp, hookCtx, false, context))
			{
				CoreActivatedStateTemp = CoreActivatedState;
			}
			target.CoreActivatedState = CoreActivatedStateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedGravityGeneratorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedGravityGeneratorComponent cast = (SharedGravityGeneratorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedGravityGeneratorComponent cast = (SharedGravityGeneratorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedGravityGeneratorComponent def = (SharedGravityGeneratorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedGravityGeneratorComponent Instantiate()
	{
		return new SharedGravityGeneratorComponent();
	}
}
