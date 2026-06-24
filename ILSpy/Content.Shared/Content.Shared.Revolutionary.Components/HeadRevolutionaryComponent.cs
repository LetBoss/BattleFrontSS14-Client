using System;
using Content.Shared.StatusIcon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Revolutionary.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedRevolutionarySystem) })]
public sealed class HeadRevolutionaryComponent : Component, ISerializationGenerated<HeadRevolutionaryComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan StunTime = TimeSpan.FromSeconds(3L);

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = ProtoId<FactionIconPrototype>.op_Implicit("HeadRevolutionaryFaction");

	public override bool SessionSpecific => true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HeadRevolutionaryComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HeadRevolutionaryComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HeadRevolutionaryComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<FactionIconPrototype> StatusIconTemp = default(ProtoId<FactionIconPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<FactionIconPrototype>>(StatusIcon, ref StatusIconTemp, hookCtx, false, context))
			{
				StatusIconTemp = serialization.CreateCopy<ProtoId<FactionIconPrototype>>(StatusIcon, hookCtx, context, false);
			}
			target.StatusIcon = StatusIconTemp;
			TimeSpan StunTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StunTime, ref StunTimeTemp, hookCtx, false, context))
			{
				StunTimeTemp = serialization.CreateCopy<TimeSpan>(StunTime, hookCtx, context, false);
			}
			target.StunTime = StunTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HeadRevolutionaryComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HeadRevolutionaryComponent cast = (HeadRevolutionaryComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HeadRevolutionaryComponent cast = (HeadRevolutionaryComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HeadRevolutionaryComponent def = (HeadRevolutionaryComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HeadRevolutionaryComponent Instantiate()
	{
		return new HeadRevolutionaryComponent();
	}
}
