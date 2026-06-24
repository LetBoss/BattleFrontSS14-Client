// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialMenuNestedLayerOption
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class RadialMenuNestedLayerOption(
  IReadOnlyCollection<RadialMenuOption> nested,
  float containerRadius = 100f) : RadialMenuOption
{
  public float? ContainerRadius { get; } = new float?(containerRadius);

  public IReadOnlyCollection<RadialMenuOption> Nested { get; } = nested;
}
