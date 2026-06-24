using System.Collections.Generic;
using Content.Shared._RMC14.CCVar;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Voicelines;

public sealed class HumanoidVoicelinesSystem : EntitySystem
{
	[Dependency]
	private INetConfigurationManager _config;

	private static readonly ProtoId<SpeciesPrototype> ArachnidSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Arachnid");

	private static readonly ProtoId<SpeciesPrototype> DionaSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Diona");

	private static readonly ProtoId<SpeciesPrototype> DwarfSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Dwarf");

	private static readonly ProtoId<SpeciesPrototype> FelinidSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Felinid");

	private static readonly ProtoId<SpeciesPrototype> HumanSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Human");

	private static readonly ProtoId<SpeciesPrototype> MothSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Moth");

	private static readonly ProtoId<SpeciesPrototype> ReptilianSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Reptilian");

	private static readonly ProtoId<SpeciesPrototype> SlimeSpecies = ProtoId<SpeciesPrototype>.op_Implicit("SlimePerson");

	private static readonly ProtoId<SpeciesPrototype> AvaliSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Avali");

	private static readonly ProtoId<SpeciesPrototype> VulpkaninSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Vulpkanin");

	private static readonly ProtoId<SpeciesPrototype> RodentiaSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Rodentia");

	private static readonly ProtoId<SpeciesPrototype> FeroxiSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Feroxi");

	private static readonly ProtoId<SpeciesPrototype> SkrellSpecies = ProtoId<SpeciesPrototype>.op_Implicit("Skrell");

	private readonly Dictionary<ProtoId<SpeciesPrototype>, CVarDef> _voicelineCVars = new Dictionary<ProtoId<SpeciesPrototype>, CVarDef>
	{
		[ArachnidSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesArachnid,
		[DionaSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesDiona,
		[DwarfSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesDwarf,
		[FelinidSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesFelinid,
		[HumanSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesHuman,
		[MothSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesMoth,
		[ReptilianSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesReptilian,
		[SlimeSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesSlime,
		[AvaliSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesAvali,
		[VulpkaninSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesVulpkanin,
		[RodentiaSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesRodentia,
		[FeroxiSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesFeroxi,
		[SkrellSpecies] = (CVarDef)(object)RMCCVars.RMCPlayVoicelinesSkrell
	};

	private readonly Dictionary<ProtoId<SpeciesPrototype>, CVarDef> _emoteCVars = new Dictionary<ProtoId<SpeciesPrototype>, CVarDef>
	{
		[ArachnidSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesArachnid,
		[DionaSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesDiona,
		[DwarfSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesDwarf,
		[FelinidSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesFelinid,
		[HumanSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesHuman,
		[MothSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesMoth,
		[ReptilianSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesReptilian,
		[SlimeSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesSlime,
		[AvaliSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesAvali,
		[VulpkaninSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesVulpkanin,
		[RodentiaSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesRodentia,
		[FeroxiSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesFeroxi,
		[SkrellSpecies] = (CVarDef)(object)RMCCVars.RMCPlayEmotesSkrell
	};

	private EntityQuery<HumanoidAppearanceComponent> _humanoidAppearanceQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_humanoidAppearanceQuery = ((EntitySystem)this).GetEntityQuery<HumanoidAppearanceComponent>();
	}

	public bool ShouldPlayVoiceline(Entity<HumanoidAppearanceComponent?> vocalizer, ICommonSession forPlayer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = forPlayer.AttachedEntity;
		EntityUid val = Entity<HumanoidAppearanceComponent>.op_Implicit(vocalizer);
		if (attachedEntity.HasValue && attachedEntity.GetValueOrDefault() == val && !_config.GetClientCVar<bool>(forPlayer.Channel, RMCCVars.RMCPlayVoicelinesYourself))
		{
			return false;
		}
		if (!_humanoidAppearanceQuery.Resolve(Entity<HumanoidAppearanceComponent>.op_Implicit(vocalizer), ref vocalizer.Comp, false) || !_voicelineCVars.TryGetValue(vocalizer.Comp.Species, out CVarDef play))
		{
			return true;
		}
		return _config.GetClientCVar<bool>(forPlayer.Channel, play.Name);
	}

	public bool ShouldPlayEmote(Entity<HumanoidAppearanceComponent?> vocalizer, ICommonSession forPlayer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = forPlayer.AttachedEntity;
		EntityUid val = Entity<HumanoidAppearanceComponent>.op_Implicit(vocalizer);
		if (attachedEntity.HasValue && attachedEntity.GetValueOrDefault() == val && !_config.GetClientCVar<bool>(forPlayer.Channel, RMCCVars.RMCPlayEmotesYourself))
		{
			return false;
		}
		if (!_humanoidAppearanceQuery.Resolve(Entity<HumanoidAppearanceComponent>.op_Implicit(vocalizer), ref vocalizer.Comp, false) || !_emoteCVars.TryGetValue(vocalizer.Comp.Species, out CVarDef play))
		{
			return true;
		}
		return _config.GetClientCVar<bool>(forPlayer.Channel, play.Name);
	}
}
