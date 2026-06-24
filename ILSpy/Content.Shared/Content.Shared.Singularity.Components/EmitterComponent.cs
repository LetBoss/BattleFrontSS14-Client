using System;
using System.Collections.Generic;
using System.Threading;
using Content.Shared.DeviceLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EmitterComponent : Component, ISerializationGenerated<EmitterComponent>, ISerializationGenerated
{
	public CancellationTokenSource? TimerCancel;

	[ViewVariables]
	public bool IsOn;

	[ViewVariables]
	public bool IsPowered;

	[ViewVariables]
	public int FireShotCounter;

	[DataField("boltType", false, 1, false, false, typeof(PrototypeIdSerializer<EntityPrototype>))]
	public string BoltType = "EmitterBolt";

	[DataField("selectableTypes", false, 1, false, false, typeof(PrototypeIdListSerializer<EntityPrototype>))]
	public List<string> SelectableTypes = new List<string>();

	[DataField("powerUseActive", false, 1, false, false, null)]
	public int PowerUseActive = 600;

	[DataField("fireBurstSize", false, 1, false, false, null)]
	public int FireBurstSize = 3;

	[DataField("fireInterval", false, 1, false, false, null)]
	public TimeSpan FireInterval = TimeSpan.FromSeconds(2L);

	[DataField("fireBurstDelayMin", false, 1, false, false, null)]
	public TimeSpan FireBurstDelayMin = TimeSpan.FromSeconds(4L);

	[DataField("fireBurstDelayMax", false, 1, false, false, null)]
	public TimeSpan FireBurstDelayMax = TimeSpan.FromSeconds(10L);

	[DataField("onState", false, 1, false, false, null)]
	public string? OnState = "beam";

	[DataField("underpoweredState", false, 1, false, false, null)]
	public string? UnderpoweredState = "underpowered";

	[DataField("onPort", false, 1, false, false, typeof(PrototypeIdSerializer<SinkPortPrototype>))]
	public string OnPort = "On";

	[DataField("offPort", false, 1, false, false, typeof(PrototypeIdSerializer<SinkPortPrototype>))]
	public string OffPort = "Off";

	[DataField("togglePort", false, 1, false, false, typeof(PrototypeIdSerializer<SinkPortPrototype>))]
	public string TogglePort = "Toggle";

	[DataField("setTypePorts", false, 1, false, false, typeof(PrototypeIdDictionarySerializer<string, SinkPortPrototype>))]
	public Dictionary<string, string> SetTypePorts = new Dictionary<string, string>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmitterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EmitterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<EmitterComponent>(this, ref target, hookCtx, false, context))
		{
			string BoltTypeTemp = null;
			if (BoltType == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(BoltType, ref BoltTypeTemp, hookCtx, false, context))
			{
				BoltTypeTemp = BoltType;
			}
			target.BoltType = BoltTypeTemp;
			List<string> SelectableTypesTemp = null;
			if (SelectableTypes == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(SelectableTypes, ref SelectableTypesTemp, hookCtx, true, context))
			{
				SelectableTypesTemp = serialization.CreateCopy<List<string>>(SelectableTypes, hookCtx, context, false);
			}
			target.SelectableTypes = SelectableTypesTemp;
			int PowerUseActiveTemp = 0;
			if (!serialization.TryCustomCopy<int>(PowerUseActive, ref PowerUseActiveTemp, hookCtx, false, context))
			{
				PowerUseActiveTemp = PowerUseActive;
			}
			target.PowerUseActive = PowerUseActiveTemp;
			int FireBurstSizeTemp = 0;
			if (!serialization.TryCustomCopy<int>(FireBurstSize, ref FireBurstSizeTemp, hookCtx, false, context))
			{
				FireBurstSizeTemp = FireBurstSize;
			}
			target.FireBurstSize = FireBurstSizeTemp;
			TimeSpan FireIntervalTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(FireInterval, ref FireIntervalTemp, hookCtx, false, context))
			{
				FireIntervalTemp = serialization.CreateCopy<TimeSpan>(FireInterval, hookCtx, context, false);
			}
			target.FireInterval = FireIntervalTemp;
			TimeSpan FireBurstDelayMinTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(FireBurstDelayMin, ref FireBurstDelayMinTemp, hookCtx, false, context))
			{
				FireBurstDelayMinTemp = serialization.CreateCopy<TimeSpan>(FireBurstDelayMin, hookCtx, context, false);
			}
			target.FireBurstDelayMin = FireBurstDelayMinTemp;
			TimeSpan FireBurstDelayMaxTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(FireBurstDelayMax, ref FireBurstDelayMaxTemp, hookCtx, false, context))
			{
				FireBurstDelayMaxTemp = serialization.CreateCopy<TimeSpan>(FireBurstDelayMax, hookCtx, context, false);
			}
			target.FireBurstDelayMax = FireBurstDelayMaxTemp;
			string OnStateTemp = null;
			if (!serialization.TryCustomCopy<string>(OnState, ref OnStateTemp, hookCtx, false, context))
			{
				OnStateTemp = OnState;
			}
			target.OnState = OnStateTemp;
			string UnderpoweredStateTemp = null;
			if (!serialization.TryCustomCopy<string>(UnderpoweredState, ref UnderpoweredStateTemp, hookCtx, false, context))
			{
				UnderpoweredStateTemp = UnderpoweredState;
			}
			target.UnderpoweredState = UnderpoweredStateTemp;
			string OnPortTemp = null;
			if (OnPort == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(OnPort, ref OnPortTemp, hookCtx, false, context))
			{
				OnPortTemp = OnPort;
			}
			target.OnPort = OnPortTemp;
			string OffPortTemp = null;
			if (OffPort == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(OffPort, ref OffPortTemp, hookCtx, false, context))
			{
				OffPortTemp = OffPort;
			}
			target.OffPort = OffPortTemp;
			string TogglePortTemp = null;
			if (TogglePort == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(TogglePort, ref TogglePortTemp, hookCtx, false, context))
			{
				TogglePortTemp = TogglePort;
			}
			target.TogglePort = TogglePortTemp;
			Dictionary<string, string> SetTypePortsTemp = null;
			if (SetTypePorts == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, string>>(SetTypePorts, ref SetTypePortsTemp, hookCtx, true, context))
			{
				SetTypePortsTemp = serialization.CreateCopy<Dictionary<string, string>>(SetTypePorts, hookCtx, context, false);
			}
			target.SetTypePorts = SetTypePortsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmitterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmitterComponent cast = (EmitterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmitterComponent cast = (EmitterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmitterComponent def = (EmitterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EmitterComponent Instantiate()
	{
		return new EmitterComponent();
	}
}
