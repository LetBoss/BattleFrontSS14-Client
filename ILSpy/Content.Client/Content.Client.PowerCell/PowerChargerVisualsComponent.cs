using System;
using System.Collections.Generic;
using Content.Shared.Power;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Client.PowerCell;

[RegisterComponent]
[Access(new Type[] { typeof(PowerChargerVisualizerSystem) })]
public sealed class PowerChargerVisualsComponent : Component, ISerializationGenerated<PowerChargerVisualsComponent>, ISerializationGenerated
{
	[DataField("emptyState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string EmptyState = "empty";

	[DataField("occupiedState", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string OccupiedState = "full";

	[DataField("lightStates", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Dictionary<CellChargerStatus, string> LightStates = new Dictionary<CellChargerStatus, string>
	{
		[CellChargerStatus.Off] = "light-off",
		[CellChargerStatus.Empty] = "light-empty",
		[CellChargerStatus.Charging] = "light-charging",
		[CellChargerStatus.Charged] = "light-charged"
	};

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PowerChargerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (PowerChargerVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<PowerChargerVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string emptyState = null;
			if (EmptyState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(EmptyState, ref emptyState, hookCtx, false, context))
			{
				emptyState = EmptyState;
			}
			target.EmptyState = emptyState;
			string occupiedState = null;
			if (OccupiedState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(OccupiedState, ref occupiedState, hookCtx, false, context))
			{
				occupiedState = OccupiedState;
			}
			target.OccupiedState = occupiedState;
			Dictionary<CellChargerStatus, string> lightStates = null;
			if (LightStates == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<CellChargerStatus, string>>(LightStates, ref lightStates, hookCtx, true, context))
			{
				lightStates = serialization.CreateCopy<Dictionary<CellChargerStatus, string>>(LightStates, hookCtx, context, false);
			}
			target.LightStates = lightStates;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PowerChargerVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerChargerVisualsComponent target2 = (PowerChargerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerChargerVisualsComponent target2 = (PowerChargerVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PowerChargerVisualsComponent target2 = (PowerChargerVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PowerChargerVisualsComponent Instantiate()
	{
		return new PowerChargerVisualsComponent();
	}
}
