using Content.Shared.Materials;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.GameObjects;

namespace Content.Client.Materials.UI;

public sealed class MaterialStorageUIController : UIController
{
	public void SendLatheEjectMessage(EntityUid uid, string material, int sheetsToEject)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		base.EntityManager.RaisePredictiveEvent<EjectMaterialMessage>(new EjectMaterialMessage(base.EntityManager.GetNetEntity(uid, (MetaDataComponent)null), material, sheetsToEject));
	}
}
