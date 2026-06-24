// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Megaphone.RMCMegaphoneSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Megaphone;

public sealed class RMCMegaphoneSystem : EntitySystem
{
  [Dependency]
  private DialogSystem _dialog;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCMegaphoneComponent, UseInHandEvent>(new EntityEventRefHandler<RMCMegaphoneComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<RMCMegaphoneComponent, ExaminedEvent>(new EntityEventRefHandler<RMCMegaphoneComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnUseInHand(Entity<RMCMegaphoneComponent> ent, ref UseInHandEvent args)
  {
    args.Handled = true;
    MegaphoneInputEvent ev = new MegaphoneInputEvent(this.GetNetEntity(args.User));
    this._dialog.OpenInput(args.User, this.Loc.GetString("rmc-megaphone-ui-text"), (DialogInputEvent) ev, characterLimit: 150);
  }

  private void OnExamined(Entity<RMCMegaphoneComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("rmc-megaphone-examine"));
  }
}
