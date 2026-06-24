using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.SprayPainter;

[Serializable]
[NetSerializable]
public sealed class SprayPainterDoorDoAfterEvent : DoAfterEvent, ISerializationGenerated<SprayPainterDoorDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Sprite;

	[DataField(null, false, 1, false, false, null)]
	public string? Department;

	public SprayPainterDoorDoAfterEvent(string sprite, string? department)
	{
		Sprite = sprite;
		Department = department;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public SprayPainterDoorDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SprayPainterDoorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SprayPainterDoorDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<SprayPainterDoorDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			string SpriteTemp = null;
			if (Sprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Sprite, ref SpriteTemp, hookCtx, false, context))
			{
				SpriteTemp = Sprite;
			}
			target.Sprite = SpriteTemp;
			string DepartmentTemp = null;
			if (!serialization.TryCustomCopy<string>(Department, ref DepartmentTemp, hookCtx, false, context))
			{
				DepartmentTemp = Department;
			}
			target.Department = DepartmentTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SprayPainterDoorDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SprayPainterDoorDoAfterEvent cast = (SprayPainterDoorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SprayPainterDoorDoAfterEvent cast = (SprayPainterDoorDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SprayPainterDoorDoAfterEvent Instantiate()
	{
		return new SprayPainterDoorDoAfterEvent();
	}
}
