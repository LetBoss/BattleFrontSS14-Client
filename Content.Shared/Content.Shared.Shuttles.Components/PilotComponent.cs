using System;
using System.Numerics;
using Content.Shared.Alert;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Shuttles.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PilotComponent : Component, ISerializationGenerated<PilotComponent>, ISerializationGenerated
{
	public Vector2 CurTickStrafeMovement = Vector2.Zero;

	public float CurTickRotationMovement;

	public float CurTickBraking;

	public GameTick LastInputTick = GameTick.Zero;

	public ushort LastInputSubTick;

	[ViewVariables]
	public ShuttleButtons HeldButtons;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<AlertPrototype> PilotingAlert = ProtoId<AlertPrototype>.op_Implicit("PilotingShuttle");

	[ViewVariables]
	public EntityUid? Console { get; set; }

	[ViewVariables]
	public EntityCoordinates? Position { get; set; }

	public override bool SendOnlyToOwner => true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PilotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (PilotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PilotComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<AlertPrototype> PilotingAlertTemp = default(ProtoId<AlertPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<AlertPrototype>>(PilotingAlert, ref PilotingAlertTemp, hookCtx, false, context))
			{
				PilotingAlertTemp = serialization.CreateCopy<ProtoId<AlertPrototype>>(PilotingAlert, hookCtx, context, false);
			}
			target.PilotingAlert = PilotingAlertTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PilotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PilotComponent cast = (PilotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PilotComponent cast = (PilotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PilotComponent def = (PilotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PilotComponent Instantiate()
	{
		return new PilotComponent();
	}
}
