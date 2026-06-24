using System;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Kitchen.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ExtractableComponent : Component, ISerializationGenerated<ExtractableComponent>, ISerializationGenerated
{
	[DataField("juiceSolution", false, 1, false, false, null)]
	public Solution? JuiceSolution;

	[DataField("grindableSolutionName", false, 1, false, false, null)]
	public string? GrindableSolution;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ExtractableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ExtractableComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ExtractableComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		Solution JuiceSolutionTemp = null;
		if (!serialization.TryCustomCopy<Solution>(JuiceSolution, ref JuiceSolutionTemp, hookCtx, true, context))
		{
			if (JuiceSolution == null)
			{
				JuiceSolutionTemp = null;
			}
			else
			{
				serialization.CopyTo<Solution>(JuiceSolution, ref JuiceSolutionTemp, hookCtx, context, false);
			}
		}
		target.JuiceSolution = JuiceSolutionTemp;
		string GrindableSolutionTemp = null;
		if (!serialization.TryCustomCopy<string>(GrindableSolution, ref GrindableSolutionTemp, hookCtx, false, context))
		{
			GrindableSolutionTemp = GrindableSolution;
		}
		target.GrindableSolution = GrindableSolutionTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ExtractableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtractableComponent cast = (ExtractableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtractableComponent cast = (ExtractableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ExtractableComponent def = (ExtractableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ExtractableComponent Instantiate()
	{
		return new ExtractableComponent();
	}
}
