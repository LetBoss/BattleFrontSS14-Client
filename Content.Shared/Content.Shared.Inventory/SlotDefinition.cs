using System;
using System.Numerics;
using Content.Shared.Whitelist;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Inventory;

[DataDefinition]
public sealed class SlotDefinition : ISerializationGenerated<SlotDefinition>, ISerializationGenerated
{
	[DataField("whitelist", false, 1, false, false, null)]
	public EntityWhitelist? Whitelist;

	[DataField("blacklist", false, 1, false, false, null)]
	public EntityWhitelist? Blacklist;

	[DataField("name", false, 1, true, false, null)]
	public string Name { get; private set; } = string.Empty;

	[DataField("slotTexture", false, 1, false, false, null)]
	public string TextureName { get; private set; } = "pocket";

	[DataField(null, false, 1, false, false, null)]
	public string FullTextureName { get; private set; } = "SlotBackground";

	[DataField("slotFlags", false, 1, false, false, null)]
	public SlotFlags SlotFlags { get; private set; } = SlotFlags.PREVENTEQUIP;

	[DataField("showInWindow", false, 1, false, false, null)]
	public bool ShowInWindow { get; private set; } = true;

	[DataField("slotGroup", false, 1, false, false, null)]
	public string SlotGroup { get; private set; } = "Default";

	[DataField("stripTime", false, 1, false, false, null)]
	public TimeSpan StripTime { get; private set; } = TimeSpan.FromSeconds(4.0);

	[DataField("uiWindowPos", false, 1, true, false, null)]
	public Vector2i UIWindowPosition { get; private set; }

	[DataField("strippingWindowPos", false, 1, true, false, null)]
	public Vector2i StrippingWindowPos { get; private set; }

	[DataField("dependsOn", false, 1, false, false, null)]
	public string? DependsOn { get; private set; }

	[DataField("dependsOnComponents", false, 1, false, false, null)]
	public ComponentRegistry? DependsOnComponents { get; private set; }

	[DataField("displayName", false, 1, true, false, null)]
	public string DisplayName { get; private set; } = string.Empty;

	[DataField("stripHidden", false, 1, false, false, null)]
	public bool StripHidden { get; private set; }

	[DataField("offset", false, 1, false, false, null)]
	public Vector2 Offset { get; private set; } = Vector2.Zero;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SlotDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<SlotDefinition>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		string NameTemp = null;
		if (Name == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
		{
			NameTemp = Name;
		}
		target.Name = NameTemp;
		string TextureNameTemp = null;
		if (TextureName == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(TextureName, ref TextureNameTemp, hookCtx, false, context))
		{
			TextureNameTemp = TextureName;
		}
		target.TextureName = TextureNameTemp;
		string FullTextureNameTemp = null;
		if (FullTextureName == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(FullTextureName, ref FullTextureNameTemp, hookCtx, false, context))
		{
			FullTextureNameTemp = FullTextureName;
		}
		target.FullTextureName = FullTextureNameTemp;
		SlotFlags SlotFlagsTemp = SlotFlags.NONE;
		if (!serialization.TryCustomCopy<SlotFlags>(SlotFlags, ref SlotFlagsTemp, hookCtx, false, context))
		{
			SlotFlagsTemp = SlotFlags;
		}
		target.SlotFlags = SlotFlagsTemp;
		bool ShowInWindowTemp = false;
		if (!serialization.TryCustomCopy<bool>(ShowInWindow, ref ShowInWindowTemp, hookCtx, false, context))
		{
			ShowInWindowTemp = ShowInWindow;
		}
		target.ShowInWindow = ShowInWindowTemp;
		string SlotGroupTemp = null;
		if (SlotGroup == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(SlotGroup, ref SlotGroupTemp, hookCtx, false, context))
		{
			SlotGroupTemp = SlotGroup;
		}
		target.SlotGroup = SlotGroupTemp;
		TimeSpan StripTimeTemp = default(TimeSpan);
		if (!serialization.TryCustomCopy<TimeSpan>(StripTime, ref StripTimeTemp, hookCtx, false, context))
		{
			StripTimeTemp = serialization.CreateCopy<TimeSpan>(StripTime, hookCtx, context, false);
		}
		target.StripTime = StripTimeTemp;
		Vector2i UIWindowPositionTemp = default(Vector2i);
		if (!serialization.TryCustomCopy<Vector2i>(UIWindowPosition, ref UIWindowPositionTemp, hookCtx, false, context))
		{
			UIWindowPositionTemp = serialization.CreateCopy<Vector2i>(UIWindowPosition, hookCtx, context, false);
		}
		target.UIWindowPosition = UIWindowPositionTemp;
		Vector2i StrippingWindowPosTemp = default(Vector2i);
		if (!serialization.TryCustomCopy<Vector2i>(StrippingWindowPos, ref StrippingWindowPosTemp, hookCtx, false, context))
		{
			StrippingWindowPosTemp = serialization.CreateCopy<Vector2i>(StrippingWindowPos, hookCtx, context, false);
		}
		target.StrippingWindowPos = StrippingWindowPosTemp;
		string DependsOnTemp = null;
		if (!serialization.TryCustomCopy<string>(DependsOn, ref DependsOnTemp, hookCtx, false, context))
		{
			DependsOnTemp = DependsOn;
		}
		target.DependsOn = DependsOnTemp;
		ComponentRegistry DependsOnComponentsTemp = null;
		if (!serialization.TryCustomCopy<ComponentRegistry>(DependsOnComponents, ref DependsOnComponentsTemp, hookCtx, false, context))
		{
			DependsOnComponentsTemp = serialization.CreateCopy<ComponentRegistry>(DependsOnComponents, hookCtx, context, false);
		}
		target.DependsOnComponents = DependsOnComponentsTemp;
		string DisplayNameTemp = null;
		if (DisplayName == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<string>(DisplayName, ref DisplayNameTemp, hookCtx, false, context))
		{
			DisplayNameTemp = DisplayName;
		}
		target.DisplayName = DisplayNameTemp;
		bool StripHiddenTemp = false;
		if (!serialization.TryCustomCopy<bool>(StripHidden, ref StripHiddenTemp, hookCtx, false, context))
		{
			StripHiddenTemp = StripHidden;
		}
		target.StripHidden = StripHiddenTemp;
		Vector2 OffsetTemp = default(Vector2);
		if (!serialization.TryCustomCopy<Vector2>(Offset, ref OffsetTemp, hookCtx, false, context))
		{
			OffsetTemp = serialization.CreateCopy<Vector2>(Offset, hookCtx, context, false);
		}
		target.Offset = OffsetTemp;
		EntityWhitelist WhitelistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, false);
			}
		}
		target.Whitelist = WhitelistTemp;
		EntityWhitelist BlacklistTemp = null;
		if (!serialization.TryCustomCopy<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, false, context))
		{
			if (Blacklist == null)
			{
				BlacklistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Blacklist, ref BlacklistTemp, hookCtx, context, false);
			}
		}
		target.Blacklist = BlacklistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SlotDefinition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlotDefinition cast = (SlotDefinition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SlotDefinition Instantiate()
	{
		return new SlotDefinition();
	}
}
