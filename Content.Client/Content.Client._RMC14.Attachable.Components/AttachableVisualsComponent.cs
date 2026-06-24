using System;
using System.Numerics;
using Content.Client._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Client._RMC14.Attachable.Components;

[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] { typeof(AttachableHolderVisualsSystem) })]
public sealed class AttachableVisualsComponent : Component, ISerializationGenerated<AttachableVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public ResPath? Rsi;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public string? Prefix;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public string? Suffix = "_a";

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public bool IncludeSlotName;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public bool ShowActive;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public bool RedrawOnAppearanceChange;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public int Layer;

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public Vector2 Offset;

	[DataField(null, false, 1, false, false, null)]
	public string? LastSlotId;

	[DataField(null, false, 1, false, false, null)]
	public string? LastSuffix;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AttachableVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (AttachableVisualsComponent)(object)val;
		if (!serialization.TryCustomCopy<AttachableVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			ResPath? rsi = null;
			if (!serialization.TryCustomCopy<ResPath?>(Rsi, ref rsi, hookCtx, false, context))
			{
				rsi = serialization.CreateCopy<ResPath?>(Rsi, hookCtx, context, false);
			}
			target.Rsi = rsi;
			string prefix = null;
			if (!serialization.TryCustomCopy<string>(Prefix, ref prefix, hookCtx, false, context))
			{
				prefix = Prefix;
			}
			target.Prefix = prefix;
			string suffix = null;
			if (!serialization.TryCustomCopy<string>(Suffix, ref suffix, hookCtx, false, context))
			{
				suffix = Suffix;
			}
			target.Suffix = suffix;
			bool includeSlotName = false;
			if (!serialization.TryCustomCopy<bool>(IncludeSlotName, ref includeSlotName, hookCtx, false, context))
			{
				includeSlotName = IncludeSlotName;
			}
			target.IncludeSlotName = includeSlotName;
			bool showActive = false;
			if (!serialization.TryCustomCopy<bool>(ShowActive, ref showActive, hookCtx, false, context))
			{
				showActive = ShowActive;
			}
			target.ShowActive = showActive;
			bool redrawOnAppearanceChange = false;
			if (!serialization.TryCustomCopy<bool>(RedrawOnAppearanceChange, ref redrawOnAppearanceChange, hookCtx, false, context))
			{
				redrawOnAppearanceChange = RedrawOnAppearanceChange;
			}
			target.RedrawOnAppearanceChange = redrawOnAppearanceChange;
			int layer = 0;
			if (!serialization.TryCustomCopy<int>(Layer, ref layer, hookCtx, false, context))
			{
				layer = Layer;
			}
			target.Layer = layer;
			Vector2 offset = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Offset, ref offset, hookCtx, false, context))
			{
				offset = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
			}
			target.Offset = offset;
			string lastSlotId = null;
			if (!serialization.TryCustomCopy<string>(LastSlotId, ref lastSlotId, hookCtx, false, context))
			{
				lastSlotId = LastSlotId;
			}
			target.LastSlotId = lastSlotId;
			string lastSuffix = null;
			if (!serialization.TryCustomCopy<string>(LastSuffix, ref lastSuffix, hookCtx, false, context))
			{
				lastSuffix = LastSuffix;
			}
			target.LastSuffix = lastSuffix;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AttachableVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableVisualsComponent target2 = (AttachableVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableVisualsComponent target2 = (AttachableVisualsComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableVisualsComponent target2 = (AttachableVisualsComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AttachableVisualsComponent Instantiate()
	{
		return new AttachableVisualsComponent();
	}
}
