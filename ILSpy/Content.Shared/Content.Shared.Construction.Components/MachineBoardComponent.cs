using System;
using System.Collections.Generic;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Construction.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MachineBoardComponent : Component, ISerializationGenerated<MachineBoardComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<StackPrototype>, int> StackRequirements = new Dictionary<ProtoId<StackPrototype>, int>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<TagPrototype>, GenericPartInfo> TagRequirements = new Dictionary<ProtoId<TagPrototype>, GenericPartInfo>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, GenericPartInfo> ComponentRequirements = new Dictionary<string, GenericPartInfo>();

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Prototype;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MachineBoardComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MachineBoardComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MachineBoardComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<ProtoId<StackPrototype>, int> StackRequirementsTemp = null;
			if (StackRequirements == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<StackPrototype>, int>>(StackRequirements, ref StackRequirementsTemp, hookCtx, true, context))
			{
				StackRequirementsTemp = serialization.CreateCopy<Dictionary<ProtoId<StackPrototype>, int>>(StackRequirements, hookCtx, context, false);
			}
			target.StackRequirements = StackRequirementsTemp;
			Dictionary<ProtoId<TagPrototype>, GenericPartInfo> TagRequirementsTemp = null;
			if (TagRequirements == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<TagPrototype>, GenericPartInfo>>(TagRequirements, ref TagRequirementsTemp, hookCtx, true, context))
			{
				TagRequirementsTemp = serialization.CreateCopy<Dictionary<ProtoId<TagPrototype>, GenericPartInfo>>(TagRequirements, hookCtx, context, false);
			}
			target.TagRequirements = TagRequirementsTemp;
			Dictionary<string, GenericPartInfo> ComponentRequirementsTemp = null;
			if (ComponentRequirements == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, GenericPartInfo>>(ComponentRequirements, ref ComponentRequirementsTemp, hookCtx, true, context))
			{
				ComponentRequirementsTemp = serialization.CreateCopy<Dictionary<string, GenericPartInfo>>(ComponentRequirements, hookCtx, context, false);
			}
			target.ComponentRequirements = ComponentRequirementsTemp;
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MachineBoardComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MachineBoardComponent cast = (MachineBoardComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MachineBoardComponent cast = (MachineBoardComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MachineBoardComponent def = (MachineBoardComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MachineBoardComponent Instantiate()
	{
		return new MachineBoardComponent();
	}
}
