using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Reagent;

[Serializable]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
public sealed class DnaData : ReagentData, ISerializationGenerated<DnaData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string DNA = string.Empty;

	public override ReagentData Clone()
	{
		return this;
	}

	public override bool Equals(ReagentData? other)
	{
		if (other == null)
		{
			return false;
		}
		return ((DnaData)other).DNA == DNA;
	}

	public override int GetHashCode()
	{
		return DNA.GetHashCode();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DnaData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ReagentData definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DnaData)definitionCast;
		if (!serialization.TryCustomCopy<DnaData>(this, ref target, hookCtx, false, context))
		{
			string DNATemp = null;
			if (DNA == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DNA, ref DNATemp, hookCtx, false, context))
			{
				DNATemp = DNA;
			}
			target.DNA = DNATemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DnaData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ReagentData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DnaData cast = (DnaData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DnaData cast = (DnaData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DnaData Instantiate()
	{
		return new DnaData();
	}
}
