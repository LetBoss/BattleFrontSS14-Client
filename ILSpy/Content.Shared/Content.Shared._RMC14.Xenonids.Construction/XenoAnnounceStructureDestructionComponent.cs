using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Construction;

[RegisterComponent]
public sealed class XenoAnnounceStructureDestructionComponent : Component, ISerializationGenerated<XenoAnnounceStructureDestructionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId MessageID = LocId.op_Implicit("rmc-xeno-construction-structure-destroyed");

	[DataField(null, false, 1, false, false, null)]
	public string? StructureName;

	[DataField(null, false, 1, false, false, null)]
	public string DestructionVerb = "destroyed";

	[DataField(null, false, 1, false, false, null)]
	public Color MessageColor = Color.FromHex((ReadOnlySpan<char>)"#2A623D", (Color?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoAnnounceStructureDestructionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoAnnounceStructureDestructionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoAnnounceStructureDestructionComponent>(this, ref target, hookCtx, false, context))
		{
			LocId MessageIDTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(MessageID, ref MessageIDTemp, hookCtx, false, context))
			{
				MessageIDTemp = serialization.CreateCopy<LocId>(MessageID, hookCtx, context, false);
			}
			target.MessageID = MessageIDTemp;
			string StructureNameTemp = null;
			if (!serialization.TryCustomCopy<string>(StructureName, ref StructureNameTemp, hookCtx, false, context))
			{
				StructureNameTemp = StructureName;
			}
			target.StructureName = StructureNameTemp;
			string DestructionVerbTemp = null;
			if (DestructionVerb == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(DestructionVerb, ref DestructionVerbTemp, hookCtx, false, context))
			{
				DestructionVerbTemp = DestructionVerb;
			}
			target.DestructionVerb = DestructionVerbTemp;
			Color MessageColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(MessageColor, ref MessageColorTemp, hookCtx, false, context))
			{
				MessageColorTemp = serialization.CreateCopy<Color>(MessageColor, hookCtx, context, false);
			}
			target.MessageColor = MessageColorTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoAnnounceStructureDestructionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAnnounceStructureDestructionComponent cast = (XenoAnnounceStructureDestructionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAnnounceStructureDestructionComponent cast = (XenoAnnounceStructureDestructionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoAnnounceStructureDestructionComponent def = (XenoAnnounceStructureDestructionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoAnnounceStructureDestructionComponent Instantiate()
	{
		return new XenoAnnounceStructureDestructionComponent();
	}
}
