using System;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class ConstructionInteractDoAfterEvent : DoAfterEvent, ISerializationGenerated<ConstructionInteractDoAfterEvent>, ISerializationGenerated
{
	[DataField("clickLocation", false, 1, false, false, null)]
	public NetCoordinates ClickLocation;

	private ConstructionInteractDoAfterEvent()
	{
	}

	public ConstructionInteractDoAfterEvent(IEntityManager entManager, InteractUsingEvent ev)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		ClickLocation = entManager.GetNetCoordinates(ev.ClickLocation, (MetaDataComponent)null);
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ConstructionInteractDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ConstructionInteractDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<ConstructionInteractDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			NetCoordinates ClickLocationTemp = default(NetCoordinates);
			if (!serialization.TryCustomCopy<NetCoordinates>(ClickLocation, ref ClickLocationTemp, hookCtx, false, context))
			{
				ClickLocationTemp = serialization.CreateCopy<NetCoordinates>(ClickLocation, hookCtx, context, false);
			}
			target.ClickLocation = ClickLocationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ConstructionInteractDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstructionInteractDoAfterEvent cast = (ConstructionInteractDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ConstructionInteractDoAfterEvent cast = (ConstructionInteractDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ConstructionInteractDoAfterEvent Instantiate()
	{
		return new ConstructionInteractDoAfterEvent();
	}
}
