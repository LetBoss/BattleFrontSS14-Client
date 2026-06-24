using System;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.ComponentTable;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedComponentTableSystem) })]
public sealed class ComponentTableComponent : Component, ISerializationGenerated<ComponentTableComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntityTableSelector Table;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ComponentTableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ComponentTableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ComponentTableComponent>(this, ref target, hookCtx, false, context))
		{
			EntityTableSelector TableTemp = null;
			if (Table == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<EntityTableSelector>(Table, ref TableTemp, hookCtx, true, context))
			{
				TableTemp = serialization.CreateCopy<EntityTableSelector>(Table, hookCtx, context, false);
			}
			target.Table = TableTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ComponentTableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentTableComponent cast = (ComponentTableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentTableComponent cast = (ComponentTableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ComponentTableComponent def = (ComponentTableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ComponentTableComponent Instantiate()
	{
		return new ComponentTableComponent();
	}
}
