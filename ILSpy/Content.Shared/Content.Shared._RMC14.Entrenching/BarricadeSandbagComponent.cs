using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Entrenching;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(BarricadeSystem) })]
public sealed class BarricadeSandbagComponent : Component, ISerializationGenerated<BarricadeSandbagComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Material = EntProtoId.op_Implicit("CMSandbagFull");

	[DataField(null, false, 1, false, false, null)]
	public int MaxMaterial;

	[DataField(null, false, 1, false, false, null)]
	public int MaterialLossDamageInterval = 75;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref BarricadeSandbagComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (BarricadeSandbagComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BarricadeSandbagComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId MaterialTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Material, ref MaterialTemp, hookCtx, false, context))
			{
				MaterialTemp = serialization.CreateCopy<EntProtoId>(Material, hookCtx, context, false);
			}
			target.Material = MaterialTemp;
			int MaxMaterialTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxMaterial, ref MaxMaterialTemp, hookCtx, false, context))
			{
				MaxMaterialTemp = MaxMaterial;
			}
			target.MaxMaterial = MaxMaterialTemp;
			int MaterialLossDamageIntervalTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaterialLossDamageInterval, ref MaterialLossDamageIntervalTemp, hookCtx, false, context))
			{
				MaterialLossDamageIntervalTemp = MaterialLossDamageInterval;
			}
			target.MaterialLossDamageInterval = MaterialLossDamageIntervalTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref BarricadeSandbagComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BarricadeSandbagComponent cast = (BarricadeSandbagComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BarricadeSandbagComponent cast = (BarricadeSandbagComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BarricadeSandbagComponent def = (BarricadeSandbagComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BarricadeSandbagComponent Instantiate()
	{
		return new BarricadeSandbagComponent();
	}
}
