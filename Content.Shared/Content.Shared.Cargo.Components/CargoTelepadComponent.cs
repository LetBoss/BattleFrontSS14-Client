using System;
using System.Collections.Generic;
using Content.Shared.DeviceLinking;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Cargo.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedCargoSystem) })]
public sealed class CargoTelepadComponent : Component, ISerializationGenerated<CargoTelepadComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<CargoOrderData> CurrentOrders = new List<CargoOrderData>();

	[DataField("delay", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Delay = 5f;

	[DataField("accumulator", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Accumulator;

	[DataField("currentState", false, 1, false, false, null)]
	public CargoTelepadState CurrentState;

	[DataField("teleportSound", false, 1, false, false, null)]
	public SoundSpecifier TeleportSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/phasein.ogg", (AudioParams?)null);

	[DataField("printerOutput", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string PrinterOutput = "PaperCargoInvoice";

	[DataField("receiverPort", false, 1, false, false, typeof(PrototypeIdSerializer<SinkPortPrototype>))]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ReceiverPort = "OrderReceiver";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CargoTelepadComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CargoTelepadComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CargoTelepadComponent>(this, ref target, hookCtx, false, context))
		{
			List<CargoOrderData> CurrentOrdersTemp = null;
			if (CurrentOrders == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<CargoOrderData>>(CurrentOrders, ref CurrentOrdersTemp, hookCtx, true, context))
			{
				CurrentOrdersTemp = serialization.CreateCopy<List<CargoOrderData>>(CurrentOrders, hookCtx, context, false);
			}
			target.CurrentOrders = CurrentOrdersTemp;
			float DelayTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Delay, ref DelayTemp, hookCtx, false, context))
			{
				DelayTemp = Delay;
			}
			target.Delay = DelayTemp;
			float AccumulatorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Accumulator, ref AccumulatorTemp, hookCtx, false, context))
			{
				AccumulatorTemp = Accumulator;
			}
			target.Accumulator = AccumulatorTemp;
			CargoTelepadState CurrentStateTemp = CargoTelepadState.Unpowered;
			if (!serialization.TryCustomCopy<CargoTelepadState>(CurrentState, ref CurrentStateTemp, hookCtx, false, context))
			{
				CurrentStateTemp = CurrentState;
			}
			target.CurrentState = CurrentStateTemp;
			SoundSpecifier TeleportSoundTemp = null;
			if (TeleportSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(TeleportSound, ref TeleportSoundTemp, hookCtx, true, context))
			{
				TeleportSoundTemp = serialization.CreateCopy<SoundSpecifier>(TeleportSound, hookCtx, context, false);
			}
			target.TeleportSound = TeleportSoundTemp;
			string PrinterOutputTemp = null;
			if (PrinterOutput == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PrinterOutput, ref PrinterOutputTemp, hookCtx, false, context))
			{
				PrinterOutputTemp = PrinterOutput;
			}
			target.PrinterOutput = PrinterOutputTemp;
			string ReceiverPortTemp = null;
			if (ReceiverPort == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ReceiverPort, ref ReceiverPortTemp, hookCtx, false, context))
			{
				ReceiverPortTemp = ReceiverPort;
			}
			target.ReceiverPort = ReceiverPortTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CargoTelepadComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoTelepadComponent cast = (CargoTelepadComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoTelepadComponent cast = (CargoTelepadComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CargoTelepadComponent def = (CargoTelepadComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CargoTelepadComponent Instantiate()
	{
		return new CargoTelepadComponent();
	}
}
