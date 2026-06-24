// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Examine.Pose.SharedRMCSetPoseSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared._RMC14.Examine.Pose;

public abstract class SharedRMCSetPoseSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCSetPoseComponent, GetVerbsEvent<Verb>>(new EntityEventRefHandler<RMCSetPoseComponent, GetVerbsEvent<Verb>>(this.OnSetPoseGetVerbs));
    this.SubscribeLocalEvent<RMCSetPoseComponent, ExaminedEvent>(new EntityEventRefHandler<RMCSetPoseComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<RMCSetPoseComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCSetPoseComponent, MobStateChangedEvent>(this.OnMobStateChanged));
  }

  private void OnSetPoseGetVerbs(Entity<RMCSetPoseComponent> ent, ref GetVerbsEvent<Verb> args)
  {
    if (!args.CanInteract || args.User != args.Target)
      return;
    Verb verb = new Verb()
    {
      Text = this.Loc.GetString("rmc-set-pose-title"),
      Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/character.svg.192dpi.png")),
      Priority = -5,
      Act = (Action) (() => this.SetPose(ent))
    };
    args.Verbs.Add(verb);
  }

  private void OnExamine(Entity<RMCSetPoseComponent> ent, ref ExaminedEvent args)
  {
    RMCSetPoseComponent comp = ent.Comp;
    if (comp.Pose.Trim() == string.Empty)
      return;
    using (args.PushGroup("RMCSetPoseComponent"))
    {
      string markup = this.Loc.GetString("rmc-set-pose-examined", (nameof (ent), (object) ent), ("pose", (object) FormattedMessage.EscapeText(comp.Pose)));
      args.PushMarkup(markup, -5);
    }
  }

  private void OnMobStateChanged(Entity<RMCSetPoseComponent> ent, ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Alive)
      return;
    ent.Comp.Pose = string.Empty;
    this.Dirty<RMCSetPoseComponent>(ent);
  }

  protected virtual void SetPose(Entity<RMCSetPoseComponent> ent)
  {
  }
}
