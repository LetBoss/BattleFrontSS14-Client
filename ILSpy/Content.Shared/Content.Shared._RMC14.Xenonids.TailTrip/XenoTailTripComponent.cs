using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.TailTrip;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(XenoTailTripSystem) })]
public sealed class XenoTailTripComponent : Component, ISerializationGenerated<XenoTailTripComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int PlasmaCost = 30;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan SlowTime = TimeSpan.FromSeconds(0.3);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan StunTime = TimeSpan.FromSeconds(0.2);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MarkedStunTime = TimeSpan.FromSeconds(2L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan MarkedDazeTime = TimeSpan.FromSeconds(4L);

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId TailEffect = EntProtoId.op_Implicit("RMCEffectDisarm");

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier Sound = (SoundSpecifier)new SoundCollectionSpecifier("XenoTailSwipe", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoTailTripComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoTailTripComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoTailTripComponent>(this, ref target, hookCtx, false, context))
		{
			int PlasmaCostTemp = 0;
			if (!serialization.TryCustomCopy<int>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = PlasmaCost;
			}
			target.PlasmaCost = PlasmaCostTemp;
			TimeSpan SlowTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(SlowTime, ref SlowTimeTemp, hookCtx, false, context))
			{
				SlowTimeTemp = serialization.CreateCopy<TimeSpan>(SlowTime, hookCtx, context, false);
			}
			target.SlowTime = SlowTimeTemp;
			TimeSpan StunTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(StunTime, ref StunTimeTemp, hookCtx, false, context))
			{
				StunTimeTemp = serialization.CreateCopy<TimeSpan>(StunTime, hookCtx, context, false);
			}
			target.StunTime = StunTimeTemp;
			TimeSpan MarkedStunTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MarkedStunTime, ref MarkedStunTimeTemp, hookCtx, false, context))
			{
				MarkedStunTimeTemp = serialization.CreateCopy<TimeSpan>(MarkedStunTime, hookCtx, context, false);
			}
			target.MarkedStunTime = MarkedStunTimeTemp;
			TimeSpan MarkedDazeTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(MarkedDazeTime, ref MarkedDazeTimeTemp, hookCtx, false, context))
			{
				MarkedDazeTimeTemp = serialization.CreateCopy<TimeSpan>(MarkedDazeTime, hookCtx, context, false);
			}
			target.MarkedDazeTime = MarkedDazeTimeTemp;
			EntProtoId TailEffectTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(TailEffect, ref TailEffectTemp, hookCtx, false, context))
			{
				TailEffectTemp = serialization.CreateCopy<EntProtoId>(TailEffect, hookCtx, context, false);
			}
			target.TailEffect = TailEffectTemp;
			SoundSpecifier SoundTemp = null;
			if (Sound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoTailTripComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTailTripComponent cast = (XenoTailTripComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTailTripComponent cast = (XenoTailTripComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTailTripComponent def = (XenoTailTripComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoTailTripComponent Instantiate()
	{
		return new XenoTailTripComponent();
	}
}
