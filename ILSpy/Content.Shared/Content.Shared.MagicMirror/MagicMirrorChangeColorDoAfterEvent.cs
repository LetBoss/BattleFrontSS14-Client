using System;
using System.Collections.Generic;
using Content.Shared.DoAfter;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorChangeColorDoAfterEvent : DoAfterEvent, ISerializationGenerated<MagicMirrorChangeColorDoAfterEvent>, ISerializationGenerated
{
	public MagicMirrorCategory Category;

	public int Slot;

	public List<Color> Colors = new List<Color>();

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MagicMirrorChangeColorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MagicMirrorChangeColorDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<MagicMirrorChangeColorDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MagicMirrorChangeColorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicMirrorChangeColorDoAfterEvent cast = (MagicMirrorChangeColorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicMirrorChangeColorDoAfterEvent cast = (MagicMirrorChangeColorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagicMirrorChangeColorDoAfterEvent Instantiate()
	{
		return new MagicMirrorChangeColorDoAfterEvent();
	}
}
