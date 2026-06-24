using System;
using Content.Shared.Revolutionary;
using Content.Shared.StatusIcon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Mindshield.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedRevolutionarySystem) })]
public sealed class MindShieldComponent : Component, ISerializationGenerated<MindShieldComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ProtoId<SecurityIconPrototype> MindShieldStatusIcon = ProtoId<SecurityIconPrototype>.op_Implicit("MindShieldIcon");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MindShieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MindShieldComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MindShieldComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<SecurityIconPrototype> MindShieldStatusIconTemp = default(ProtoId<SecurityIconPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<SecurityIconPrototype>>(MindShieldStatusIcon, ref MindShieldStatusIconTemp, hookCtx, false, context))
			{
				MindShieldStatusIconTemp = serialization.CreateCopy<ProtoId<SecurityIconPrototype>>(MindShieldStatusIcon, hookCtx, context, false);
			}
			target.MindShieldStatusIcon = MindShieldStatusIconTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MindShieldComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MindShieldComponent cast = (MindShieldComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MindShieldComponent cast = (MindShieldComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MindShieldComponent def = (MindShieldComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MindShieldComponent Instantiate()
	{
		return new MindShieldComponent();
	}
}
