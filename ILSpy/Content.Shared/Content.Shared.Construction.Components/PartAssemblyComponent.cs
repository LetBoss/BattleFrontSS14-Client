using System;
using System.Collections.Generic;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Construction.Components;

[RegisterComponent]
public sealed class PartAssemblyComponent : Component, ISerializationGenerated<PartAssemblyComponent>, ISerializationGenerated
{
	[DataField("parts", false, 1, true, false, null)]
	public Dictionary<string, List<string>> Parts = new Dictionary<string, List<string>>();

	[DataField("currentAssembly", false, 1, false, false, null)]
	public string? CurrentAssembly;

	[DataField("containerId", false, 1, false, false, null)]
	public string ContainerId = "part-container";

	[ViewVariables]
	public Container PartsContainer;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PartAssemblyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PartAssemblyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PartAssemblyComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, List<string>> PartsTemp = null;
			if (Parts == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(Parts, ref PartsTemp, hookCtx, true, context))
			{
				PartsTemp = serialization.CreateCopy<Dictionary<string, List<string>>>(Parts, hookCtx, context, false);
			}
			target.Parts = PartsTemp;
			string CurrentAssemblyTemp = null;
			if (!serialization.TryCustomCopy<string>(CurrentAssembly, ref CurrentAssemblyTemp, hookCtx, false, context))
			{
				CurrentAssemblyTemp = CurrentAssembly;
			}
			target.CurrentAssembly = CurrentAssemblyTemp;
			string ContainerIdTemp = null;
			if (ContainerId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ContainerId, ref ContainerIdTemp, hookCtx, false, context))
			{
				ContainerIdTemp = ContainerId;
			}
			target.ContainerId = ContainerIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PartAssemblyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PartAssemblyComponent cast = (PartAssemblyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PartAssemblyComponent cast = (PartAssemblyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PartAssemblyComponent def = (PartAssemblyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PartAssemblyComponent Instantiate()
	{
		return new PartAssemblyComponent();
	}
}
