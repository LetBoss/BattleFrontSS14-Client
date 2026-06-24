using System;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ScoopableSolutionSystem) })]
public sealed class ScoopableSolutionComponent : Component, ISerializationGenerated<ScoopableSolutionComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Solution = "default";

	[DataField(null, false, 1, false, false, null)]
	public bool Delete = true;

	[DataField(null, false, 1, false, false, null)]
	public LocId Popup = LocId.op_Implicit("scoopable-component-popup");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ScoopableSolutionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ScoopableSolutionComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ScoopableSolutionComponent>(this, ref target, hookCtx, false, context))
		{
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
			bool DeleteTemp = false;
			if (!serialization.TryCustomCopy<bool>(Delete, ref DeleteTemp, hookCtx, false, context))
			{
				DeleteTemp = Delete;
			}
			target.Delete = DeleteTemp;
			LocId PopupTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Popup, ref PopupTemp, hookCtx, false, context))
			{
				PopupTemp = serialization.CreateCopy<LocId>(Popup, hookCtx, context, false);
			}
			target.Popup = PopupTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ScoopableSolutionComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ScoopableSolutionComponent cast = (ScoopableSolutionComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ScoopableSolutionComponent cast = (ScoopableSolutionComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ScoopableSolutionComponent def = (ScoopableSolutionComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ScoopableSolutionComponent Instantiate()
	{
		return new ScoopableSolutionComponent();
	}
}
