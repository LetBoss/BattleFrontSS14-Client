using System;
using Content.Shared.EntityTable;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.PostGeneration;

public sealed class CorridorClutterDunGen : IDunGenLayer, ISerializationGenerated<IDunGenLayer>, ISerializationGenerated, ISerializationGenerated<CorridorClutterDunGen>
{
	[DataField(null, false, 1, false, false, null)]
	public float Chance = 0.05f;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<EntityTablePrototype> Contents;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CorridorClutterDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CorridorClutterDunGen>(this, ref target, hookCtx, false, context))
		{
			float ChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Chance, ref ChanceTemp, hookCtx, false, context))
			{
				ChanceTemp = Chance;
			}
			target.Chance = ChanceTemp;
			ProtoId<EntityTablePrototype> ContentsTemp = default(ProtoId<EntityTablePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(Contents, ref ContentsTemp, hookCtx, false, context))
			{
				ContentsTemp = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(Contents, hookCtx, context, false);
			}
			target.Contents = ContentsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CorridorClutterDunGen target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorridorClutterDunGen cast = (CorridorClutterDunGen)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CorridorClutterDunGen def = (CorridorClutterDunGen)target;
		Copy(ref def, serialization, hookCtx, context);
		target = def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IDunGenLayer target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CorridorClutterDunGen Instantiate()
	{
		return new CorridorClutterDunGen();
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
