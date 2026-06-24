using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Xenoarchaeology.Equipment.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(NodeScannerSystem) })]
public sealed class NodeScannerComponent : Component, ISerializationGenerated<NodeScannerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int MaxLinkedRange = 5;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DisplayDataUpdateInterval = TimeSpan.FromSeconds(1L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NodeScannerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NodeScannerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NodeScannerComponent>(this, ref target, hookCtx, false, context))
		{
			int MaxLinkedRangeTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxLinkedRange, ref MaxLinkedRangeTemp, hookCtx, false, context))
			{
				MaxLinkedRangeTemp = MaxLinkedRange;
			}
			target.MaxLinkedRange = MaxLinkedRangeTemp;
			TimeSpan DisplayDataUpdateIntervalTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DisplayDataUpdateInterval, ref DisplayDataUpdateIntervalTemp, hookCtx, false, context))
			{
				DisplayDataUpdateIntervalTemp = serialization.CreateCopy<TimeSpan>(DisplayDataUpdateInterval, hookCtx, context, false);
			}
			target.DisplayDataUpdateInterval = DisplayDataUpdateIntervalTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NodeScannerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NodeScannerComponent cast = (NodeScannerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NodeScannerComponent cast = (NodeScannerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NodeScannerComponent def = (NodeScannerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NodeScannerComponent Instantiate()
	{
		return new NodeScannerComponent();
	}
}
