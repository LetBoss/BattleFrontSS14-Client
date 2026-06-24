using System;
using System.Collections.Generic;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;

namespace Content.Client.IconSmoothing;

[RegisterComponent]
public sealed class IconSmoothComponent : Component, ISerializationGenerated<IconSmoothComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("enabled", false, 1, false, false, null)]
	public bool Enabled = true;

	public (EntityUid?, Vector2i)? LastPosition;

	[DataField(null, false, 1, false, false, null)]
	public List<string> AdditionalKeys = new List<string>();

	[DataField("shader", false, 1, false, false, typeof(PrototypeIdSerializer<ShaderPrototype>))]
	public string? Shader;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("mode", false, 1, false, false, null)]
	public IconSmoothingMode Mode;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("key", false, 1, false, false, null)]
	public string? SmoothKey { get; private set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("base", false, 1, false, false, null)]
	public string StateBase { get; set; } = string.Empty;

	internal int UpdateGeneration { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref IconSmoothComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Component val = (Component)(object)target;
		((Component)this).InternalCopy(ref val, serialization, hookCtx, context);
		target = (IconSmoothComponent)(object)val;
		if (!serialization.TryCustomCopy<IconSmoothComponent>(this, ref target, hookCtx, false, context))
		{
			bool enabled = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref enabled, hookCtx, false, context))
			{
				enabled = Enabled;
			}
			target.Enabled = enabled;
			string smoothKey = null;
			if (!serialization.TryCustomCopy<string>(SmoothKey, ref smoothKey, hookCtx, false, context))
			{
				smoothKey = SmoothKey;
			}
			target.SmoothKey = smoothKey;
			List<string> additionalKeys = null;
			if (AdditionalKeys == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(AdditionalKeys, ref additionalKeys, hookCtx, true, context))
			{
				additionalKeys = serialization.CreateCopy<List<string>>(AdditionalKeys, hookCtx, context, false);
			}
			target.AdditionalKeys = additionalKeys;
			string stateBase = null;
			if (StateBase == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(StateBase, ref stateBase, hookCtx, false, context))
			{
				stateBase = StateBase;
			}
			target.StateBase = stateBase;
			string shader = null;
			if (!serialization.TryCustomCopy<string>(Shader, ref shader, hookCtx, false, context))
			{
				shader = Shader;
			}
			target.Shader = shader;
			IconSmoothingMode mode = IconSmoothingMode.Corners;
			if (!serialization.TryCustomCopy<IconSmoothingMode>(Mode, ref mode, hookCtx, false, context))
			{
				mode = Mode;
			}
			target.Mode = mode;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref IconSmoothComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IconSmoothComponent target2 = (IconSmoothComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (Component)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IconSmoothComponent target2 = (IconSmoothComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IconSmoothComponent target2 = (IconSmoothComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override IconSmoothComponent Instantiate()
	{
		return new IconSmoothComponent();
	}
}
