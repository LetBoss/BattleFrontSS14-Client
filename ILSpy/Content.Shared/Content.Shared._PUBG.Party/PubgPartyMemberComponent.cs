using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Party;

[RegisterComponent]
public sealed class PubgPartyMemberComponent : Component, ISerializationGenerated<PubgPartyMemberComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int PartyId;

	[DataField(null, false, 1, false, false, null)]
	public int SlotIndex;

	[DataField(null, false, 1, false, false, null)]
	public int MaxTeamSize;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgPartyMemberComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgPartyMemberComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgPartyMemberComponent>(this, ref target, hookCtx, false, context))
		{
			int PartyIdTemp = 0;
			if (!serialization.TryCustomCopy<int>(PartyId, ref PartyIdTemp, hookCtx, false, context))
			{
				PartyIdTemp = PartyId;
			}
			target.PartyId = PartyIdTemp;
			int SlotIndexTemp = 0;
			if (!serialization.TryCustomCopy<int>(SlotIndex, ref SlotIndexTemp, hookCtx, false, context))
			{
				SlotIndexTemp = SlotIndex;
			}
			target.SlotIndex = SlotIndexTemp;
			int MaxTeamSizeTemp = 0;
			if (!serialization.TryCustomCopy<int>(MaxTeamSize, ref MaxTeamSizeTemp, hookCtx, false, context))
			{
				MaxTeamSizeTemp = MaxTeamSize;
			}
			target.MaxTeamSize = MaxTeamSizeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgPartyMemberComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgPartyMemberComponent cast = (PubgPartyMemberComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgPartyMemberComponent cast = (PubgPartyMemberComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgPartyMemberComponent def = (PubgPartyMemberComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgPartyMemberComponent Instantiate()
	{
		return new PubgPartyMemberComponent();
	}
}
