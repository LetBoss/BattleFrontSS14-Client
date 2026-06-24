using System;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cargo;

[Serializable]
[DataDefinition]
[NetSerializable]
public readonly record struct CargoBountyData : ISerializationGenerated<CargoBountyData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string Id { get; init; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<CargoBountyPrototype> Bounty { get; init; }

	public CargoBountyData(CargoBountyPrototype bounty, int uniqueIdentifier)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Id = string.Empty;
		Bounty = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
		Bounty = ProtoId<CargoBountyPrototype>.op_Implicit(bounty.ID);
		Id = $"{bounty.IdPrefix}{uniqueIdentifier:D3}";
	}

	public CargoBountyData()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Id = string.Empty;
		Bounty = ProtoId<CargoBountyPrototype>.op_Implicit(string.Empty);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CargoBountyData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CargoBountyData>(this, ref target, hookCtx, false, context))
		{
			string IdTemp = null;
			if (Id == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = Id;
			}
			ProtoId<CargoBountyPrototype> BountyTemp = default(ProtoId<CargoBountyPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<CargoBountyPrototype>>(Bounty, ref BountyTemp, hookCtx, false, context))
			{
				BountyTemp = serialization.CreateCopy<ProtoId<CargoBountyPrototype>>(Bounty, hookCtx, context, false);
			}
			target = target with
			{
				Id = IdTemp,
				Bounty = BountyTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CargoBountyData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoBountyData cast = (CargoBountyData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CargoBountyData Instantiate()
	{
		return new CargoBountyData();
	}
}
