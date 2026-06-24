// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.AegisEvent.AegisSharedAnnouncement
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Xenonids.Announce;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.AegisEvent;

public static class AegisSharedAnnouncement
{
  public static void AnnounceToBoth(IEntitySystemManager sysMan, string message)
  {
    sysMan.GetEntitySystem<SharedMarineAnnounceSystem>().AnnounceHighCommand("[font size=16][bold][color=#CECECE]UNMC High Command Announcement[/color][/bold][/font][font size=16][color=red]\n\nAttention UNS Almayer,\n\nThis is General Carvain, contacting your vessel from the UNS Oberon.\nDuring a goods transport operation for AEGIS Armaments, we have lost communications with the colony. Being the closest vessel, the UNS Almayer is instructed to recover the package. Once recovered, assuming a hostile presence, you are permitted to use the cargo as required. Devices essential for the retrival will be sent to your requisitions department via your ASRS Lift shortly. Do not let the cargo or it's key fall into enemy hands.\n\nMore detailed information will be sent to the Almayer's command staff shortly.[italic]\n\nSigned by,\nGeneral Carvain[/italic][/color][/font]");
    sysMan.GetEntitySystem<SharedXenoAnnounceSystem>().AnnounceAll(new EntityUid(), "\n[font size=24][italic][color=green]The Queen Mother reaches into your mind from worlds away.[/color][/italic][/font]\n\n[head=3][color=Red]Daughters, danger is amidst you. A powerful device- the source of the dreaded skyfire- lies somewhere near your hive, built by the hive of the Tall Empress before we claimed it. They will stop at nothing to take it back and turn it upon you.\n\nThis cannot be allowed. My daughters, your carapace is tougher than any metal resin. Your strength a legion of indomitable might. You are an army of claw and lightning. You will not yield. You will not fail. In you, my daughters, I have not trust - but faith. Faith in the smallest of lessers. Faith in your Queen.[/color][/head]\n\n[bolditalic][head=2][color=DarkViolet]Fight! For the Hive![/color][/head][/bolditalic]\n\n");
  }
}
