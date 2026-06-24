using Content.Shared.Singularity.Components;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0039: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_004d: Expected O, but got Unknown
		_greyScaleShader = IoCManager.Resolve<IPrototypeManager>().Index<ShaderPrototype>(GreyscaleShaderId).Instance();
		TextureRect val = new TextureRect();
		TextureRect val2 = val;
		_base = val;
		((Control)this).AddChild((Control)(object)val2);
		TextureRect val3 = new TextureRect();
		val2 = val3;
		_unlit = val3;
		((Control)this).AddChild((Control)(object)val2);
	}

	protected override void EnteredTree()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).EnteredTree();
		_rsi = IoCManager.Resolve<IResourceCache>().GetResource<RSIResource>("/Textures/Structures/Power/Generation/PA/" + BaseState + ".rsi", true).RSI;
		((Control)this).MinSize = Vector2i.op_Implicit(_rsi.Size);
		_base.Texture = _rsi[StateId.op_Implicit("completed")].Frame0;
		SetVisible(DefaultVisible);
		((Control)_unlit).Visible = DefaultVisible;
	}

	public void SetPowerState(ParticleAcceleratorUIState state, bool exists)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		SetVisible(exists);
		if (!state.Enabled || !exists)
		{
			((Control)_unlit).Visible = false;
			return;
		}
		((Control)_unlit).Visible = true;
		string text = state.State switch
		{
			ParticleAcceleratorPowerState.Standby => "_unlitp", 
			ParticleAcceleratorPowerState.Level0 => "_unlitp0", 
			ParticleAcceleratorPowerState.Level1 => "_unlitp1", 
			ParticleAcceleratorPowerState.Level2 => "_unlitp2", 
			ParticleAcceleratorPowerState.Level3 => "_unlitp3", 
			_ => "", 
		};
		if (_rsi != null)
		{
			State val = default(State);
			if (!_rsi.TryGetState(StateId.op_Implicit(BaseState + text), ref val))
			{
				((Control)_unlit).Visible = false;
			}
			else
			{
				_unlit.Texture = val.Frame0;
			}
		}
	}

	private void SetVisible(bool state)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (state)
		{
			_base.ShaderOverride = null;
			((Control)_base).ModulateSelfOverride = null;
		}
		else
		{
			_base.ShaderOverride = _greyScaleShader;
			((Control)_base).ModulateSelfOverride = new Color((byte)127, (byte)127, (byte)127, byte.MaxValue);
		}
	}
}
