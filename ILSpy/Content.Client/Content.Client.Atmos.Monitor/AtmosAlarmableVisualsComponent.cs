using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Monitor;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Atmos.Monitor;

[RegisterComponent]
public sealed class AtmosAlarmableVisualsComponent : Component, ISerializationGenerated<AtmosAlarmableVisualsComponent>, ISerializationGenerated
{
	[DataField("alarmStates", false, 1, false, false, null)]
	public Dictionary<AtmosAlarmType, string> AlarmStates = new Dictionary<AtmosAlarmType, string>();

	[DataField("hideOnDepowered", false, 1, false, false, null)]
	public List<string>? HideOnDepowered;

	[DataField("setOnDepowered", false, 1, false, false, null)]
	public Dictionary<string, string>? SetOnDepowered;

	[DataField("layerMap", false, 1, false, false, null)]
	public string LayerMap { get; private set; } = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AtmosAlarmableVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (AtmosAlarmableVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<AtmosAlarmableVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			string layerMap = null;
			if (LayerMap == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(LayerMap, ref layerMap, hookCtx, false, context))
			{
				layerMap = LayerMap;
			}
			target.LayerMap = layerMap;
			Dictionary<AtmosAlarmType, string> alarmStates = null;
			if (AlarmStates == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<AtmosAlarmType, string>>(AlarmStates, ref alarmStates, hookCtx, true, context))
			{
				alarmStates = serialization.CreateCopy<Dictionary<AtmosAlarmType, string>>(AlarmStates, hookCtx, context, false);
			}
			target.AlarmStates = alarmStates;
			List<string> hideOnDepowered = null;
			if (!serialization.TryCustomCopy<List<string>>(HideOnDepowered, ref hideOnDepowered, hookCtx, true, context))
			{
				hideOnDepowered = serialization.CreateCopy<List<string>>(HideOnDepowered, hookCtx, context, false);
			}
			target.HideOnDepowered = hideOnDepowered;
			Dictionary<string, string> setOnDepowered = null;
			if (!serialization.TryCustomCopy<Dictionary<string, string>>(SetOnDepowered, ref setOnDepowered, hookCtx, true, context))
			{
				setOnDepowered = serialization.CreateCopy<Dictionary<string, string>>(SetOnDepowered, hookCtx, context, false);
			}
			target.SetOnDepowered = setOnDepowered;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AtmosAlarmableVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlarmableVisualsComponent target2 = (AtmosAlarmableVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlarmableVisualsComponent target2 = (AtmosAlarmableVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosAlarmableVisualsComponent target2 = (AtmosAlarmableVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AtmosAlarmableVisualsComponent Instantiate()
	{
		return new AtmosAlarmableVisualsComponent();
	}
}
