using System;
using System.Collections.Generic;
using Content.Shared.Chat.Prototypes;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Xenonids.Neurotoxin;

[RegisterComponent]
[NetworkedComponent]
public sealed class NeurotoxinLingeringHallucinationComponent : Component, ISerializationGenerated<NeurotoxinLingeringHallucinationComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> Hallucinations = new List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>();

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier BoneBreak = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Weapons/alien_knockdown.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier XenoClaw = (SoundSpecifier)new SoundCollectionSpecifier("AlienClaw", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier OBTravel = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_orbital_travel.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier MortarTravel = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Weapons/gun_mortar_travel.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier GauFire = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Dropship/gau.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier RocketFire = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Effects/rocketpod_fire.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier GauHit = (SoundSpecifier)new SoundPathSpecifier("/Audio/_RMC14/Dropship/gauimpact.ogg", (AudioParams?)((AudioParams)(ref AudioParams.Default)).WithVolume(-5f));

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier Explosion = (SoundSpecifier)new SoundCollectionSpecifier("CMExplosion", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier BigExplosion = (SoundSpecifier)new SoundCollectionSpecifier("RMCExplosionBig", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<EmotePrototype> PainEmote = ProtoId<EmotePrototype>.op_Implicit("Scream");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NeurotoxinLingeringHallucinationComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NeurotoxinLingeringHallucinationComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<NeurotoxinLingeringHallucinationComponent>(this, ref target, hookCtx, false, context))
		{
			List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)> HallucinationsTemp = null;
			if (Hallucinations == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>>(Hallucinations, ref HallucinationsTemp, hookCtx, true, context))
			{
				HallucinationsTemp = serialization.CreateCopy<List<(NeuroHallucinations, int, TimeSpan, EntityCoordinates?)>>(Hallucinations, hookCtx, context, false);
			}
			target.Hallucinations = HallucinationsTemp;
			SoundSpecifier BoneBreakTemp = null;
			if (BoneBreak == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(BoneBreak, ref BoneBreakTemp, hookCtx, true, context))
			{
				BoneBreakTemp = serialization.CreateCopy<SoundSpecifier>(BoneBreak, hookCtx, context, false);
			}
			target.BoneBreak = BoneBreakTemp;
			SoundSpecifier XenoClawTemp = null;
			if (XenoClaw == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(XenoClaw, ref XenoClawTemp, hookCtx, true, context))
			{
				XenoClawTemp = serialization.CreateCopy<SoundSpecifier>(XenoClaw, hookCtx, context, false);
			}
			target.XenoClaw = XenoClawTemp;
			SoundSpecifier OBTravelTemp = null;
			if (OBTravel == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(OBTravel, ref OBTravelTemp, hookCtx, true, context))
			{
				OBTravelTemp = serialization.CreateCopy<SoundSpecifier>(OBTravel, hookCtx, context, false);
			}
			target.OBTravel = OBTravelTemp;
			SoundSpecifier MortarTravelTemp = null;
			if (MortarTravel == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(MortarTravel, ref MortarTravelTemp, hookCtx, true, context))
			{
				MortarTravelTemp = serialization.CreateCopy<SoundSpecifier>(MortarTravel, hookCtx, context, false);
			}
			target.MortarTravel = MortarTravelTemp;
			SoundSpecifier GauFireTemp = null;
			if (GauFire == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(GauFire, ref GauFireTemp, hookCtx, true, context))
			{
				GauFireTemp = serialization.CreateCopy<SoundSpecifier>(GauFire, hookCtx, context, false);
			}
			target.GauFire = GauFireTemp;
			SoundSpecifier RocketFireTemp = null;
			if (RocketFire == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(RocketFire, ref RocketFireTemp, hookCtx, true, context))
			{
				RocketFireTemp = serialization.CreateCopy<SoundSpecifier>(RocketFire, hookCtx, context, false);
			}
			target.RocketFire = RocketFireTemp;
			SoundSpecifier GauHitTemp = null;
			if (GauHit == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(GauHit, ref GauHitTemp, hookCtx, true, context))
			{
				GauHitTemp = serialization.CreateCopy<SoundSpecifier>(GauHit, hookCtx, context, false);
			}
			target.GauHit = GauHitTemp;
			SoundSpecifier ExplosionTemp = null;
			if (Explosion == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(Explosion, ref ExplosionTemp, hookCtx, true, context))
			{
				ExplosionTemp = serialization.CreateCopy<SoundSpecifier>(Explosion, hookCtx, context, false);
			}
			target.Explosion = ExplosionTemp;
			SoundSpecifier BigExplosionTemp = null;
			if (BigExplosion == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(BigExplosion, ref BigExplosionTemp, hookCtx, true, context))
			{
				BigExplosionTemp = serialization.CreateCopy<SoundSpecifier>(BigExplosion, hookCtx, context, false);
			}
			target.BigExplosion = BigExplosionTemp;
			ProtoId<EmotePrototype> PainEmoteTemp = default(ProtoId<EmotePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<EmotePrototype>>(PainEmote, ref PainEmoteTemp, hookCtx, false, context))
			{
				PainEmoteTemp = serialization.CreateCopy<ProtoId<EmotePrototype>>(PainEmote, hookCtx, context, false);
			}
			target.PainEmote = PainEmoteTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NeurotoxinLingeringHallucinationComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NeurotoxinLingeringHallucinationComponent cast = (NeurotoxinLingeringHallucinationComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NeurotoxinLingeringHallucinationComponent cast = (NeurotoxinLingeringHallucinationComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NeurotoxinLingeringHallucinationComponent def = (NeurotoxinLingeringHallucinationComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NeurotoxinLingeringHallucinationComponent Instantiate()
	{
		return new NeurotoxinLingeringHallucinationComponent();
	}
}
