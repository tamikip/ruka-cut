#ifndef AppVersion
  #define AppVersion "1.0.1"
#endif

#ifndef PublishDir
  #define PublishDir "..\release\RukaCut-v" + AppVersion
#endif

[Setup]
AppId={{D8B0F06D-E4CA-45F4-A4A2-FBE8919B698E}
AppName=Ruka Cut
AppVersion={#AppVersion}
AppVerName=Ruka Cut {#AppVersion}
AppPublisher=Ruka Cut contributors
AppPublisherURL=https://github.com/tamikip/ruka-cut
AppSupportURL=https://github.com/tamikip/ruka-cut/issues
AppUpdatesURL=https://github.com/tamikip/ruka-cut/releases
DefaultDirName={localappdata}\Programs\Ruka Cut
DefaultGroupName=Ruka Cut
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\RukaCut.exe
LicenseFile=..\LICENSE
OutputDir=..\release
OutputBaseFilename=RukaCut-{#AppVersion}-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
SetupLogging=yes
VersionInfoVersion={#AppVersion}
VersionInfoProductName=Ruka Cut
VersionInfoDescription=Ruka Cut Setup
VersionInfoCompany=Ruka Cut contributors

[Languages]
Name: "zh"; MessagesFile: "compiler:Default.isl"
Name: "en"; MessagesFile: "compiler:Languages\English.isl"

[LangOptions]
DialogFontName=Microsoft YaHei UI
DialogFontSize=9
WelcomeFontName=Microsoft YaHei UI
TitleFontName=Microsoft YaHei UI

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Ruka Cut"; Filename: "{app}\RukaCut.exe"
Name: "{autodesktop}\Ruka Cut"; Filename: "{app}\RukaCut.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\RukaCut.exe"; Description: "{cm:LaunchProgram,Ruka Cut}"; Flags: nowait postinstall skipifsilent
