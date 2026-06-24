// Decompiled with JetBrains decompiler
// Type: Content.Client.Viewport.ViewportExt
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface.CustomControls;

#nullable enable
namespace Content.Client.Viewport;

public static class ViewportExt
{
  public static int GetRenderScale(this IViewportControl viewport)
  {
    return viewport is ScalingViewport scalingViewport ? scalingViewport.CurrentRenderScale : 1;
  }
}
