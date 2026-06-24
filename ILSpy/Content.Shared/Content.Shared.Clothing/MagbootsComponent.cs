using System;
using Content.Shared.Alert;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Clothing;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedMagbootsSystem) })]
public sealed class MagbootsComponent : Component, ISerializationGenerated<MagbootsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<AlertPrototype> MagbootsAlert = ProtoId<AlertPrototype>.op_Implicit("Magboots");

	[DataField(null, false, 1, false, false, null)]
	public bool RequiresGrid = true;

	[DataField(null, false, 1, false, false, null)]
	public string Slot = "shoes";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MagbootsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MagbootsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MagbootsComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<AlertPrototype> MagbootsAlertTemp = default(ProtoId<AlertPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(MagbootsAlert, ref MagbootsAlertTemp, hookCtx, false, context))
			{
				MagbootsAlertTemp = serialization.CreateCopy<ProtoId<AlertPrototype>>(MagbootsAlert, hookCtx, context, false);
			}
			target.MagbootsAlert = MagbootsAlertTemp;
			bool RequiresGridTemp = false;
			if (!serialization.TryCustomCopy<bool>(RequiresGrid, ref RequiresGridTemp, hookCtx, false, context))
			{
				RequiresGridTemp = RequiresGrid;
			}
			target.RequiresGrid = RequiresGridTemp;
			string SlotTemp = null;
			if (Slot == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Slot, ref SlotTemp, hookCtx, false, context))
			{
				SlotTemp = Slot;
			}
			target.Slot = SlotTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MagbootsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagbootsComponent cast = (MagbootsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagbootsComponent cast = (MagbootsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagbootsComponent def = (MagbootsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagbootsComponent Instantiate()
	{
		return new MagbootsComponent();
	}
}
