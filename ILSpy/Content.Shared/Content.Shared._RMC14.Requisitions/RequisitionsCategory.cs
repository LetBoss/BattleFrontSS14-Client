using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Requisitions;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RequisitionsCategory : ISerializationGenerated<RequisitionsCategory>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public List<RequisitionsEntry> Entries = new List<RequisitionsEntry>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RequisitionsCategory target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RequisitionsCategory>(this, ref target, hookCtx, false, context))
		{
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			List<RequisitionsEntry> EntriesTemp = null;
			if (Entries == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<RequisitionsEntry>>(Entries, ref EntriesTemp, hookCtx, true, context))
			{
				EntriesTemp = serialization.CreateCopy<List<RequisitionsEntry>>(Entries, hookCtx, context, false);
			}
			target.Entries = EntriesTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RequisitionsCategory target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RequisitionsCategory cast = (RequisitionsCategory)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RequisitionsCategory Instantiate()
	{
		return new RequisitionsCategory();
	}
}
