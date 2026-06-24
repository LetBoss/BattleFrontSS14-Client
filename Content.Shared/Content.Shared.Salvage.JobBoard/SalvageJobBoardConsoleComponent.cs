using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Salvage.JobBoard;

[RegisterComponent]
[NetworkedComponent]
public sealed class SalvageJobBoardConsoleComponent : Component, ISerializationGenerated<SalvageJobBoardConsoleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId LabelEntity = EntProtoId.op_Implicit("PaperSalvageJobLabel");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier PrintSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Machines/printer.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, typeof(TimeOffsetSerializer))]
	public TimeSpan NextPrintTime = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan PrintDelay = TimeSpan.FromSeconds(5L);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SalvageJobBoardConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SalvageJobBoardConsoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SalvageJobBoardConsoleComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId LabelEntityTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(LabelEntity, ref LabelEntityTemp, hookCtx, false, context))
			{
				LabelEntityTemp = serialization.CreateCopy<EntProtoId>(LabelEntity, hookCtx, context, false);
			}
			target.LabelEntity = LabelEntityTemp;
			SoundSpecifier PrintSoundTemp = null;
			if (PrintSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(PrintSound, ref PrintSoundTemp, hookCtx, true, context))
			{
				PrintSoundTemp = serialization.CreateCopy<SoundSpecifier>(PrintSound, hookCtx, context, false);
			}
			target.PrintSound = PrintSoundTemp;
			TimeSpan NextPrintTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NextPrintTime, ref NextPrintTimeTemp, hookCtx, false, context))
			{
				NextPrintTimeTemp = serialization.CreateCopy<TimeSpan>(NextPrintTime, hookCtx, context, false);
			}
			target.NextPrintTime = NextPrintTimeTemp;
			TimeSpan PrintDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(PrintDelay, ref PrintDelayTemp, hookCtx, false, context))
			{
				PrintDelayTemp = serialization.CreateCopy<TimeSpan>(PrintDelay, hookCtx, context, false);
			}
			target.PrintDelay = PrintDelayTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SalvageJobBoardConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageJobBoardConsoleComponent cast = (SalvageJobBoardConsoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageJobBoardConsoleComponent cast = (SalvageJobBoardConsoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SalvageJobBoardConsoleComponent def = (SalvageJobBoardConsoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SalvageJobBoardConsoleComponent Instantiate()
	{
		return new SalvageJobBoardConsoleComponent();
	}
}
