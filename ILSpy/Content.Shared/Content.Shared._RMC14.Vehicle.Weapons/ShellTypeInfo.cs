using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle.Weapons;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class ShellTypeInfo : ISerializationGenerated<ShellTypeInfo>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntProtoId ProtoId;

	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string Description = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string SpriteState = string.Empty;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ShellTypeInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ShellTypeInfo>(this, ref target, hookCtx, false, context))
		{
			EntProtoId ProtoIdTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(ProtoId, ref ProtoIdTemp, hookCtx, false, context))
			{
				ProtoIdTemp = serialization.CreateCopy<EntProtoId>(ProtoId, hookCtx, context, false);
			}
			target.ProtoId = ProtoIdTemp;
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
			string DescriptionTemp = null;
			if (Description == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = Description;
			}
			target.Description = DescriptionTemp;
			string SpriteStateTemp = null;
			if (SpriteState == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SpriteState, ref SpriteStateTemp, hookCtx, false, context))
			{
				SpriteStateTemp = SpriteState;
			}
			target.SpriteState = SpriteStateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ShellTypeInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShellTypeInfo cast = (ShellTypeInfo)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ShellTypeInfo Instantiate()
	{
		return new ShellTypeInfo();
	}
}
