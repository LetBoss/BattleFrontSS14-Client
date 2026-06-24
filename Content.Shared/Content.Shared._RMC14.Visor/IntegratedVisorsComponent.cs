using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Visor;

[RegisterComponent]
[NetworkedComponent]
public sealed class IntegratedVisorsComponent : Component, ISerializationGenerated<IntegratedVisorsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> VisorsToAdd = new List<EntProtoId>();

	[DataField(null, false, 1, false, false, null)]
	public bool StartToggled;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IntegratedVisorsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (IntegratedVisorsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<IntegratedVisorsComponent>(this, ref target, hookCtx, false, context))
		{
			List<EntProtoId> VisorsToAddTemp = null;
			if (VisorsToAdd == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntProtoId>>(VisorsToAdd, ref VisorsToAddTemp, hookCtx, true, context))
			{
				VisorsToAddTemp = serialization.CreateCopy<List<EntProtoId>>(VisorsToAdd, hookCtx, context, false);
			}
			target.VisorsToAdd = VisorsToAddTemp;
			bool StartToggledTemp = false;
			if (!serialization.TryCustomCopy<bool>(StartToggled, ref StartToggledTemp, hookCtx, false, context))
			{
				StartToggledTemp = StartToggled;
			}
			target.StartToggled = StartToggledTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IntegratedVisorsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntegratedVisorsComponent cast = (IntegratedVisorsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntegratedVisorsComponent cast = (IntegratedVisorsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IntegratedVisorsComponent def = (IntegratedVisorsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IntegratedVisorsComponent Instantiate()
	{
		return new IntegratedVisorsComponent();
	}
}
