; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "HMQService"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "福州北科大舟宇电子有限公司"
#define MyAppURL "http://www.bekzoyo.com.cn/"
#define DotNetFile "NDP451-KB2858728-x86-x64-AllOS-ENU.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{473CDD01-B0B6-43AF-8534-74E1F1DFFFF4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
;DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=HMQService
Compression=lzma
SolidCompression=yes
;要求管理员权限
PrivilegesRequired=admin
;安装完成要求重启
AlwaysRestart=yes
;不允许卸载
Uninstallable=false

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "./3rd-party/vcredist.exe"; DestDir: "{app}"; Flags: onlyifdoesntexist ignoreversion
Source: "./3rd-party/{#DotNetFile}"; DestDir: "{tmp}"; Flags: onlyifdoesntexist ignoreversion deleteafterinstall; AfterInstall: InstallFramework; Check: FrameworkIsNotInstalled
Source: "./3rd-party/mencoder.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "./3rd-party/HKLib/*"; DestDir: "{app}/bin"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "../HMQService/bin/Release/*"; DestDir: "{app}"; Flags: ignoreversion


;一些通用的资源文件和配置文件
Source: "./res/通用/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

;科目二项目牌模式
Source: "./res/科目二项目牌/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

;科目二地图模式

;科目三项目牌模式

;科目三地图模式


[Run]
;安装服务
;Filename: "{sys}\sc.exe"; Parameters: "create ttts DisplayName= ""ttts"" binPah= ""D:\Program Files (x86)\HMQService\HMQService.exe"""; Flags: runhidden waituntilterminated;;Filename: "{sys}\sc.exe"; Parameters: "create ""{#MyAppName}"" binPah= ""{app}\{#MyAppName}.exe"""; Flags: runhidden;
;设置服务开机自启动
;Filename: "{sys}\sc.exe"; Parameters: "config ttts start=AUTO"; Flags: runhidden waituntilterminated;
;Filename: "{sys}\cmd.exe"; Parameters: "sc config ""{#MyAppName}"" start=AUTO"; Flags: runhidden;
;Filename: "{app}\HMQService.exe"; Parameters: "-install"; Flags: runhidden;

[code]

//检测服务是否已经安装
function InitializeSetup():boolean;
var
  ResultCode: Integer;
  uicmd: String;
begin
  if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{473CDD01-B0B6-43AF-8534-74E1F1DFFFF4}_is1', 'UninstallString', uicmd) then
  begin
    //停止服务
    Exec(ExpandConstant('c:\windows\system32\sc.exe'), 'stop "{#MyAppName}"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    //卸载服务
    Exec(ExpandConstant('c:\windows\system32\sc.exe'), 'detele "{#MyAppName}"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;

  Result:= True
end;

//检测系统是否已经安装.net 4.5.1
function FrameworkIsNotInstalled: Boolean;
var
    bSuccess: Boolean;
    regVersion: Cardinal;
begin
    Result := True;
    bSuccess := RegQueryDWordValue(HKLM, 'Software\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', regVersion)

    if (True = bSuccess) and (regVersion >= 378675) then
    begin
        Result := False;
    end;
end;

//安装.net 4.5.1
procedure InstallFramework;
var ResultCode: Integer;
begin
    Exec(ExpandConstant('{app}\vcredist.exe'), '/q /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
    if FrameworkIsNotInstalled() then
    begin
        Exec(ExpandConstant('{tmp}\{#DotNetFile}'), '/q /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
    end;
end;

//安装完成
procedure CurStepChanged(CurStep: TSetupStep);  
var  
uninspath, uninsname, NewUninsName, MyAppName: string;  
userName: string;
passWord: string;
parmStr:  string;
oldFilePath : string;
newfilePath : string;
resultCode: Integer;
begin  
if CurStep=ssDone then  
  begin

    //MsgBox('{app}\{#MyAppName}.exe',mbInformation,MB_OK);


  end;  
end;

