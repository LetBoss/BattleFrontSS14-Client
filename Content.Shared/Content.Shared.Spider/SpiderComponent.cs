using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Spider;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedSpiderSystem) })]
public sealed class SpiderComponent : Component, ISerializationGenerated<SpiderComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("webPrototype", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string WebPrototype = "SpiderWeb";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("webAction", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string WebAction = "ActionSpiderWeb";

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Action;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpiderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpiderComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpiderComponent>(this, ref target, hookCtx, false, context))
		{
			string WebPrototypeTemp = null;
			if (WebPrototype == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(WebPrototype, ref WebPrototypeTemp, hookCtx, false, context))
			{
				WebPrototypeTemp = WebPrototype;
			}
			target.WebPrototype = WebPrototypeTemp;
			string WebActionTemp = null;
			if (WebAction == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(WebAction, ref WebActionTemp, hookCtx, false, context))
			{
				WebActionTemp = WebAction;
			}
			target.WebAction = WebActionTemp;
			EntityUid? ActionTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Action, ref ActionTemp, hookCtx, false, context))
			{
				ActionTemp = serialization.CreateCopy<EntityUid?>(Action, hookCtx, context, false);
			}
			target.Action = ActionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpiderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpiderComponent cast = (SpiderComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpiderComponent cast = (SpiderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpiderComponent def = (SpiderComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpiderComponent Instantiate()
	{
		return new SpiderComponent();
	}
}
