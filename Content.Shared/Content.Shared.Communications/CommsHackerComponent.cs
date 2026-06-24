using System;
using Content.Shared.Random;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Communications;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCommsHackerSystem) })]
public sealed class CommsHackerComponent : Component, ISerializationGenerated<CommsHackerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan Delay = TimeSpan.FromSeconds(20L);

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<WeightedRandomPrototype> Threats = ProtoId<WeightedRandomPrototype>.op_Implicit(string.Empty);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CommsHackerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CommsHackerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CommsHackerComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan DelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = serialization.CreateCopy<TimeSpan>(Delay, hookCtx, context, false);
			}
			target.Delay = DelayTemp;
			ProtoId<WeightedRandomPrototype> ThreatsTemp = default(ProtoId<WeightedRandomPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<WeightedRandomPrototype>>(Threats, ref ThreatsTemp, hookCtx, false, context))
			{
				ThreatsTemp = serialization.CreateCopy<ProtoId<WeightedRandomPrototype>>(Threats, hookCtx, context, false);
			}
			target.Threats = ThreatsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CommsHackerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CommsHackerComponent cast = (CommsHackerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CommsHackerComponent cast = (CommsHackerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CommsHackerComponent def = (CommsHackerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CommsHackerComponent Instantiate()
	{
		return new CommsHackerComponent();
	}
}
