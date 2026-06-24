// Decompiled with JetBrains decompiler
// Type: Content.Client.ParticleAccelerator.UI.PASegmentControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Client.ParticleAccelerator.UI;

public sealed class PASegmentControl : Control
{
  private static readonly ProtoId<ShaderPrototype> GreyscaleShaderId = ProtoId<ShaderPrototype>.op_Implicit("Greyscale");
  private readonly ShaderInstance _greyScaleShader;
  private readonly TextureRect _base;
  private readonly TextureRect _unlit;
  private RSI? _rsi;

  public string BaseState { get; set; } = "control_box";

  public bool DefaultVisible { get; set; }

  public PASegmentControl()
  {
    this._greyScaleShader = IoCManager.Resolve<IPrototypeManager>().Index<ShaderPrototype>(PASegmentControl.GreyscaleShaderId).Instance();
    this.AddChild((Control) (this._base = new TextureRect()));
    this.AddChild((Control) (this._unlit = new TextureRect()));
  }

  protected virtual void EnteredTree()
  {
    base.EnteredTree();
    this._rsi = IoCManager.Resolve<IResourceCache>().GetResource<RSIResource>($"/Textures/Structures/Power/Generation/PA/{this.BaseState}.rsi", true).RSI;
    this.MinSize = Vector2i.op_Implicit(this._rsi.Size);
    this._base.Texture = this._rsi[RSI.StateId.op_Implicit("completed")].Frame0;
    this.SetVisible(this.DefaultVisible);
    ((Control) this._unlit).Visible = this.DefaultVisible;
  }

  public void SetPowerState(ParticleAcceleratorUIState state, bool exists)
  {
    this.SetVisible(exists);
    if (!state.Enabled || !exists)
    {
      ((Control) this._unlit).Visible = false;
    }
    else
    {
      ((Control) this._unlit).Visible = true;
      string str1;
      switch (state.State)
      {
        case ParticleAcceleratorPowerState.Standby:
          str1 = "_unlitp";
          break;
        case ParticleAcceleratorPowerState.Level0:
          str1 = "_unlitp0";
          break;
        case ParticleAcceleratorPowerState.Level1:
          str1 = "_unlitp1";
          break;
        case ParticleAcceleratorPowerState.Level2:
          str1 = "_unlitp2";
          break;
        case ParticleAcceleratorPowerState.Level3:
          str1 = "_unlitp3";
          break;
        default:
          str1 = "";
          break;
      }
      string str2 = str1;
      if (this._rsi == null)
        return;
      RSI.State state1;
      if (!this._rsi.TryGetState(RSI.StateId.op_Implicit(this.BaseState + str2), ref state1))
        ((Control) this._unlit).Visible = false;
      else
        this._unlit.Texture = state1.Frame0;
    }
  }

  private void SetVisible(bool state)
  {
    if (state)
    {
      this._base.ShaderOverride = (ShaderInstance) null;
      ((Control) this._base).ModulateSelfOverride = new Color?();
    }
    else
    {
      this._base.ShaderOverride = this._greyScaleShader;
      ((Control) this._base).ModulateSelfOverride = new Color?(new Color((byte) 127 /*0x7F*/, (byte) 127 /*0x7F*/, (byte) 127 /*0x7F*/, byte.MaxValue));
    }
  }
}
