using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Silicons.Laws;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client.Silicons.Laws.SiliconLawEditUi;

public sealed class SiliconLawEui : BaseEui
{
	private readonly EntityManager _entityManager;

	private SiliconLawUi _siliconLawUi;

	private EntityUid _target;

	public SiliconLawEui()
	{
		_entityManager = IoCManager.Resolve<EntityManager>();
		_siliconLawUi = new SiliconLawUi();
		((BaseWindow)_siliconLawUi).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
		((BaseButton)_siliconLawUi.Save).OnPressed += delegate
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			SendMessage(new SiliconLawsSaveMessage(_siliconLawUi.GetLaws(), _entityManager.GetNetEntity(_target, (MetaDataComponent)null)));
		};
	}

	public override void HandleState(EuiStateBase state)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (state is SiliconLawsEuiState siliconLawsEuiState)
		{
			_target = _entityManager.GetEntity(siliconLawsEuiState.Target);
			_siliconLawUi.SetLaws(siliconLawsEuiState.Laws);
		}
	}

	public override void Opened()
	{
		((BaseWindow)_siliconLawUi).OpenCentered();
	}
}
