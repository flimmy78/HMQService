; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "HMQService"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "���ݱ��ƴ�����������޹�˾"
#define MyAppURL "http://www.bekzoyo.com.cn/"
#define DotNetFile "NDP451-KB2858728-x86-x64-AllOS-ENU.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
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
;Ҫ�����ԱȨ��
PrivilegesRequired=admin
;��װ���Ҫ������
AlwaysRestart=yes
;������ж��
Uninstallable=false

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "./3rd-party/vcredist.exe"; DestDir: "{app}"; Flags: onlyifdoesntexist ignoreversion
Source: "./3rd-party/{#DotNetFile}"; DestDir: "{tmp}"; Flags: onlyifdoesntexist ignoreversion deleteafterinstall; AfterInstall: InstallFramework; Check: FrameworkIsNotInstalled
Source: "./3rd-party/mencoder.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "./3rd-party/HKLib/*"; DestDir: "{app}/bin"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "../HMQService/bin/Release/*"; DestDir: "{app}"; Flags: ignoreversion


;һЩͨ�õ���Դ�ļ��������ļ�
Source: "./res/ͨ��/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

;��Ŀ����Ŀ��ģʽ
Source: "./res/��Ŀ����Ŀ��/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

;��Ŀ����ͼģʽ

;��Ŀ����Ŀ��ģʽ

;��Ŀ����ͼģʽ


[Run]
;��װ����
;Filename: "{sys}\sc.exe"; Parameters: "create ttts DisplayName= ""ttts"" binPah= ""D:\Program Files (x86)\HMQService\HMQService.exe"""; Flags: runhidden waituntilterminated;;Filename: "{sys}\sc.exe"; Parameters: "create ""{#MyAppName}"" binPah= ""{app}\{#MyAppName}.exe"""; Flags: runhidden;
;���÷��񿪻�������
;Filename: "{sys}\sc.exe"; Parameters: "config ttts start=AUTO"; Flags: runhidden waituntilterminated;
;Filename: "{sys}\cmd.exe"; Parameters: "sc config ""{#MyAppName}"" start=AUTO"; Flags: runhidden;
;Filename: "{app}\HMQService.exe"; Parameters: "-install"; Flags: runhidden;

[code]

//�������Ƿ��Ѿ���װ
function InitializeSetup():boolean;
var
  ResultCode: Integer;
  uicmd: String;
begin
  if RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{473CDD01-B0B6-43AF-8534-74E1F1DFFFF4}_is1', 'UninstallString', uicmd) then
  begin
    //ֹͣ����
    Exec(ExpandConstant('c:\windows\system32\sc.exe'), 'stop "{#MyAppName}"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    //ж�ط���
    Exec(ExpandConstant('c:\windows\system32\sc.exe'), 'detele "{#MyAppName}"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;

  Result:= True
end;

//���ϵͳ�Ƿ��Ѿ���װ.net 4.5.1
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

//��װ.net 4.5.1
procedure InstallFramework;
var ResultCode: Integer;
begin
    Exec(ExpandConstant('{app}\vcredist.exe'), '/q /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
    if FrameworkIsNotInstalled() then
    begin
        Exec(ExpandConstant('{tmp}\{#DotNetFile}'), '/q /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
    end;
end;

//��װ���
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

