using System;
using Content.Shared.Alert;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Tracker.Xeno;

public sealed class HiveTrackerAltClickedAlertEvent : BaseAlertEvent, ISerializationGenerated<HiveTrackerAltClickedAlertEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HiveTrackerAltClickedAlertEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseAlertEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HiveTrackerAltClickedAlertEvent)definitionCast;
		serialization.TryCustomCopy<HiveTrackerAltClickedAlertEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HiveTrackerAltClickedAlertEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref BaseAlertEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveTrackerAltClickedAlertEvent cast = (HiveTrackerAltClickedAlertEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveTrackerAltClickedAlertEvent cast = (HiveTrackerAltClickedAlertEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HiveTrackerAltClickedAlertEvent Instantiate()
	{
		return new HiveTrackerAltClickedAlertEvent();
	}
}
