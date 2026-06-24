using System;
using System.Collections.Generic;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[]
{
	typeof(HardpointSystem),
	typeof(HardpointSlotSystem)
})]
public sealed class HardpointSlotsComponent : Component, ISerializationGenerated<HardpointSlotsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HardpointVehicleFamilyPrototype>? VehicleFamily;

	[DataField(null, false, 1, true, false, null)]
	public List<HardpointSlot> Slots = new List<HardpointSlot>();

	[DataField(null, false, 1, false, false, null)]
	public float FrameDamageFractionWhileIntact = 0.1f;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ToolQualityPrototype> RemoveToolQuality = ProtoId<ToolQualityPrototype>.op_Implicit("Prying");

	[NonSerialized]
	public HashSet<string> PendingInserts = new HashSet<string>();

	[NonSerialized]
	public HashSet<string> CompletingInserts = new HashSet<string>();

	[NonSerialized]
	public HashSet<string> PendingRemovals = new HashSet<string>();

	[NonSerialized]
	public HashSet<EntityUid> PendingInsertUsers = new HashSet<EntityUid>();

	[NonSerialized]
	public string? LastUiError;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HardpointSlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HardpointSlotsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HardpointSlotsComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<HardpointVehicleFamilyPrototype>? VehicleFamilyTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(VehicleFamily, ref VehicleFamilyTemp, hookCtx, false, context))
			{
				VehicleFamilyTemp = serialization.CreateCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(VehicleFamily, hookCtx, context, false);
			}
			target.VehicleFamily = VehicleFamilyTemp;
			List<HardpointSlot> SlotsTemp = null;
			if (Slots == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<HardpointSlot>>(Slots, ref SlotsTemp, hookCtx, true, context))
			{
				SlotsTemp = serialization.CreateCopy<List<HardpointSlot>>(Slots, hookCtx, context, false);
			}
			target.Slots = SlotsTemp;
			float FrameDamageFractionWhileIntactTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FrameDamageFractionWhileIntact, ref FrameDamageFractionWhileIntactTemp, hookCtx, false, context))
			{
				FrameDamageFractionWhileIntactTemp = FrameDamageFractionWhileIntact;
			}
			target.FrameDamageFractionWhileIntact = FrameDamageFractionWhileIntactTemp;
			ProtoId<ToolQualityPrototype> RemoveToolQualityTemp = default(ProtoId<ToolQualityPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(RemoveToolQuality, ref RemoveToolQualityTemp, hookCtx, false, context))
			{
				RemoveToolQualityTemp = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(RemoveToolQuality, hookCtx, context, false);
			}
			target.RemoveToolQuality = RemoveToolQualityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HardpointSlotsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointSlotsComponent cast = (HardpointSlotsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointSlotsComponent cast = (HardpointSlotsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointSlotsComponent def = (HardpointSlotsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HardpointSlotsComponent Instantiate()
	{
		return new HardpointSlotsComponent();
	}
}
