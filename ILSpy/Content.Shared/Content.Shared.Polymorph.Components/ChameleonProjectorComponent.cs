using System;
using Content.Shared.Polymorph.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Polymorph.Components;

[RegisterComponent]
[Access(new Type[] { typeof(SharedChameleonProjectorSystem) })]
public sealed class ChameleonProjectorComponent : Component, ISerializationGenerated<ChameleonProjectorComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField(null, false, 1, true, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId DisguiseProto = EntProtoId.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId NoRotAction = EntProtoId.op_Implicit("ActionDisguiseNoRot");

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? NoRotActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId AnchorAction = EntProtoId.op_Implicit("ActionDisguiseAnchor");

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? AnchorActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public float MinHealth = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float MaxHealth = 100f;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Disguised;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChameleonProjectorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChameleonProjectorComponent)(object)definitionCast;
		if (serialization.TryCustomCopy<ChameleonProjectorComponent>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		target.Blacklist = BlacklistTemp;
		EntProtoId DisguiseProtoTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(DisguiseProto, ref DisguiseProtoTemp, hookCtx, false, context))
		{
			DisguiseProtoTemp = serialization.CreateCopy<EntProtoId>(DisguiseProto, hookCtx, context, false);
		}
		target.DisguiseProto = DisguiseProtoTemp;
		EntProtoId NoRotActionTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(NoRotAction, ref NoRotActionTemp, hookCtx, false, context))
		{
			NoRotActionTemp = serialization.CreateCopy<EntProtoId>(NoRotAction, hookCtx, context, false);
		}
		target.NoRotAction = NoRotActionTemp;
		EntityUid? NoRotActionEntityTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(NoRotActionEntity, ref NoRotActionEntityTemp, hookCtx, false, context))
		{
			NoRotActionEntityTemp = serialization.CreateCopy<EntityUid?>(NoRotActionEntity, hookCtx, context, false);
		}
		target.NoRotActionEntity = NoRotActionEntityTemp;
		EntProtoId AnchorActionTemp = default(EntProtoId);
		if (!serialization.TryCustomCopy<EntProtoId>(AnchorAction, ref AnchorActionTemp, hookCtx, false, context))
		{
			AnchorActionTemp = serialization.CreateCopy<EntProtoId>(AnchorAction, hookCtx, context, false);
		}
		target.AnchorAction = AnchorActionTemp;
		EntityUid? AnchorActionEntityTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(AnchorActionEntity, ref AnchorActionEntityTemp, hookCtx, false, context))
		{
			AnchorActionEntityTemp = serialization.CreateCopy<EntityUid?>(AnchorActionEntity, hookCtx, context, false);
		}
		target.AnchorActionEntity = AnchorActionEntityTemp;
		float MinHealthTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MinHealth, ref MinHealthTemp, hookCtx, false, context))
		{
			MinHealthTemp = MinHealth;
		}
		target.MinHealth = MinHealthTemp;
		float MaxHealthTemp = 0f;
		if (!serialization.TryCustomCopy<float>(MaxHealth, ref MaxHealthTemp, hookCtx, false, context))
		{
			MaxHealthTemp = MaxHealth;
		}
		target.MaxHealth = MaxHealthTemp;
		EntityUid? DisguisedTemp = null;
		if (!serialization.TryCustomCopy<EntityUid?>(Disguised, ref DisguisedTemp, hookCtx, false, context))
		{
			DisguisedTemp = serialization.CreateCopy<EntityUid?>(Disguised, hookCtx, context, false);
		}
		target.Disguised = DisguisedTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChameleonProjectorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChameleonProjectorComponent cast = (ChameleonProjectorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChameleonProjectorComponent cast = (ChameleonProjectorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChameleonProjectorComponent def = (ChameleonProjectorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChameleonProjectorComponent Instantiate()
	{
		return new ChameleonProjectorComponent();
	}
}
