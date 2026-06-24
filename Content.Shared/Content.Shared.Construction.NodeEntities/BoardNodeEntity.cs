using System;
using Content.Shared.Construction.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Construction.NodeEntities;

[DataDefinition]
public sealed class BoardNodeEntity : IGraphNodeEntity, ISerializationGenerated<BoardNodeEntity>, ISerializationGenerated
{
	[DataField("container", false, 1, false, false, null)]
	public string Container { get; private set; } = string.Empty;

	public string? GetId(EntityUid? uid, EntityUid? userUid, GraphNodeEntityArgs args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!uid.HasValue)
		{
			return null;
		}
		BaseContainer container = default(BaseContainer);
		if (!args.EntityManager.EntitySysManager.GetEntitySystem<SharedContainerSystem>().TryGetContainer(uid.Value, Container, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count == 0)
		{
			return null;
		}
		EntityUid board = container.ContainedEntities[0];
		MachineBoardComponent machine = default(MachineBoardComponent);
		if (args.EntityManager.TryGetComponent<MachineBoardComponent>(board, ref machine))
		{
			return EntProtoId.op_Implicit(machine.Prototype);
		}
		ComputerBoardComponent computer = default(ComputerBoardComponent);
		if (args.EntityManager.TryGetComponent<ComputerBoardComponent>(board, ref computer))
		{
			return computer.Prototype;
		}
		return null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BoardNodeEntity target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<BoardNodeEntity>(this, ref target, hookCtx, false, context))
		{
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
	public void Copy(ref BoardNodeEntity target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BoardNodeEntity cast = (BoardNodeEntity)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public BoardNodeEntity Instantiate()
	{
		return new BoardNodeEntity();
	}
}
