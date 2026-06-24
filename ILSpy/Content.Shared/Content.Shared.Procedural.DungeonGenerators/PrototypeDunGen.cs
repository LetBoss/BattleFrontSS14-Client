using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.DungeonGenerators;

public sealed class PrototypeDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<PrototypeDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public DungeonInheritance InheritDungeons;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<DungeonConfigPrototype> Proto;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PrototypeDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<PrototypeDunGen>(this, ref target, hookCtx, false, context))
		{
			DungeonInheritance InheritDungeonsTemp = DungeonInheritance.None;
			if (!serialization.TryCustomCopy<DungeonInheritance>(InheritDungeons, ref InheritDungeonsTemp, hookCtx, false, context))
			{
				InheritDungeonsTemp = InheritDungeons;
			}
			target.InheritDungeons = InheritDungeonsTemp;
			ProtoId<DungeonConfigPrototype> ProtoTemp = default(ProtoId<DungeonConfigPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<DungeonConfigPrototype>>(Proto, ref ProtoTemp, hookCtx, false, context))
			{
				ProtoTemp = serialization.CreateCopy<ProtoId<DungeonConfigPrototype>>(Proto, hookCtx, context, false);
			}
			target.Proto = ProtoTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PrototypeDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrototypeDunGen cast = (PrototypeDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrototypeDunGen def = (PrototypeDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PrototypeDunGen Instantiate()
	{
		return new PrototypeDunGen();
	}

	IDunGenLayer IDunGenLayer.Instantiate()
	{
		return Instantiate();
	}

	IDunGenLayer ISerializationGenerated<IDunGenLayer>.Instantiate()
	{
		return Instantiate();
	}
}
