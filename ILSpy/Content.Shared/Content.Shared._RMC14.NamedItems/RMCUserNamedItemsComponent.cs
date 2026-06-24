using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.NamedItems;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedRMCNamedItemSystem) })]
public sealed class RMCUserNamedItemsComponent : Component, ISerializationGenerated<RMCUserNamedItemsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SharedRMCNamedItems Names = new SharedRMCNamedItems();

	[DataField(null, false, 1, false, false, null)]
	public EntityUid?[] Entities = new EntityUid?[SharedRMCNamedItemSystem.TypeCount];

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCUserNamedItemsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCUserNamedItemsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCUserNamedItemsComponent>(this, ref target, hookCtx, false, context))
		{
			SharedRMCNamedItems NamesTemp = null;
			if (Names == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SharedRMCNamedItems>(Names, ref NamesTemp, hookCtx, true, context))
			{
				NamesTemp = serialization.CreateCopy<SharedRMCNamedItems>(Names, hookCtx, context, false);
			}
			target.Names = NamesTemp;
			EntityUid?[] EntitiesTemp = null;
			if (Entities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<EntityUid?[]>(Entities, ref EntitiesTemp, hookCtx, true, context))
			{
				EntitiesTemp = serialization.CreateCopy<EntityUid?[]>(Entities, hookCtx, context, false);
			}
			target.Entities = EntitiesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCUserNamedItemsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCUserNamedItemsComponent cast = (RMCUserNamedItemsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCUserNamedItemsComponent cast = (RMCUserNamedItemsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCUserNamedItemsComponent def = (RMCUserNamedItemsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCUserNamedItemsComponent Instantiate()
	{
		return new RMCUserNamedItemsComponent();
	}
}
