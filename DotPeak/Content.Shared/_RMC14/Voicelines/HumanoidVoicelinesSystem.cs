// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Voicelines.HumanoidVoicelinesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Voicelines;

public sealed class HumanoidVoicelinesSystem : EntitySystem
{
  [Dependency]
  private INetConfigurationManager _config;
  private static readonly ProtoId<SpeciesPrototype> ArachnidSpecies = (ProtoId<SpeciesPrototype>) "Arachnid";
  private static readonly ProtoId<SpeciesPrototype> DionaSpecies = (ProtoId<SpeciesPrototype>) "Diona";
  private static readonly ProtoId<SpeciesPrototype> DwarfSpecies = (ProtoId<SpeciesPrototype>) "Dwarf";
  private static readonly ProtoId<SpeciesPrototype> FelinidSpecies = (ProtoId<SpeciesPrototype>) "Felinid";
  private static readonly ProtoId<SpeciesPrototype> HumanSpecies = (ProtoId<SpeciesPrototype>) "Human";
  private static readonly ProtoId<SpeciesPrototype> MothSpecies = (ProtoId<SpeciesPrototype>) "Moth";
  private static readonly ProtoId<SpeciesPrototype> ReptilianSpecies = (ProtoId<SpeciesPrototype>) "Reptilian";
  private static readonly ProtoId<SpeciesPrototype> SlimeSpecies = (ProtoId<SpeciesPrototype>) "SlimePerson";
  private static readonly ProtoId<SpeciesPrototype> AvaliSpecies = (ProtoId<SpeciesPrototype>) "Avali";
  private static readonly ProtoId<SpeciesPrototype> VulpkaninSpecies = (ProtoId<SpeciesPrototype>) "Vulpkanin";
  private static readonly ProtoId<SpeciesPrototype> RodentiaSpecies = (ProtoId<SpeciesPrototype>) "Rodentia";
  private static readonly ProtoId<SpeciesPrototype> FeroxiSpecies = (ProtoId<SpeciesPrototype>) "Feroxi";
  private static readonly ProtoId<SpeciesPrototype> SkrellSpecies = (ProtoId<SpeciesPrototype>) "Skrell";
  private readonly Dictionary<ProtoId<SpeciesPrototype>, CVarDef> _voicelineCVars = new Dictionary<ProtoId<SpeciesPrototype>, CVarDef>()
  {
    [HumanoidVoicelinesSystem.ArachnidSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesArachnid,
    [HumanoidVoicelinesSystem.DionaSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesDiona,
    [HumanoidVoicelinesSystem.DwarfSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesDwarf,
    [HumanoidVoicelinesSystem.FelinidSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesFelinid,
    [HumanoidVoicelinesSystem.HumanSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesHuman,
    [HumanoidVoicelinesSystem.MothSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesMoth,
    [HumanoidVoicelinesSystem.ReptilianSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesReptilian,
    [HumanoidVoicelinesSystem.SlimeSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesSlime,
    [HumanoidVoicelinesSystem.AvaliSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesAvali,
    [HumanoidVoicelinesSystem.VulpkaninSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesVulpkanin,
    [HumanoidVoicelinesSystem.RodentiaSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesRodentia,
    [HumanoidVoicelinesSystem.FeroxiSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesFeroxi,
    [HumanoidVoicelinesSystem.SkrellSpecies] = (CVarDef) RMCCVars.RMCPlayVoicelinesSkrell
  };
  private readonly Dictionary<ProtoId<SpeciesPrototype>, CVarDef> _emoteCVars = new Dictionary<ProtoId<SpeciesPrototype>, CVarDef>()
  {
    [HumanoidVoicelinesSystem.ArachnidSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesArachnid,
    [HumanoidVoicelinesSystem.DionaSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesDiona,
    [HumanoidVoicelinesSystem.DwarfSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesDwarf,
    [HumanoidVoicelinesSystem.FelinidSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesFelinid,
    [HumanoidVoicelinesSystem.HumanSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesHuman,
    [HumanoidVoicelinesSystem.MothSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesMoth,
    [HumanoidVoicelinesSystem.ReptilianSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesReptilian,
    [HumanoidVoicelinesSystem.SlimeSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesSlime,
    [HumanoidVoicelinesSystem.AvaliSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesAvali,
    [HumanoidVoicelinesSystem.VulpkaninSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesVulpkanin,
    [HumanoidVoicelinesSystem.RodentiaSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesRodentia,
    [HumanoidVoicelinesSystem.FeroxiSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesFeroxi,
    [HumanoidVoicelinesSystem.SkrellSpecies] = (CVarDef) RMCCVars.RMCPlayEmotesSkrell
  };
  private Robust.Shared.GameObjects.EntityQuery<HumanoidAppearanceComponent> _humanoidAppearanceQuery;

  public override void Initialize()
  {
    this._humanoidAppearanceQuery = this.GetEntityQuery<HumanoidAppearanceComponent>();
  }

  public bool ShouldPlayVoiceline(
    Entity<HumanoidAppearanceComponent?> vocalizer,
    ICommonSession forPlayer)
  {
    EntityUid? attachedEntity = forPlayer.AttachedEntity;
    EntityUid entityUid = (EntityUid) vocalizer;
    if ((attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && !this._config.GetClientCVar<bool>(forPlayer.Channel, RMCCVars.RMCPlayVoicelinesYourself))
      return false;
    CVarDef cvarDef;
    return !this._humanoidAppearanceQuery.Resolve((EntityUid) vocalizer, ref vocalizer.Comp, false) || !this._voicelineCVars.TryGetValue(vocalizer.Comp.Species, out cvarDef) || this._config.GetClientCVar<bool>(forPlayer.Channel, cvarDef.Name);
  }

  public bool ShouldPlayEmote(
    Entity<HumanoidAppearanceComponent?> vocalizer,
    ICommonSession forPlayer)
  {
    EntityUid? attachedEntity = forPlayer.AttachedEntity;
    EntityUid entityUid = (EntityUid) vocalizer;
    if ((attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && !this._config.GetClientCVar<bool>(forPlayer.Channel, RMCCVars.RMCPlayEmotesYourself))
      return false;
    CVarDef cvarDef;
    return !this._humanoidAppearanceQuery.Resolve((EntityUid) vocalizer, ref vocalizer.Comp, false) || !this._emoteCVars.TryGetValue(vocalizer.Comp.Species, out cvarDef) || this._config.GetClientCVar<bool>(forPlayer.Channel, cvarDef.Name);
  }
}
