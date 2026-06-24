using System;
using System.Collections.Generic;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Silicons.Laws;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class SiliconLawset : ISerializationGenerated<SiliconLawset>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public List<SiliconLaw> Laws = new List<SiliconLaw>();

	[DataField(null, false, 1, true, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string ObeysTo = string.Empty;

	public string LoggingString()
	{
		List<string> laws = new List<string>(Laws.Count);
		foreach (SiliconLaw law in Laws)
		{
			laws.Add($"{law.Order}: {Loc.GetString(law.LawString)}");
		}
		return string.Join(" / ", laws);
	}

	public SiliconLawset Clone()
	{
		List<SiliconLaw> laws = new List<SiliconLaw>(Laws.Count);
		foreach (SiliconLaw law in Laws)
		{
			laws.Add(law.ShallowClone());
		}
		return new SiliconLawset
		{
			Laws = laws,
			ObeysTo = ObeysTo
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SiliconLawset target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<SiliconLawset>(this, ref target, hookCtx, false, context))
		{
			List<SiliconLaw> LawsTemp = null;
			if (Laws == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<SiliconLaw>>(Laws, ref LawsTemp, hookCtx, true, context))
			{
				LawsTemp = serialization.CreateCopy<List<SiliconLaw>>(Laws, hookCtx, context, false);
			}
			target.Laws = LawsTemp;
			string ObeysToTemp = null;
			if (ObeysTo == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ObeysTo, ref ObeysToTemp, hookCtx, false, context))
			{
				ObeysToTemp = ObeysTo;
			}
			target.ObeysTo = ObeysToTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SiliconLawset target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SiliconLawset cast = (SiliconLawset)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SiliconLawset Instantiate()
	{
		return new SiliconLawset();
	}
}
