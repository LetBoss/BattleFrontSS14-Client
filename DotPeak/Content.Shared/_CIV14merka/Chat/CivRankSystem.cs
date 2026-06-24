// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Chat.CivRankSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Content.Shared.Chat;
using Content.Shared.Examine;
using Content.Shared.Humanoid;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._CIV14merka.Chat;

public abstract class CivRankSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<CivTeamMemberComponent, TransformSpeakerNameEvent>(new EntityEventRefHandler<CivTeamMemberComponent, TransformSpeakerNameEvent>(this.OnTransformSpeakerName));
    this.SubscribeLocalEvent<CivTeamMemberComponent, ExaminedEvent>(new EntityEventRefHandler<CivTeamMemberComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnTransformSpeakerName(
    Entity<CivTeamMemberComponent> ent,
    ref TransformSpeakerNameEvent args)
  {
    string civ14Rank = this.GetCiv14Rank(ent.Owner, ent.Comp);
    if (string.IsNullOrEmpty(civ14Rank))
      return;
    args.VoiceName = $"{civ14Rank} {args.VoiceName}";
  }

  private void OnExamined(Entity<CivTeamMemberComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    string civ14Rank = this.GetCiv14Rank(ent.Owner, ent.Comp);
    if (string.IsNullOrEmpty(civ14Rank))
      return;
    string genderPronoun = this.GetGenderPronoun(ent.Owner);
    args.PushMarkup($"{genderPronoun} is {civ14Rank}");
  }

  private string GetGenderPronoun(EntityUid uid)
  {
    HumanoidAppearanceComponent comp;
    if (!this.TryComp<HumanoidAppearanceComponent>(uid, out comp))
      return "They";
    string genderPronoun;
    switch (comp.Gender)
    {
      case Gender.Female:
        genderPronoun = "She";
        break;
      case Gender.Male:
        genderPronoun = "He";
        break;
      default:
        genderPronoun = "They";
        break;
    }
    return genderPronoun;
  }

  protected abstract string? GetCiv14Rank(EntityUid source, CivTeamMemberComponent speakerTeam);
}
