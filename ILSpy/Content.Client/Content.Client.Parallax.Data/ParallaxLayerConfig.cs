using System;
using System.Numerics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Client.Parallax.Data;

[DataDefinition]
public sealed class ParallaxLayerConfig : ISerializationGenerated<ParallaxLayerConfig>, ISerializationGenerated
{
	[DataField("scrolling", false, 1, false, false, null)]
	public Vector2 Scrolling = Vector2.Zero;

	[DataField("shader", false, 1, false, false, null)]
	public string? Shader = "unshaded";

	[DataField("texture", false, 1, true, false, null)]
	public IParallaxTextureSource Texture { get; set; }

	[DataField("scale", false, 1, false, false, null)]
	public Vector2 Scale { get; set; } = Vector2.One;

	[DataField("tiled", false, 1, false, false, null)]
	public bool Tiled { get; set; } = true;

	[DataField("controlHomePosition", false, 1, false, false, null)]
	public Vector2 ControlHomePosition { get; set; }

	[DataField("worldHomePosition", false, 1, false, false, null)]
	public Vector2 WorldHomePosition { get; set; }

	[DataField("worldAdjustPosition", false, 1, false, false, null)]
	public Vector2 WorldAdjustPosition { get; set; }

	[DataField("slowness", false, 1, false, false, null)]
	public float Slowness { get; set; } = 0.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ParallaxLayerConfig target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ParallaxLayerConfig>(this, ref target, hookCtx, false, context))
		{
			IParallaxTextureSource texture = null;
			if (Texture == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IParallaxTextureSource>(Texture, ref texture, hookCtx, true, context))
			{
				texture = serialization.CreateCopy<IParallaxTextureSource>(Texture, hookCtx, context, false);
			}
			target.Texture = texture;
			Vector2 scale = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Scale, ref scale, hookCtx, false, context))
			{
				scale = serialization.CreateCopy<Vector2>(Scale, hookCtx, context, false);
			}
			target.Scale = scale;
			bool tiled = false;
			if (!serialization.TryCustomCopy<bool>(Tiled, ref tiled, hookCtx, false, context))
			{
				tiled = Tiled;
			}
			target.Tiled = tiled;
			Vector2 controlHomePosition = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(ControlHomePosition, ref controlHomePosition, hookCtx, false, context))
			{
				controlHomePosition = serialization.CreateCopy<Vector2>(ControlHomePosition, hookCtx, context, false);
			}
			target.ControlHomePosition = controlHomePosition;
			Vector2 worldHomePosition = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(WorldHomePosition, ref worldHomePosition, hookCtx, false, context))
			{
				worldHomePosition = serialization.CreateCopy<Vector2>(WorldHomePosition, hookCtx, context, false);
			}
			target.WorldHomePosition = worldHomePosition;
			Vector2 worldAdjustPosition = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(WorldAdjustPosition, ref worldAdjustPosition, hookCtx, false, context))
			{
				worldAdjustPosition = serialization.CreateCopy<Vector2>(WorldAdjustPosition, hookCtx, context, false);
			}
			target.WorldAdjustPosition = worldAdjustPosition;
			float slowness = 0f;
			if (!serialization.TryCustomCopy<float>(Slowness, ref slowness, hookCtx, false, context))
			{
				slowness = Slowness;
			}
			target.Slowness = slowness;
			Vector2 scrolling = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Scrolling, ref scrolling, hookCtx, false, context))
			{
				scrolling = serialization.CreateCopy<Vector2>(Scrolling, hookCtx, context, false);
			}
			target.Scrolling = scrolling;
			string shader = null;
			if (!serialization.TryCustomCopy<string>(Shader, ref shader, hookCtx, false, context))
			{
				shader = Shader;
			}
			target.Shader = shader;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ParallaxLayerConfig target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ParallaxLayerConfig target2 = (ParallaxLayerConfig)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ParallaxLayerConfig Instantiate()
	{
		return new ParallaxLayerConfig();
	}
}
