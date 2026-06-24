using System;
using Content.Shared.Tools.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Tools.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SimpleToolUsageSystem) })]
public sealed class SimpleToolUsageComponent : Component, ISerializationGenerated<SimpleToolUsageComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ToolQualityPrototype> Quality = ProtoId<ToolQualityPrototype>.op_Implicit("Slicing");

	[DataField(null, false, 1, false, false, null)]
	public float DoAfter = 5f;

	[DataField(null, false, 1, false, false, null)]
	public LocId? UsageVerb;

	[DataField(null, false, 1, false, false, null)]
	public LocId BlockedMessage = LocId.op_Implicit("simple-tool-usage-blocked-message");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SimpleToolUsageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SimpleToolUsageComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SimpleToolUsageComponent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ToolQualityPrototype> QualityTemp = default(ProtoId<ToolQualityPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(Quality, ref QualityTemp, hookCtx, false, context))
			{
				QualityTemp = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(Quality, hookCtx, context, false);
			}
			target.Quality = QualityTemp;
			float DoAfterTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DoAfter, ref DoAfterTemp, hookCtx, false, context))
			{
				DoAfterTemp = DoAfter;
			}
			target.DoAfter = DoAfterTemp;
			LocId? UsageVerbTemp = null;
			if (!serialization.TryCustomCopy<LocId?>(UsageVerb, ref UsageVerbTemp, hookCtx, false, context))
			{
				UsageVerbTemp = serialization.CreateCopy<LocId?>(UsageVerb, hookCtx, context, false);
			}
			target.UsageVerb = UsageVerbTemp;
			LocId BlockedMessageTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(BlockedMessage, ref BlockedMessageTemp, hookCtx, false, context))
			{
				BlockedMessageTemp = serialization.CreateCopy<LocId>(BlockedMessage, hookCtx, context, false);
			}
			target.BlockedMessage = BlockedMessageTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SimpleToolUsageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleToolUsageComponent cast = (SimpleToolUsageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleToolUsageComponent cast = (SimpleToolUsageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleToolUsageComponent def = (SimpleToolUsageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SimpleToolUsageComponent Instantiate()
	{
		return new SimpleToolUsageComponent();
	}
}
