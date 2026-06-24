// Decompiled with JetBrains decompiler
// Type: Content.Shared.CombatMode.DisarmedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.CombatMode;

[ByRefEvent]
public record struct DisarmedEvent(EntityUid Target, EntityUid Source, float PushProb)
{
  public readonly EntityUid Target = Target;
  public readonly EntityUid Source = Source;
  public readonly float PushProbability = PushProb;
  public string PopupPrefix = "";
  public bool IsStunned = false;
  public bool Handled = false;

  public float PushProb { get; set; } = PushProb;

  [CompilerGenerated]
  public readonly void Deconstruct(out EntityUid Target, out EntityUid Source, out float PushProb)
  {
    Target = this.Target;
    Source = this.Source;
    PushProb = this.PushProb;
  }
}
