using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client._RMC14.Attachable.Components;

[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] { typeof(AttachableHolderVisualsSystem) })]
public sealed class AttachableHolderVisualsComponent : Component, ISerializationGenerated<AttachableHolderVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[AutoNetworkedField]
	public Dictionary<string, Vector2> Offsets = new Dictionary<string, Vector2>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AttachableHolderVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (AttachableHolderVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<AttachableHolderVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, Vector2> offsets = null;
			if (Offsets == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, Vector2>>(Offsets, ref offsets, hookCtx, true, context))
			{
				offsets = serialization.CreateCopy<Dictionary<string, Vector2>>(Offsets, hookCtx, context, false);
			}
			target.Offsets = offsets;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AttachableHolderVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableHolderVisualsComponent target2 = (AttachableHolderVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableHolderVisualsComponent target2 = (AttachableHolderVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableHolderVisualsComponent target2 = (AttachableHolderVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AttachableHolderVisualsComponent Instantiate()
	{
		return new AttachableHolderVisualsComponent();
	}
}
