using System;
using Content.Shared.Friends.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Friends.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(PettableFriendSystem) })]
public sealed class PettableFriendComponent : Component, ISerializationGenerated<PettableFriendComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public LocId SuccessString = LocId.op_Implicit(string.Empty);

	[DataField(null, false, 1, true, false, null)]
	public LocId FailureString = LocId.op_Implicit(string.Empty);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PettableFriendComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PettableFriendComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PettableFriendComponent>(this, ref target, hookCtx, false, context))
		{
			LocId SuccessStringTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(SuccessString, ref SuccessStringTemp, hookCtx, false, context))
			{
				SuccessStringTemp = serialization.CreateCopy<LocId>(SuccessString, hookCtx, context, false);
			}
			target.SuccessString = SuccessStringTemp;
			LocId FailureStringTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(FailureString, ref FailureStringTemp, hookCtx, false, context))
			{
				FailureStringTemp = serialization.CreateCopy<LocId>(FailureString, hookCtx, context, false);
			}
			target.FailureString = FailureStringTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PettableFriendComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PettableFriendComponent cast = (PettableFriendComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PettableFriendComponent cast = (PettableFriendComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PettableFriendComponent def = (PettableFriendComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PettableFriendComponent Instantiate()
	{
		return new PettableFriendComponent();
	}
}
