using System;
using System.Collections.Generic;
using Content.Shared.Atmos.Consoles;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedAtmosMonitoringConsoleSystem) })]
public sealed class AtmosMonitoringConsoleComponent : Component, ISerializationGenerated<AtmosMonitoringConsoleComponent>, ISerializationGenerated
{
	[ViewVariables]
	public Dictionary<Vector2i, AtmosPipeChunk> AtmosPipeChunks = new Dictionary<Vector2i, AtmosPipeChunk>();

	[ViewVariables]
	public Dictionary<NetEntity, AtmosDeviceNavMapData> AtmosDevices = new Dictionary<NetEntity, AtmosDeviceNavMapData>();

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables]
	public Color NavMapTileColor;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables]
	public Color NavMapWallColor;

	[ViewVariables]
	public bool ForceFullUpdate;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AtmosMonitoringConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AtmosMonitoringConsoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<AtmosMonitoringConsoleComponent>(this, ref target, hookCtx, false, context))
		{
			Color NavMapTileColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(NavMapTileColor, ref NavMapTileColorTemp, hookCtx, false, context))
			{
				NavMapTileColorTemp = serialization.CreateCopy<Color>(NavMapTileColor, hookCtx, context, false);
			}
			target.NavMapTileColor = NavMapTileColorTemp;
			Color NavMapWallColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(NavMapWallColor, ref NavMapWallColorTemp, hookCtx, false, context))
			{
				NavMapWallColorTemp = serialization.CreateCopy<Color>(NavMapWallColor, hookCtx, context, false);
			}
			target.NavMapWallColor = NavMapWallColorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AtmosMonitoringConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosMonitoringConsoleComponent cast = (AtmosMonitoringConsoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosMonitoringConsoleComponent cast = (AtmosMonitoringConsoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AtmosMonitoringConsoleComponent def = (AtmosMonitoringConsoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AtmosMonitoringConsoleComponent Instantiate()
	{
		return new AtmosMonitoringConsoleComponent();
	}
}
