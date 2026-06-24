using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Shared.Telephone;

public abstract class SharedTelephoneSystem : EntitySystem
{
	public bool IsTelephoneEngaged(Entity<TelephoneComponent> entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return entity.Comp.LinkedTelephones.Any();
	}

	public string GetFormattedCallerIdForEntity(string? presumedName, string? presumedJob, Color fontColor, string fontType = "Default", int fontSize = 12)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		string callerId = base.Loc.GetString("chat-telephone-unknown-caller", new(string, object)[3]
		{
			("color", fontColor),
			("fontType", fontType),
			("fontSize", fontSize)
		});
		if (presumedName == null)
		{
			return callerId;
		}
		if (presumedJob != null)
		{
			return base.Loc.GetString("chat-telephone-caller-id-with-job", new(string, object)[5]
			{
				("callerName", presumedName),
				("callerJob", presumedJob),
				("color", fontColor),
				("fontType", fontType),
				("fontSize", fontSize)
			});
		}
		return base.Loc.GetString("chat-telephone-caller-id-without-job", new(string, object)[4]
		{
			("callerName", presumedName),
			("color", fontColor),
			("fontType", fontType),
			("fontSize", fontSize)
		});
	}

	public string GetFormattedDeviceIdForEntity(string? deviceName, Color fontColor, string fontType = "Default", int fontSize = 12)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (deviceName == null)
		{
			return base.Loc.GetString("chat-telephone-unknown-device", new(string, object)[3]
			{
				("color", fontColor),
				("fontType", fontType),
				("fontSize", fontSize)
			});
		}
		return base.Loc.GetString("chat-telephone-device-id", new(string, object)[4]
		{
			("deviceName", deviceName),
			("color", fontColor),
			("fontType", fontType),
			("fontSize", fontSize)
		});
	}
}
