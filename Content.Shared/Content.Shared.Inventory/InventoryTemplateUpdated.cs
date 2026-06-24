using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Inventory;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public struct InventoryTemplateUpdated
{
}
