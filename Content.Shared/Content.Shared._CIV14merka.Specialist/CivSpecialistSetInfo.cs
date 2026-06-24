using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;

namespace Content.Shared._CIV14merka.Specialist;

[Serializable]
[NetSerializable]
[DataDefinition]
public struct CivSpecialistSetInfo : ISerializationGenerated<CivSpecialistSetInfo>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Name;

	[DataField(null, false, 1, false, false, null)]
	public string Description;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier Sprite;

	public bool Selected;

	public CivSpecialistSetInfo(string name, string desc, SpriteSpecifier sprite, bool selected)
	{
		Name = name;
		Description = desc;
		Sprite = sprite;
		Selected = selected;
	}

	public CivSpecialistSetInfo()
	{
		Name = null;
		Description = null;
		Sprite = null;
		Selected = false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivSpecialistSetInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<CivSpecialistSetInfo>(this, ref target, hookCtx, false, context))
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
			string DescriptionTemp = null;
			if (Description == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = Description;
			}
			SpriteSpecifier SpriteTemp = null;
			if (Sprite == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Sprite, ref SpriteTemp, hookCtx, true, context))
			{
				SpriteTemp = serialization.CreateCopy<SpriteSpecifier>(Sprite, hookCtx, context, false);
			}
			CivSpecialistSetInfo civSpecialistSetInfo = target;
			civSpecialistSetInfo.Name = NameTemp;
			civSpecialistSetInfo.Description = DescriptionTemp;
			civSpecialistSetInfo.Sprite = SpriteTemp;
			target = civSpecialistSetInfo;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivSpecialistSetInfo target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSpecialistSetInfo cast = (CivSpecialistSetInfo)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public CivSpecialistSetInfo Instantiate()
	{
		return new CivSpecialistSetInfo();
	}
}
