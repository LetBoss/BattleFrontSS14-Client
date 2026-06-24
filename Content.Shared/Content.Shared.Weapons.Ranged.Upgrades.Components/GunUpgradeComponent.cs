using System;
using System.Collections.Generic;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Weapons.Ranged.Upgrades.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(GunUpgradeSystem) })]
public sealed class GunUpgradeComponent : Component, ISerializationGenerated<GunUpgradeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<ProtoId<TagPrototype>> Tags = new List<ProtoId<TagPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public LocId ExamineText;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GunUpgradeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GunUpgradeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GunUpgradeComponent>(this, ref target, hookCtx, false, context))
		{
			List<ProtoId<TagPrototype>> TagsTemp = null;
			if (Tags == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<ProtoId<TagPrototype>>>(Tags, ref TagsTemp, hookCtx, true, context))
			{
				TagsTemp = serialization.CreateCopy<List<ProtoId<TagPrototype>>>(Tags, hookCtx, context, false);
			}
			target.Tags = TagsTemp;
			LocId ExamineTextTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(ExamineText, ref ExamineTextTemp, hookCtx, false, context))
			{
				ExamineTextTemp = serialization.CreateCopy<LocId>(ExamineText, hookCtx, context, false);
			}
			target.ExamineText = ExamineTextTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GunUpgradeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeComponent cast = (GunUpgradeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeComponent cast = (GunUpgradeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GunUpgradeComponent def = (GunUpgradeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GunUpgradeComponent Instantiate()
	{
		return new GunUpgradeComponent();
	}
}
