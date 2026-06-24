using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG.Sponsor;

[RegisterComponent]
[NetworkedComponent]
public sealed class PubgSponsorSandboxComponent : Component, ISerializationGenerated<PubgSponsorSandboxComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<string> Ckeys = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, List<string>> Permissions = new Dictionary<string, List<string>>();

	[DataField(null, false, 1, false, false, null)]
	public List<string> DisallowedEntityIds = new List<string>();

	[DataField(null, false, 1, false, false, null)]
	public bool BlockEraseMinds;

	[DataField(null, false, 1, false, false, null)]
	public bool IsMiniGameSandbox;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgSponsorSandboxComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgSponsorSandboxComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgSponsorSandboxComponent>(this, ref target, hookCtx, false, context))
		{
			List<string> CkeysTemp = null;
			if (Ckeys == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(Ckeys, ref CkeysTemp, hookCtx, true, context))
			{
				CkeysTemp = serialization.CreateCopy<List<string>>(Ckeys, hookCtx, context, false);
			}
			target.Ckeys = CkeysTemp;
			Dictionary<string, List<string>> PermissionsTemp = null;
			if (Permissions == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(Permissions, ref PermissionsTemp, hookCtx, true, context))
			{
				PermissionsTemp = serialization.CreateCopy<Dictionary<string, List<string>>>(Permissions, hookCtx, context, false);
			}
			target.Permissions = PermissionsTemp;
			List<string> DisallowedEntityIdsTemp = null;
			if (DisallowedEntityIds == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(DisallowedEntityIds, ref DisallowedEntityIdsTemp, hookCtx, true, context))
			{
				DisallowedEntityIdsTemp = serialization.CreateCopy<List<string>>(DisallowedEntityIds, hookCtx, context, false);
			}
			target.DisallowedEntityIds = DisallowedEntityIdsTemp;
			bool BlockEraseMindsTemp = false;
			if (!serialization.TryCustomCopy<bool>(BlockEraseMinds, ref BlockEraseMindsTemp, hookCtx, false, context))
			{
				BlockEraseMindsTemp = BlockEraseMinds;
			}
			target.BlockEraseMinds = BlockEraseMindsTemp;
			bool IsMiniGameSandboxTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsMiniGameSandbox, ref IsMiniGameSandboxTemp, hookCtx, false, context))
			{
				IsMiniGameSandboxTemp = IsMiniGameSandbox;
			}
			target.IsMiniGameSandbox = IsMiniGameSandboxTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgSponsorSandboxComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSponsorSandboxComponent cast = (PubgSponsorSandboxComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSponsorSandboxComponent cast = (PubgSponsorSandboxComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgSponsorSandboxComponent def = (PubgSponsorSandboxComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgSponsorSandboxComponent Instantiate()
	{
		return new PubgSponsorSandboxComponent();
	}
}
