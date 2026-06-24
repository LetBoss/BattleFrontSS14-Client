using System;
using Content.Shared.DoAfter;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Repairable;

public abstract class SharedRepairableSystem : EntitySystem
{
	[Serializable]
	[NetSerializable]
	protected sealed class RepairFinishedEvent : SimpleDoAfterEvent, ISerializationGenerated<RepairFinishedEvent>, ISerializationGenerated
	{
		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void InternalCopy(ref RepairFinishedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			SimpleDoAfterEvent definitionCast = target;
			base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
			target = (RepairFinishedEvent)definitionCast;
			serialization.TryCustomCopy<RepairFinishedEvent>(this, ref target, hookCtx, false, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public void Copy(ref RepairFinishedEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			InternalCopy(ref target, serialization, hookCtx, context);
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			RepairFinishedEvent cast = (RepairFinishedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CopyTo instead")]
		public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
		{
			RepairFinishedEvent cast = (RepairFinishedEvent)target;
			Copy(ref cast, serialization, hookCtx, context);
			target = cast;
		}

		[Obsolete("Use ISerializationManager.CreateCopy instead")]
		public override RepairFinishedEvent Instantiate()
		{
			return new RepairFinishedEvent();
		}
	}
}
