using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Polymorph;

[Prototype(null, 1)]
[DataDefinition]
public sealed class PolymorphPrototype : IPrototype, IInheritingPrototype, ISerializationGenerated<PolymorphPrototype>, ISerializationGenerated
{
	[DataField(null, false, 1, true, true, null)]
	public PolymorphConfiguration Configuration = new PolymorphConfiguration();

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<PolymorphPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PolymorphPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<PolymorphPrototype>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string IDTemp = null;
		if (ID == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(ID, ref IDTemp, hookCtx, false, context))
		{
			IDTemp = ID;
		}
		target.ID = IDTemp;
		string[] ParentsTemp = null;
		if (!serialization.TryCustomCopy<string[]>(Parents, ref ParentsTemp, hookCtx, true, context))
		{
			ParentsTemp = serialization.CreateCopy<string[]>(Parents, hookCtx, context, false);
		}
		target.Parents = ParentsTemp;
		bool AbstractTemp = false;
		if (!serialization.TryCustomCopy<bool>(Abstract, ref AbstractTemp, hookCtx, false, context))
		{
			AbstractTemp = Abstract;
		}
		target.Abstract = AbstractTemp;
		PolymorphConfiguration ConfigurationTemp = null;
		if (Configuration == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<PolymorphConfiguration>(Configuration, ref ConfigurationTemp, hookCtx, false, context))
		{
			if (Configuration == null)
			{
				ConfigurationTemp = null;
			}
			else
			{
				serialization.CopyTo<PolymorphConfiguration>(Configuration, ref ConfigurationTemp, hookCtx, context, true);
			}
		}
		target.Configuration = ConfigurationTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PolymorphPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PolymorphPrototype cast = (PolymorphPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PolymorphPrototype Instantiate()
	{
		return new PolymorphPrototype();
	}
}
