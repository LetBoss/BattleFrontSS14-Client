using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedBorgSystem) })]
public sealed class SelectableBorgModuleComponent : Component, ISerializationGenerated<SelectableBorgModuleComponent>, ISerializationGenerated
{
	[DataField("moduleSwapAction", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string? ModuleSwapActionId = "ActionBorgSwapModule";

	[DataField("moduleSwapActionEntity", false, 1, false, false, null)]
	public EntityUid? ModuleSwapActionEntity;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SelectableBorgModuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SelectableBorgModuleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SelectableBorgModuleComponent>(this, ref target, hookCtx, false, context))
		{
			string ModuleSwapActionIdTemp = null;
			if (!serialization.TryCustomCopy<string>(ModuleSwapActionId, ref ModuleSwapActionIdTemp, hookCtx, false, context))
			{
				ModuleSwapActionIdTemp = ModuleSwapActionId;
			}
			target.ModuleSwapActionId = ModuleSwapActionIdTemp;
			EntityUid? ModuleSwapActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ModuleSwapActionEntity, ref ModuleSwapActionEntityTemp, hookCtx, false, context))
			{
				ModuleSwapActionEntityTemp = serialization.CreateCopy<EntityUid?>(ModuleSwapActionEntity, hookCtx, context, false);
			}
			target.ModuleSwapActionEntity = ModuleSwapActionEntityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SelectableBorgModuleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SelectableBorgModuleComponent cast = (SelectableBorgModuleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SelectableBorgModuleComponent cast = (SelectableBorgModuleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SelectableBorgModuleComponent def = (SelectableBorgModuleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SelectableBorgModuleComponent Instantiate()
	{
		return new SelectableBorgModuleComponent();
	}
}
