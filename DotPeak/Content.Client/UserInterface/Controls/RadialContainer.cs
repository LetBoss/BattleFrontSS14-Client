// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.RadialContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable disable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class RadialContainer : LayoutContainer
{
  private const float RadiusIncrement = 5f;
  private Vector2 _angularRange = new Vector2(0.0f, 6.28318548f);

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2 AngularRange
  {
    get => this._angularRange;
    set
    {
      float x = value.X;
      float y = value.Y;
      float num1 = (double) x > 6.2831854820251465 ? x % 6.28318548f : x;
      float num2 = (double) y > 6.2831854820251465 ? y % 6.28318548f : y;
      this._angularRange = new Vector2((double) num1 < 0.0 ? 6.28318548f + num1 : num1, (double) num2 < 0.0 ? 6.28318548f + num2 : num2);
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public RadialContainer.RAlignment RadialAlignment { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public float InitialRadius { get; set; } = 100f;

  [Robust.Shared.ViewVariables.ViewVariables]
  public float CalculatedRadius { get; private set; }

  public float InnerRadiusMultiplier { get; set; } = 0.5f;

  public float OuterRadiusMultiplier { get; set; } = 1.5f;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool ReserveSpaceForHiddenChildren { get; set; } = true;

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    IEnumerable<Control> source = this.ReserveSpaceForHiddenChildren ? (IEnumerable<Control>) ((Control) this).Children : ((IEnumerable<Control>) ((Control) this).Children).Where<Control>((Func<Control, bool>) (x => x.Visible));
    int num1 = source.Count<Control>();
    this.CalculatedRadius = this.InitialRadius + (float) num1 * 5f;
    bool flag = this.RadialAlignment == RadialContainer.RAlignment.AntiClockwise;
    float num2 = this.AngularRange.Y - this.AngularRange.X;
    float num3 = (double) num2 < 0.0 ? 6.28318548f + num2 : num2;
    float num4 = flag ? 6.28318548f - num3 : num3;
    int num5 = !MathHelper.CloseTo(num4, 6.28318548f, 0.01f) ? 1 : 0;
    float num6 = num4 / (float) (num1 - num5) * (flag ? -1f : 1f);
    Vector2 vector2_1 = finalSize * 0.5f;
    foreach ((int num7, Control control) in source.Select<Control, (int, Control)>((Func<Control, int, (int, Control)>) ((x, index) => (index, x))))
    {
      float x = (float) ((double) this.AngularRange.X + (double) num6 * ((double) num7 + 0.5) + 1.5707963705062866);
      Vector2 vector2_2 = new Vector2(MathF.Floor(this.CalculatedRadius * MathF.Cos(x)), MathF.Floor(-this.CalculatedRadius * MathF.Sin(x))) + vector2_1 - control.DesiredSize * 0.5f + ((Control) this).Position;
      LayoutContainer.SetPosition(control, vector2_2);
      if (control is IRadialMenuItemWithSector menuItemWithSector)
      {
        menuItemWithSector.AngleSectorFrom = num6 * (float) num7;
        menuItemWithSector.AngleSectorTo = num6 * (float) (num7 + 1);
        menuItemWithSector.AngleOffset = 1.57079637f;
        menuItemWithSector.InnerRadius = this.CalculatedRadius * this.InnerRadiusMultiplier;
        menuItemWithSector.OuterRadius = this.CalculatedRadius * this.OuterRadiusMultiplier;
        menuItemWithSector.ParentCenter = vector2_1;
      }
    }
    return base.ArrangeOverride(finalSize);
  }

  public enum RAlignment : byte
  {
    Clockwise,
    AntiClockwise,
  }
}
