using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Containers;

[RegisterComponent]
[NetworkedComponent]
public sealed class ContainerCompComponent : Component, ISerializationGenerated<ContainerCompComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Proto;

	[DataField(null, false, 1, true, false, null)]
	public string Container = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ContainerCompComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ContainerCompComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ContainerCompComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId ProtoTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Proto, ref ProtoTemp, hookCtx, false, context))
			{
				ProtoTemp = serialization.CreateCopy<EntProtoId>(Proto, hookCtx, context, false);
			}
			target.Proto = ProtoTemp;
			string ContainerTemp = null;
			if (Container == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Container, ref ContainerTemp, hookCtx, false, context))
			{
				ContainerTemp = Container;
			}
			target.Container = ContainerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ContainerCompComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerCompComponent cast = (ContainerCompComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerCompComponent cast = (ContainerCompComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ContainerCompComponent def = (ContainerCompComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ContainerCompComponent Instantiate()
	{
		return new ContainerCompComponent();
	}
}
