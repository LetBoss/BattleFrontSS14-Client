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
public sealed class RMCNamedItemComponent : Component, ISerializationGenerated<RMCNamedItemComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? User;

	[DataField(null, false, 1, false, false, null)]
	public RMCNamedItemType? Type;

	[DataField(null, false, 1, false, false, null)]
	public string Name = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCNamedItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RMCNamedItemComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RMCNamedItemComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? UserTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(User, ref UserTemp, hookCtx, false, context))
			{
				UserTemp = serialization.CreateCopy<EntityUid?>(User, hookCtx, context, false);
			}
			target.User = UserTemp;
			RMCNamedItemType? TypeTemp = null;
			if (!serialization.TryCustomCopy<RMCNamedItemType?>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCNamedItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCNamedItemComponent cast = (RMCNamedItemComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCNamedItemComponent cast = (RMCNamedItemComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCNamedItemComponent def = (RMCNamedItemComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RMCNamedItemComponent Instantiate()
	{
		return new RMCNamedItemComponent();
	}
}
