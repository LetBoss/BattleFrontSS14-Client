using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;

namespace Content.Shared.VendingMachines;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedVendingMachineSystem) })]
public sealed class VendingMachineRestockComponent : Component, ISerializationGenerated<VendingMachineRestockComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("restockDelay", false, 1, false, false, null)]
	public TimeSpan RestockDelay = TimeSpan.FromSeconds(5.0);

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("canRestock", false, 1, false, false, typeof(PrototypeIdHashSetSerializer<VendingMachineInventoryPrototype>))]
	public HashSet<string> CanRestock = new HashSet<string>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("soundRestockStart", false, 1, false, false, null)]
	public SoundSpecifier SoundRestockStart;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("soundRestockDone", false, 1, false, false, null)]
	public SoundSpecifier SoundRestockDone;

	public VendingMachineRestockComponent()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		SoundPathSpecifier val = new SoundPathSpecifier("/Audio/Machines/vending_restock_start.ogg", (AudioParams?)null);
		AudioParams val2 = default(AudioParams);
		((AudioParams)(ref val2))._002Ector();
		((AudioParams)(ref val2)).Volume = -2f;
		((AudioParams)(ref val2)).Variation = 0.2f;
		((SoundSpecifier)val).Params = val2;
		SoundRestockStart = (SoundSpecifier)val;
		SoundRestockDone = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/vending_restock_done.ogg", (AudioParams?)null);
		((Component)this)._002Ector();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VendingMachineRestockComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VendingMachineRestockComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VendingMachineRestockComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan RestockDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(RestockDelay, ref RestockDelayTemp, hookCtx, false, context))
			{
				RestockDelayTemp = serialization.CreateCopy<TimeSpan>(RestockDelay, hookCtx, context, false);
			}
			target.RestockDelay = RestockDelayTemp;
			HashSet<string> CanRestockTemp = null;
			if (CanRestock == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<string>>(CanRestock, ref CanRestockTemp, hookCtx, true, context))
			{
				CanRestockTemp = serialization.CreateCopy<HashSet<string>>(CanRestock, hookCtx, context, false);
			}
			target.CanRestock = CanRestockTemp;
			SoundSpecifier SoundRestockStartTemp = null;
			if (SoundRestockStart == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(SoundRestockStart, ref SoundRestockStartTemp, hookCtx, true, context))
			{
				SoundRestockStartTemp = serialization.CreateCopy<SoundSpecifier>(SoundRestockStart, hookCtx, context, false);
			}
			target.SoundRestockStart = SoundRestockStartTemp;
			SoundSpecifier SoundRestockDoneTemp = null;
			if (SoundRestockDone == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(SoundRestockDone, ref SoundRestockDoneTemp, hookCtx, true, context))
			{
				SoundRestockDoneTemp = serialization.CreateCopy<SoundSpecifier>(SoundRestockDone, hookCtx, context, false);
			}
			target.SoundRestockDone = SoundRestockDoneTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VendingMachineRestockComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VendingMachineRestockComponent cast = (VendingMachineRestockComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VendingMachineRestockComponent cast = (VendingMachineRestockComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VendingMachineRestockComponent def = (VendingMachineRestockComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VendingMachineRestockComponent Instantiate()
	{
		return new VendingMachineRestockComponent();
	}
}
