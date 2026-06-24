// Decompiled with JetBrains decompiler
// Type: Content.Shared.Verbs.ActivationVerb
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Verbs;

[NetSerializable]
[Serializable]
public sealed class ActivationVerb : Verb
{
  public new static string DefaultTextStyleClass = nameof (ActivationVerb);

  public override int TypePriority => 1;

  public override bool DefaultDoContactInteraction => true;

  public ActivationVerb() => this.TextStyleClass = ActivationVerb.DefaultTextStyleClass;
}
