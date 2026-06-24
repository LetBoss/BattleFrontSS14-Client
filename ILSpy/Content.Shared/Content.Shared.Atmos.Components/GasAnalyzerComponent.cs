using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class GasAnalyzerComponent : Component, ISerializationGenerated<GasAnalyzerComponent>, ISerializationGenerated
{
	[Serializable]
	[NetSerializable]
	public enum GasAnalyzerUiKey
	{
		Key
	}

	[Serializable]
	[NetSerializable]
	public sealed class GasAnalyzerUserMessage : BoundUserInterfaceMessage
	{
		public string DeviceName;

		public NetEntity DeviceUid;

		public bool DeviceFlipped;

		public string? Error;

		public GasMixEntry[] NodeGasMixes;

		public GasAnalyzerUserMessage(GasMixEntry[] nodeGasMixes, string deviceName, NetEntity deviceUid, bool deviceFlipped, string? error = null)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			NodeGasMixes = nodeGasMixes;
			DeviceName = deviceName;
			DeviceUid = deviceUid;
			DeviceFlipped = deviceFlipped;
			Error = error;
		}
	}

	[Serializable]
	[NetSerializable]
	public struct GasMixEntry(string name, float volume, float pressure, float temperature, GasEntry[]? gases = null)
	{
		public readonly string Name = name;

		public readonly float Volume = volume;

		public readonly float Pressure = pressure;

		public readonly float Temperature = temperature;

		public readonly GasEntry[]? Gases = gases;
	}

	[Serializable]
	[NetSerializable]
	public struct GasEntry(string name, float amount, string color)
	{
		public readonly string Name = name;

		public readonly float Amount = amount;

		public readonly string Color = color;

		public override string ToString()
		{
			return Loc.GetString("gas-entry-info", new(string, object)[2]
			{
				("gasName", Name),
				("gasAmount", Amount)
			});
		}
	}

	[ViewVariables]
	public EntityUid? Target;

	[ViewVariables]
	public EntityUid User;

	[DataField("enabled", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Enabled;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GasAnalyzerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GasAnalyzerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GasAnalyzerComponent>(this, ref target, hookCtx, false, context))
		{
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GasAnalyzerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GasAnalyzerComponent cast = (GasAnalyzerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GasAnalyzerComponent cast = (GasAnalyzerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GasAnalyzerComponent def = (GasAnalyzerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GasAnalyzerComponent Instantiate()
	{
		return new GasAnalyzerComponent();
	}
}
