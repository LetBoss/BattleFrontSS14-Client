using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Chat.TypingIndicator;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedTypingIndicatorSystem) })]
public sealed class TypingIndicatorComponent : Component, ISerializationGenerated<TypingIndicatorComponent>, ISerializationGenerated
{
	[DataField("proto", false, 1, false, false, null)]
	public ProtoId<TypingIndicatorPrototype> TypingIndicatorPrototype = ProtoId<Content.Shared.Chat.TypingIndicator.TypingIndicatorPrototype>.op_Implicit("default");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref TypingIndicatorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (TypingIndicatorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<TypingIndicatorComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<TypingIndicatorPrototype> TypingIndicatorPrototypeTemp = default(ProtoId<TypingIndicatorPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<TypingIndicatorPrototype>>(TypingIndicatorPrototype, ref TypingIndicatorPrototypeTemp, hookCtx, false, context))
			{
				TypingIndicatorPrototypeTemp = serialization.CreateCopy<ProtoId<TypingIndicatorPrototype>>(TypingIndicatorPrototype, hookCtx, context, false);
			}
			target.TypingIndicatorPrototype = TypingIndicatorPrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref TypingIndicatorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TypingIndicatorComponent cast = (TypingIndicatorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TypingIndicatorComponent cast = (TypingIndicatorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		TypingIndicatorComponent def = (TypingIndicatorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override TypingIndicatorComponent Instantiate()
	{
		return new TypingIndicatorComponent();
	}
}
