using System;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Medical.Cryogenics;

[RegisterComponent]
[NetworkedComponent]
public sealed class CryoPodComponent : Component, ISerializationGenerated<CryoPodComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public enum CryoPodVisuals : byte
	{
		ContainsEntity,
		IsOn
	}

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("beakerTransferTime", false, 1, false, false, null)]
	public float BeakerTransferTime = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("nextInjectionTime", false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan? NextInjectionTime;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("beakerTransferAmount", false, 1, false, false, null)]
	public float BeakerTransferAmount = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("entryDelay", false, 1, false, false, null)]
	public float EntryDelay = 2f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("pryDelay", false, 1, false, false, null)]
	public float PryDelay = 5f;

	[ViewVariables]
	public ContainerSlot BodyContainer;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("port", false, 1, false, false, null)]
	public string PortName { get; set; } = "port";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("solutionContainerName", false, 1, false, false, null)]
	public string SolutionContainerName { get; set; } = "beakerSlot";

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("locked", false, 1, false, false, null)]
	public bool Locked { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("permaLocked", false, 1, false, false, null)]
	public bool PermaLocked { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CryoPodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CryoPodComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CryoPodComponent>(this, ref target, hookCtx, false, context))
		{
			string PortNameTemp = null;
			if (PortName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PortName, ref PortNameTemp, hookCtx, false, context))
			{
				PortNameTemp = PortName;
			}
			target.PortName = PortNameTemp;
			string SolutionContainerNameTemp = null;
			if (SolutionContainerName == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SolutionContainerName, ref SolutionContainerNameTemp, hookCtx, false, context))
			{
				SolutionContainerNameTemp = SolutionContainerName;
			}
			target.SolutionContainerName = SolutionContainerNameTemp;
			float BeakerTransferTimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BeakerTransferTime, ref BeakerTransferTimeTemp, hookCtx, false, context))
			{
				BeakerTransferTimeTemp = BeakerTransferTime;
			}
			target.BeakerTransferTime = BeakerTransferTimeTemp;
			TimeSpan? NextInjectionTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(NextInjectionTime, ref NextInjectionTimeTemp, hookCtx, false, context))
			{
				NextInjectionTimeTemp = serialization.CreateCopy<TimeSpan?>(NextInjectionTime, hookCtx, context, false);
			}
			target.NextInjectionTime = NextInjectionTimeTemp;
			float BeakerTransferAmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BeakerTransferAmount, ref BeakerTransferAmountTemp, hookCtx, false, context))
			{
				BeakerTransferAmountTemp = BeakerTransferAmount;
			}
			target.BeakerTransferAmount = BeakerTransferAmountTemp;
			float EntryDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EntryDelay, ref EntryDelayTemp, hookCtx, false, context))
			{
				EntryDelayTemp = EntryDelay;
			}
			target.EntryDelay = EntryDelayTemp;
			float PryDelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PryDelay, ref PryDelayTemp, hookCtx, false, context))
			{
				PryDelayTemp = PryDelay;
			}
			target.PryDelay = PryDelayTemp;
			bool LockedTemp = false;
			if (!serialization.TryCustomCopy<bool>(Locked, ref LockedTemp, hookCtx, false, context))
			{
				LockedTemp = Locked;
			}
			target.Locked = LockedTemp;
			bool PermaLockedTemp = false;
			if (!serialization.TryCustomCopy<bool>(PermaLocked, ref PermaLockedTemp, hookCtx, false, context))
			{
				PermaLockedTemp = PermaLocked;
			}
			target.PermaLocked = PermaLockedTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CryoPodComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CryoPodComponent cast = (CryoPodComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CryoPodComponent cast = (CryoPodComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CryoPodComponent def = (CryoPodComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CryoPodComponent Instantiate()
	{
		return new CryoPodComponent();
	}
}
