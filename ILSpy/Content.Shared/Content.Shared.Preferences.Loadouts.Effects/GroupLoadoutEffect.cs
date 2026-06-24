using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed class GroupLoadoutEffect : LoadoutEffect, ISerializationGenerated<GroupLoadoutEffect>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<LoadoutEffectGroupPrototype> Proto;

	public override bool Validate(HumanoidCharacterProfile profile, RoleLoadout loadout, ICommonSession? session, IDependencyCollection collection, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		LoadoutEffectGroupPrototype loadoutEffectGroupPrototype = collection.Resolve<IPrototypeManager>().Index<LoadoutEffectGroupPrototype>(Proto);
		List<string> reasons = new List<string>();
		foreach (LoadoutEffect effect in loadoutEffectGroupPrototype.Effects)
		{
			if (!effect.Validate(profile, loadout, session, collection, out reason))
			{
				reasons.Add(reason.ToMarkup());
			}
		}
		reason = ((reasons.Count == 0) ? null : FormattedMessage.FromMarkupOrThrow(string.Join('\n', reasons)));
		return reason == null;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GroupLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		LoadoutEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GroupLoadoutEffect)definitionCast;
		if (!serialization.TryCustomCopy<GroupLoadoutEffect>(this, ref target, hookCtx, false, context))
		{
			ProtoId<LoadoutEffectGroupPrototype> ProtoTemp = default(ProtoId<LoadoutEffectGroupPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<LoadoutEffectGroupPrototype>>(Proto, ref ProtoTemp, hookCtx, false, context))
			{
				ProtoTemp = serialization.CreateCopy<ProtoId<LoadoutEffectGroupPrototype>>(Proto, hookCtx, context, false);
			}
			target.Proto = ProtoTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GroupLoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref LoadoutEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupLoadoutEffect cast = (GroupLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupLoadoutEffect cast = (GroupLoadoutEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GroupLoadoutEffect Instantiate()
	{
		return new GroupLoadoutEffect();
	}
}
