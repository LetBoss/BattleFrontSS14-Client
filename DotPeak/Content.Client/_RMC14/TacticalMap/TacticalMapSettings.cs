// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.TacticalMap.TacticalMapSettings
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using System.Numerics;

#nullable disable
namespace Content.Client._RMC14.TacticalMap;

public struct TacticalMapSettings
{
  public float ZoomFactor;
  public Vector2 PanOffset;
  public float BlipSizeMultiplier;
  public float LineThickness;
  public int SelectedColorIndex;
  public bool SettingsVisible;
  public TacticalMapControl.LabelMode LabelMode;
  public Vector2 WindowSize;
  public Vector2 WindowPosition;

  public TacticalMapSettings()
  {
    this.ZoomFactor = 0.0f;
    this.PanOffset = new Vector2();
    this.BlipSizeMultiplier = 0.0f;
    this.LineThickness = 0.0f;
    this.SelectedColorIndex = 0;
    this.SettingsVisible = false;
    this.WindowSize = new Vector2();
    this.WindowPosition = new Vector2();
    this.LabelMode = TacticalMapControl.LabelMode.Area;
  }
}
