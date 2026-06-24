using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.NodeContainer;

[RegisterComponent]
[NetworkedComponent]
public sealed class NodeContainerComponent : Component, ISerializationGenerated<NodeContainerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Examinable;

	[DataField(null, true, 1, false, true, null)]
	public Dictionary<string, Node> Nodes { get; private set; } = new Dictionary<string, Node>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NodeContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NodeContainerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NodeContainerComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, Node> NodesTemp = null;
			if (Nodes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, Node>>(Nodes, ref NodesTemp, hookCtx, true, context))
			{
				NodesTemp = serialization.CreateCopy<Dictionary<string, Node>>(Nodes, hookCtx, context, false);
			}
			target.Nodes = NodesTemp;
			bool ExaminableTemp = false;
			if (!serialization.TryCustomCopy<bool>(Examinable, ref ExaminableTemp, hookCtx, false, context))
			{
				ExaminableTemp = Examinable;
			}
			target.Examinable = ExaminableTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NodeContainerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NodeContainerComponent cast = (NodeContainerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NodeContainerComponent cast = (NodeContainerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NodeContainerComponent def = (NodeContainerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NodeContainerComponent Instantiate()
	{
		return new NodeContainerComponent();
	}
}
