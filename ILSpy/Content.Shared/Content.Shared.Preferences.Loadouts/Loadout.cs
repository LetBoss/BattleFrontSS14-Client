using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Preferences.Loadouts;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class Loadout : IEquatable<Loadout>, ISerializationGenerated<Loadout>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<LoadoutPrototype> Prototype;

	public bool Equals(Loadout? other)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		return Prototype.Equals(other.Prototype);
	}

	public override bool Equals(object? obj)
	{
		if (this != obj)
		{
			if (obj is Loadout other)
			{
				return Equals(other);
			}
			return false;
		}
		return true;
	}

	public override int GetHashCode()
	{
		return ((object)Prototype/*cast due to constrained. prefix*/).GetHashCode();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Loadout target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<Loadout>(this, ref target, hookCtx, false, context))
		{
			ProtoId<LoadoutPrototype> PrototypeTemp = default(ProtoId<LoadoutPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<LoadoutPrototype>>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<ProtoId<LoadoutPrototype>>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Loadout target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Loadout cast = (Loadout)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public Loadout Instantiate()
	{
		return new Loadout();
	}
}
