using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Weapons.Misc;

public abstract class BaseForceGunComponent : Component, ISerializationGenerated<BaseForceGunComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("lineColor", false, 1, false, false, null)]
	[AutoNetworkedField]
	public Color LineColor = Color.Orange;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("canUnanchor", false, 1, false, false, null)]
	[AutoNetworkedField]
	public bool CanUnanchor;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("canTetherAlive", false, 1, false, false, null)]
	[AutoNetworkedField]
	public bool CanTetherAlive;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("maxForce", false, 1, false, false, null)]
	[AutoNetworkedField]
	public float MaxForce = 200f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("frequency", false, 1, false, false, null)]
	[AutoNetworkedField]
	public float Frequency = 10f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("dampingRatio", false, 1, false, false, null)]
	[AutoNetworkedField]
	public float DampingRatio = 2f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("massLimit", false, 1, false, false, null)]
	[AutoNetworkedField]
	public float MassLimit = 100f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("sound", false, 1, false, false, null)]
	[AutoNetworkedField]
	public SoundSpecifier? Sound;

	public EntityUid? Stream;

	[DataField("tetherEntity", false, 1, false, false, null)]
	[AutoNetworkedField]
	public virtual EntityUid? TetherEntity { get; set; }

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("tethered", false, 1, false, false, null)]
	[AutoNetworkedField]
	public virtual EntityUid? Tethered { get; set; }

	public BaseForceGunComponent()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		SoundPathSpecifier val = new SoundPathSpecifier("/Audio/Weapons/weoweo.ogg", (AudioParams?)null);
		AudioParams val2 = ((AudioParams)(ref AudioParams.Default)).WithLoop(true);
		((SoundSpecifier)val).Params = ((AudioParams)(ref val2)).WithVolume(-8f);
		Sound = (SoundSpecifier?)val;
		((Component)this)._002Ector();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref BaseForceGunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
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
		target = (BaseForceGunComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<BaseForceGunComponent>(this, ref target, hookCtx, false, context))
		{
			Color LineColorTemp = default(Color);
			if (!serialization.TryCustomCopy<Color>(LineColor, ref LineColorTemp, hookCtx, false, context))
			{
				LineColorTemp = serialization.CreateCopy<Color>(LineColor, hookCtx, context, false);
			}
			target.LineColor = LineColorTemp;
			EntityUid? TetherEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(TetherEntity, ref TetherEntityTemp, hookCtx, false, context))
			{
				TetherEntityTemp = serialization.CreateCopy<EntityUid?>(TetherEntity, hookCtx, context, false);
			}
			target.TetherEntity = TetherEntityTemp;
			EntityUid? TetheredTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Tethered, ref TetheredTemp, hookCtx, false, context))
			{
				TetheredTemp = serialization.CreateCopy<EntityUid?>(Tethered, hookCtx, context, false);
			}
			target.Tethered = TetheredTemp;
			bool CanUnanchorTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanUnanchor, ref CanUnanchorTemp, hookCtx, false, context))
			{
				CanUnanchorTemp = CanUnanchor;
			}
			target.CanUnanchor = CanUnanchorTemp;
			bool CanTetherAliveTemp = false;
			if (!serialization.TryCustomCopy<bool>(CanTetherAlive, ref CanTetherAliveTemp, hookCtx, false, context))
			{
				CanTetherAliveTemp = CanTetherAlive;
			}
			target.CanTetherAlive = CanTetherAliveTemp;
			float MaxForceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxForce, ref MaxForceTemp, hookCtx, false, context))
			{
				MaxForceTemp = MaxForce;
			}
			target.MaxForce = MaxForceTemp;
			float FrequencyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Frequency, ref FrequencyTemp, hookCtx, false, context))
			{
				FrequencyTemp = Frequency;
			}
			target.Frequency = FrequencyTemp;
			float DampingRatioTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DampingRatio, ref DampingRatioTemp, hookCtx, false, context))
			{
				DampingRatioTemp = DampingRatio;
			}
			target.DampingRatio = DampingRatioTemp;
			float MassLimitTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MassLimit, ref MassLimitTemp, hookCtx, false, context))
			{
				MassLimitTemp = MassLimit;
			}
			target.MassLimit = MassLimitTemp;
			SoundSpecifier SoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(Sound, ref SoundTemp, hookCtx, true, context))
			{
				SoundTemp = serialization.CreateCopy<SoundSpecifier>(Sound, hookCtx, context, false);
			}
			target.Sound = SoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref BaseForceGunComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseForceGunComponent cast = (BaseForceGunComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseForceGunComponent cast = (BaseForceGunComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		BaseForceGunComponent def = (BaseForceGunComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override BaseForceGunComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
