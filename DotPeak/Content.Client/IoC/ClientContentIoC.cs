// Decompiled with JetBrains decompiler
// Type: Content.Client.IoC.ClientContentIoC
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.LanguageSelect;
using Content.Client._RMC14.Commendations;
using Content.Client._RMC14.LinkAccount;
using Content.Client._RMC14.PlayTimeTracking;
using Content.Client._RMC14.TacticalMap;
using Content.Client.Administration.Managers;
using Content.Client.Changelog;
using Content.Client.Chat.Managers;
using Content.Client.Clickable;
using Content.Client.DebugMon;
using Content.Client.Eui;
using Content.Client.Fullscreen;
using Content.Client.GameTicking.Managers;
using Content.Client.GhostKick;
using Content.Client.Guidebook;
using Content.Client.Launcher;
using Content.Client.Lobby;
using Content.Client.Mapping;
using Content.Client.Parallax.Managers;
using Content.Client.Players.PlayTimeTracking;
using Content.Client.Players.RateLimiting;
using Content.Client.Playtime;
using Content.Client.Replay;
using Content.Client.Screenshot;
using Content.Client.Stylesheets;
using Content.Client.Viewport;
using Content.Client.Voting;
using Content.Shared.Administration.Logs;
using Content.Shared.Administration.Managers;
using Content.Shared.Chat;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Players.RateLimiting;
using Robust.Shared.IoC;

#nullable disable
namespace Content.Client.IoC;

internal static class ClientContentIoC
{
  public static void Register()
  {
    IDependencyCollection instance = IoCManager.Instance;
    instance.Register<IParallaxManager, ParallaxManager>(false);
    DependencyCollectionExt.Register<GeneratedParallaxCache>(instance);
    instance.Register<IChatManager, ChatManager>(false);
    instance.Register<ISharedChatManager, ChatManager>(false);
    instance.Register<IClientPreferencesManager, ClientPreferencesManager>(false);
    instance.Register<IStylesheetManager, StylesheetManager>(false);
    instance.Register<IScreenshotHook, ScreenshotHook>(false);
    instance.Register<FullscreenHook, FullscreenHook>(false);
    instance.Register<IClickMapManager, ClickMapManager>(false);
    instance.Register<IClientAdminManager, ClientAdminManager>(false);
    instance.Register<ISharedAdminManager, ClientAdminManager>(false);
    instance.Register<EuiManager, EuiManager>(false);
    instance.Register<IVoteManager, VoteManager>(false);
    instance.Register<ChangelogManager, ChangelogManager>(false);
    instance.Register<ViewportManager, ViewportManager>(false);
    instance.Register<ISharedAdminLogManager, SharedAdminLogManager>(false);
    DependencyCollectionExt.Register<GhostKickManager>(instance);
    DependencyCollectionExt.Register<ExtendedDisconnectInformationManager>(instance);
    DependencyCollectionExt.Register<JobRequirementsManager>(instance);
    DependencyCollectionExt.Register<DocumentParsingManager>(instance);
    DependencyCollectionExt.Register<ContentReplayPlaybackManager>(instance);
    instance.Register<ISharedPlaytimeManager, JobRequirementsManager>(false);
    DependencyCollectionExt.Register<MappingManager>(instance);
    DependencyCollectionExt.Register<DebugMonitorManager>(instance);
    DependencyCollectionExt.Register<PlayerRateLimitManager>(instance);
    instance.Register<SharedPlayerRateLimitManager, PlayerRateLimitManager>(false);
    DependencyCollectionExt.Register<TitleWindowManager>(instance);
    DependencyCollectionExt.Register<ClientsidePlaytimeTrackingManager>(instance);
    DependencyCollectionExt.Register<LinkAccountManager>(instance);
    DependencyCollectionExt.Register<RMCPlayTimeManager>(instance);
    DependencyCollectionExt.Register<CommendationsManager>(instance);
    DependencyCollectionExt.Register<TacticalMapSettingsManager>(instance);
    DependencyCollectionExt.Register<LanguageSelectManager>(instance);
  }
}
