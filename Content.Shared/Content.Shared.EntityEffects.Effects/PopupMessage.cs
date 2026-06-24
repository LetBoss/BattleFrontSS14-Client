using System;
using System.Collections.Generic;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects;

public sealed class PopupMessage : EntityEffect, ISerializationGenerated<PopupMessage>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string[] Messages;

	[DataField(null, false, 1, false, false, null)]
	public PopupRecipients Type = PopupRecipients.Local;

	[DataField(null, false, 1, false, false, null)]
	public PopupType VisualType;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return null;
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		SharedPopupSystem popupSys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedPopupSystem>();
		string msg = RandomExtensions.Pick<string>(IoCManager.Resolve<IRobustRandom>(), (IReadOnlyList<string>)Messages);
		(string, object)[] msgArgs = new(string, object)[1] { ("entity", args.TargetEntity) };
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			msgArgs = new(string, object)[2]
			{
				("entity", reagentArgs.TargetEntity),
				("organ", reagentArgs.OrganEntity.GetValueOrDefault())
			};
		}
		if (Type == PopupRecipients.Local)
		{
			popupSys.PopupEntity(Loc.GetString(msg, msgArgs), args.TargetEntity, args.TargetEntity, VisualType);
		}
		else if (Type == PopupRecipients.Pvs)
		{
			popupSys.PopupEntity(Loc.GetString(msg, msgArgs), args.TargetEntity, VisualType);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PopupMessage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PopupMessage)definitionCast;
		if (!serialization.TryCustomCopy<PopupMessage>(this, ref target, hookCtx, false, context))
		{
			string[] MessagesTemp = null;
			if (Messages == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string[]>(Messages, ref MessagesTemp, hookCtx, true, context))
			{
				MessagesTemp = serialization.CreateCopy<string[]>(Messages, hookCtx, context, false);
			}
			target.Messages = MessagesTemp;
			PopupRecipients TypeTemp = PopupRecipients.Pvs;
			if (!serialization.TryCustomCopy<PopupRecipients>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = Type;
			}
			target.Type = TypeTemp;
			PopupType VisualTypeTemp = PopupType.Small;
			if (!serialization.TryCustomCopy<PopupType>(VisualType, ref VisualTypeTemp, hookCtx, false, context))
			{
				VisualTypeTemp = VisualType;
			}
			target.VisualType = VisualTypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PopupMessage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PopupMessage cast = (PopupMessage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PopupMessage cast = (PopupMessage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PopupMessage Instantiate()
	{
		return new PopupMessage();
	}
}
