using System;
using System.Collections.Generic;
using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cuffs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCuffableSystem) })]
public sealed class CuffableComponent : Component, ISerializationGenerated<CuffableComponent>, ISerializationGenerated
{
	[DataField("currentRSI", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string? CurrentRSI;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Container Container;

	[DataField("canStillInteract", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool CanStillInteract = true;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<AlertPrototype> CuffedAlert = ProtoId<AlertPrototype>.op_Implicit("Handcuffed");

	[ViewVariables]
	public int CuffedHandCount => ((BaseContainer)Container).ContainedEntities.Count * 2;

	[ViewVariables]
	public EntityUid LastAddedCuffs
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			IReadOnlyList<EntityUid> containedEntities = ((BaseContainer)Container).ContainedEntities;
			return containedEntities[containedEntities.Count - 1];
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CuffableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CuffableComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CuffableComponent>(this, ref target, hookCtx, false, context))
		{
			string CurrentRSITemp = null;
			if (!serialization.TryCustomCopy<string>(CurrentRSI, ref CurrentRSITemp, hookCtx, false, context))
			{
				CurrentRSITemp = CurrentRSI;
			}
			target.CurrentRSI = CurrentRSITemp;
			bool CanStillInteractTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanStillInteract, ref CanStillInteractTemp, hookCtx, false, context))
			{
				CanStillInteractTemp = CanStillInteract;
			}
			target.CanStillInteract = CanStillInteractTemp;
			ProtoId<AlertPrototype> CuffedAlertTemp = default(ProtoId<AlertPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(CuffedAlert, ref CuffedAlertTemp, hookCtx, false, context))
			{
				CuffedAlertTemp = serialization.CreateCopy<ProtoId<AlertPrototype>>(CuffedAlert, hookCtx, context, false);
			}
			target.CuffedAlert = CuffedAlertTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CuffableComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CuffableComponent cast = (CuffableComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CuffableComponent cast = (CuffableComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CuffableComponent def = (CuffableComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CuffableComponent Instantiate()
	{
		return new CuffableComponent();
	}
}
