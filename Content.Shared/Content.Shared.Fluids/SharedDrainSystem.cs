using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Fluids;

public abstract class SharedDrainSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	public sealed class DrainDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<DrainDoAfterEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref DrainDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (DrainDoAfterEvent)definitionCast;
			serialization.TryCustomCopy<DrainDoAfterEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref DrainDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DrainDoAfterEvent cast = (DrainDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			DrainDoAfterEvent cast = (DrainDoAfterEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override DrainDoAfterEvent Instantiate()
		{
			return new DrainDoAfterEvent();
		}
	}
}
