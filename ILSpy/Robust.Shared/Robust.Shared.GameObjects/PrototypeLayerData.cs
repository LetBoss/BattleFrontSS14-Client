using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class PrototypeLayerData : ISerializationGenerated<PrototypeLayerData>, ISerializationGenerated
{
	[DataField("shader", false, 1, false, false, null)]
	public string? Shader;

	[DataField("texture", false, 1, false, false, null)]
	public string? TexturePath;

	[DataField("sprite", false, 1, false, false, null)]
	public string? RsiPath;

	[DataField("state", false, 1, false, false, null)]
	public string? State;

	[DataField("scale", false, 1, false, false, null)]
	public Vector2? Scale;

	[DataField("rotation", false, 1, false, false, null)]
	public Angle? Rotation;

	[DataField("offset", false, 1, false, false, null)]
	public Vector2? Offset;

	[DataField("visible", false, 1, false, false, null)]
	public bool? Visible;

	[DataField("color", false, 1, false, false, null)]
	public Color? Color;

	[DataField("map", false, 1, false, false, null)]
	public HashSet<string>? MapKeys;

	[DataField("renderingStrategy", false, 1, false, false, null)]
	public LayerRenderingStrategy? RenderingStrategy;

	[DataField(null, false, 1, false, false, null)]
	public PrototypeCopyToShaderParameters? CopyToShaderParameters;

	[DataField(null, false, 1, false, false, null)]
	public bool Cycle;

	[DataField(null, false, 1, false, false, null)]
	public bool Loop = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PrototypeLayerData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			return;
		}
		string target2 = null;
		if (!serialization.TryCustomCopy(Shader, ref target2, hookCtx, hasHooks: false, context))
		{
			target2 = Shader;
		}
		target.Shader = target2;
		string target3 = null;
		if (!serialization.TryCustomCopy(TexturePath, ref target3, hookCtx, hasHooks: false, context))
		{
			target3 = TexturePath;
		}
		target.TexturePath = target3;
		string target4 = null;
		if (!serialization.TryCustomCopy(RsiPath, ref target4, hookCtx, hasHooks: false, context))
		{
			target4 = RsiPath;
		}
		target.RsiPath = target4;
		string target5 = null;
		if (!serialization.TryCustomCopy(State, ref target5, hookCtx, hasHooks: false, context))
		{
			target5 = State;
		}
		target.State = target5;
		Vector2? target6 = null;
		if (!serialization.TryCustomCopy(Scale, ref target6, hookCtx, hasHooks: false, context))
		{
			target6 = serialization.CreateCopy(Scale, hookCtx, context);
		}
		target.Scale = target6;
		Angle? target7 = null;
		if (!serialization.TryCustomCopy(Rotation, ref target7, hookCtx, hasHooks: false, context))
		{
			target7 = serialization.CreateCopy(Rotation, hookCtx, context);
		}
		target.Rotation = target7;
		Vector2? target8 = null;
		if (!serialization.TryCustomCopy(Offset, ref target8, hookCtx, hasHooks: false, context))
		{
			target8 = serialization.CreateCopy(Offset, hookCtx, context);
		}
		target.Offset = target8;
		bool? target9 = null;
		if (!serialization.TryCustomCopy(Visible, ref target9, hookCtx, hasHooks: false, context))
		{
			target9 = Visible;
		}
		target.Visible = target9;
		Color? target10 = null;
		if (!serialization.TryCustomCopy(Color, ref target10, hookCtx, hasHooks: false, context))
		{
			target10 = serialization.CreateCopy(Color, hookCtx, context);
		}
		target.Color = target10;
		HashSet<string> target11 = null;
		if (!serialization.TryCustomCopy(MapKeys, ref target11, hookCtx, hasHooks: true, context))
		{
			target11 = serialization.CreateCopy(MapKeys, hookCtx, context);
		}
		target.MapKeys = target11;
		LayerRenderingStrategy? target12 = null;
		if (!serialization.TryCustomCopy(RenderingStrategy, ref target12, hookCtx, hasHooks: false, context))
		{
			target12 = RenderingStrategy;
		}
		target.RenderingStrategy = target12;
		PrototypeCopyToShaderParameters target13 = null;
		if (!serialization.TryCustomCopy(CopyToShaderParameters, ref target13, hookCtx, hasHooks: false, context))
		{
			if (CopyToShaderParameters == null)
			{
				target13 = null;
			}
			else
			{
				serialization.CopyTo(CopyToShaderParameters, ref target13, hookCtx, context);
			}
		}
		target.CopyToShaderParameters = target13;
		bool target14 = false;
		if (!serialization.TryCustomCopy(Cycle, ref target14, hookCtx, hasHooks: false, context))
		{
			target14 = Cycle;
		}
		target.Cycle = target14;
		bool target15 = false;
		if (!serialization.TryCustomCopy(Loop, ref target15, hookCtx, hasHooks: false, context))
		{
			target15 = Loop;
		}
		target.Loop = target15;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PrototypeLayerData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PrototypeLayerData target2 = (PrototypeLayerData)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public PrototypeLayerData Instantiate()
	{
		return new PrototypeLayerData();
	}
}
