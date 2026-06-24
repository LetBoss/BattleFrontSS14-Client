using System;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(FactionClothingSystem) })]
public sealed class FactionClothingComponent : Component, ISerializationGenerated<FactionClothingComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<NpcFactionPrototype> Faction = ProtoId<NpcFactionPrototype>.op_Implicit(string.Empty);

	[DataField(null, false, 1, false, false, null)]
	public bool AlreadyMember;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FactionClothingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FactionClothingComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<FactionClothingComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<NpcFactionPrototype> FactionTemp = default(ProtoId<NpcFactionPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<NpcFactionPrototype>>(Faction, ref FactionTemp, hookCtx, false, context))
			{
				FactionTemp = serialization.CreateCopy<ProtoId<NpcFactionPrototype>>(Faction, hookCtx, context, false);
			}
			target.Faction = FactionTemp;
			bool AlreadyMemberTemp = false;
			if (!serialization.TryCustomCopy<bool>(AlreadyMember, ref AlreadyMemberTemp, hookCtx, false, context))
			{
				AlreadyMemberTemp = AlreadyMember;
			}
			target.AlreadyMember = AlreadyMemberTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FactionClothingComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionClothingComponent cast = (FactionClothingComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionClothingComponent cast = (FactionClothingComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FactionClothingComponent def = (FactionClothingComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FactionClothingComponent Instantiate()
	{
		return new FactionClothingComponent();
	}
}
