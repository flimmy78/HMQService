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
Source: "./res/Common/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

;科目二项目牌模式
Source: "./res/km2Xm/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsKm2AndXm 

;科目二地图模式
Source: "./res/km2Map/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsKm2AndMap

;科目三项目牌模式
Source: "./res/km3Xm/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsKm3AndXm

;科目三地图模式
Source: "./res/km3Map/*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: IsKm3AndMap

[Run]
;安装服务
;Filename: "{sys}\sc.exe"; Parameters: "create ttts DisplayName= ""ttts"" binPah= ""D:\Program Files (x86)\HMQService\HMQService.exe"""; Flags: runhidden waituntilterminated;;Filename: "{sys}\sc.exe"; Parameters: "create ""{#MyAppName}"" binPah= ""{app}\{#MyAppName}.exe"""; Flags: runhidden;
;设置服务开机自启动
;Filename: "{sys}\sc.exe"; Parameters: "config ttts start=AUTO"; Flags: runhidden waituntilterminated;
;Filename: "{sys}\cmd.exe"; Parameters: "sc config ""{#MyAppName}"" start=AUTO"; Flags: runhidden;
;Filename: "{app}\HMQService.exe"; Parameters: "-install"; Flags: runhidden;

[code]

var
  lbl,lb2,lb3,lb4,lb5:TLabel;
  jmsbPage,kskmPage,dbPage,mapPage,carPage:TwizardPage;
  radioHmq,radioJmq,radioKm2,radioKm3,radioSql,radioOracle,radioMap,radioXm,radioCarYes,radioCarNo:TRadioButton;

//科目二地图版本
function IsKm2AndMap: Boolean;
begin
    Result := radioKm2.Checked and radioMap.Checked;
end;

//科目二项目牌版本
function IsKm2AndXm: Boolean;
begin
    Result := radioKm2.Checked and radioXm.Checked;
end;

//科目三地图版本
function IsKm3AndMap: Boolean;
begin
    Result := radioKm3.Checked and radioMap.Checked;
end;

//科目三项目牌版本
function IsKm3AndXm: Boolean;
begin
    Result := radioKm3.Checked and radioXm.Checked;
end;

function kskmPage_NextButtonClick(Page: TWizardPage): Boolean;
var 
  bKm2 : Boolean;
begin
  bKm2 := radioKm2.Checked;
  if bKm2 = true then
    begin 
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','KSKM','2');
        Result := true;
    end
  else
    begin
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','KSKM','3');
        Result := true;
  end
end;

function jmsbPage_NextButtonClick(Page: TWizardPage): Boolean;
var 
  bHmq : Boolean;
begin
  bHmq := radioHmq.Checked;
  if bHmq = true then
    begin 
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','HMQ','1');
        Result := true;
    end
  else
    begin
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','HMQ','0');
        Result := true;
  end
end;

function dbPage_NextButtonClick(Page: TWizardPage): Boolean;
var 
  bSql : Boolean;
begin
  bSql := radioSql.Checked;
  if bSql = true then
    begin 
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','SQLORACLE','1');
        Result := true;
    end
  else
    begin
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','SQLORACLE','0');
        Result := true;
  end
end;

function mapPage_NextButtonClick(Page: TWizardPage): Boolean;
var 
  bLoadMap : Boolean;
begin
  bLoadMap := radioMap.Checked;
  if bLoadMap = true then
    begin 
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','LOADMAP','1');
        Result := true;
    end
  else
    begin
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','LOADMAP','0');
        Result := true;
  end
end;

function carPage_NextButtonClick(Page: TWizardPage): Boolean;
var 
  bDrawCar : Boolean;
begin
  bDrawCar := radioCarYes.Checked;
  if bDrawCar = true then
    begin 
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','DRAWCAR','1');
        Result := true;
    end
  else
    begin
        RegWriteStringValue(HKEY_LOCAL_MACHINE,'SOFTWARE\Bekzoyo\HMQService','DRAWCAR','0');
        Result := true;
  end
end;

//控制页面是否允许跳过
//绘制车模型页面，只有当选中地图模式时才需要进行选择
function ShouldSkipPage(PageID: Integer): Boolean;
begin
  { initialize result to not skip any page (not necessary, but safer) }
  Result := False;
  { if the page that is asked to be skipped is your custom page, then... }
  if PageID = carPage.ID then
    { if the component is not selected, skip the page }
    Result := radioXm.Checked;  //选择项目模式，不需要进入车模型页面
end;

procedure InitializeWizard();
begin
       //考试科目类型选择页面
       kskmPage:=CreateCustomPage(wpSelectDir, '初始化设置', '设置考试科目');
       lbl:=TLabel.Create(kskmPage);
       lbl.Parent:=kskmPage.Surface;
       lbl.Caption:='请选择考试科目';

       radioKm2:=TRadioButton.Create(kskmPage);
       radioKm2.Parent:=kskmPage.Surface;
       radioKm2.Caption:='科目二';
       radioKm2.Top:=lbl.Top+30;
       radioKm2.Checked:=true;

       radioKm3:=TRadioButton.Create(kskmPage);
       radioKm3.Parent:=kskmPage.Surface;
       radioKm3.Caption:='科目三';
       radioKm3.Top:=radioKm2.Top+30;

       with kskmPage do
       begin
         OnNextButtonClick := @kskmPage_NextButtonClick;
       end;
       
       //解码设备类型选择页面
       jmsbPage:=CreateCustomPage(kskmPage.ID, '初始化设置', '设置解码设备类型');
       lb2:=TLabel.Create(jmsbPage);
       lb2.Parent:=jmsbPage.Surface;
       lb2.Caption:='请选择解码设备类型';

       radioHmq:=TRadioButton.Create(jmsbPage);
       radioHmq.Parent:=jmsbPage.Surface;
       radioHmq.Caption:='合码器';
       radioHmq.Top:=lb2.Top+30;
       radioHmq.Checked:=true;

       radioJmq:=TRadioButton.Create(jmsbPage);
       radioJmq.Parent:=jmsbPage.Surface;
       radioJmq.Caption:='解码器';
       radioJmq.Top:=radioHmq.Top+30;

       with jmsbPage do
       begin
         OnNextButtonClick := @jmsbPage_NextButtonClick;
       end;

       //数据库类型选择页面
       dbPage:=CreateCustomPage(jmsbPage.ID, '初始化设置', '设置数据库类型');
       lb3:=TLabel.Create(dbPage);
       lb3.Parent:=dbPage.Surface;
       lb3.Caption:='请选择数据库类型';

       radioSql:=TRadioButton.Create(dbPage);
       radioSql.Parent:=dbPage.Surface;
       radioSql.Caption:='SqlServer';
       radioSql.Top:=lb3.Top+30;
       radioSql.Checked:=true;

       radioOracle:=TRadioButton.Create(dbPage);
       radioOracle.Parent:=dbPage.Surface;
       radioOracle.Caption:='Oracle';
       radioOracle.Top:=radioSql.Top+30;

       with dbPage do
       begin
         OnNextButtonClick := @dbPage_NextButtonClick;
       end;

       //地图轨迹页面
       mapPage:=CreateCustomPage(dbPage.ID, '初始化设置', '设置考试实时信息展示页面');
       lb4:=TLabel.Create(mapPage);
       lb4.Parent:=mapPage.Surface;
       lb4.Caption:='请选择是否加载地图';

       radioMap:=TRadioButton.Create(mapPage);
       radioMap.Parent:=mapPage.Surface;
       radioMap.Caption:='是，加载地图';
       radioMap.Top:=lb4.Top+30;
       radioMap.Checked:=true;

       radioXm:=TRadioButton.Create(mapPage);
       radioXm.Parent:=mapPage.Surface;
       radioXm.Caption:='否，加载项目牌';
       radioXm.Top:=radioMap.Top+30;

       with mapPage do
       begin
         OnNextButtonClick := @mapPage_NextButtonClick;
       end;

       //配置是否绘制车模型
       carPage:=CreateCustomPage(mapPage.ID, '初始化设置', '设置车模型');
       lb5:=TLabel.Create(carPage);
       lb5.Parent:=carPage.Surface;
       lb5.Caption:='请选择是否绘制车模型';

       radioCarYes:=TRadioButton.Create(carPage);
       radioCarYes.Parent:=carPage.Surface;
       radioCarYes.Caption:='是';
       radioCarYes.Top:=lb5.Top+30;
       radioCarYes.Checked:=true;

       radioCarNo:=TRadioButton.Create(carPage);
       radioCarNo.Parent:=carPage.Surface;
       radioCarNo.Caption:='否';
       radioCarNo.Top:=radioCarYes.Top+30;

       with carPage do
       begin
         OnNextButtonClick := @carPage_NextButtonClick;
       end;
end;

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

  confPath: string;
begin  
if CurStep=ssDone then  
  begin

    //将配置写入配置文件 conf/HS_CONF_ENV.ini
    confPath := ExpandConstant('{app}') +'\\conf\\HS_CONF_ENV.ini';
    if radioKm2.Checked = true then begin
        SetIniString('CONFIG', 'KSKM', '2', confPath);
    end else begin
        SetIniString('CONFIG', 'KSKM', '3', confPath);
    end;
    if radioHmq.Checked = true then begin
        SetIniString('CONFIG', 'HMQ', '1', confPath);
    end else begin
        SetIniString('CONFIG', 'HMQ', '0', confPath);
    end;
    if radioSql.Checked = true then begin
        SetIniString('CONFIG', 'SQLORACLE', '1', confPath);
    end else begin
        SetIniString('CONFIG', 'SQLORACLE', '0', confPath);
    end;
    if radioMap.Checked = true then begin
        SetIniString('CONFIG', 'LOADMAP', '1', confPath);
        if radioCarYes.Checked = true then begin
            SetIniString('CONFIG', 'DRAWCAR', '1', confPath);
        end else begin
            SetIniString('CONFIG', 'DRAWCAR', '0', confPath);
        end
    end else begin
        SetIniString('CONFIG', 'LOADMAP', '0', confPath);
        SetIniString('CONFIG', 'DRAWCAR', '0', confPath);
    end;

  end;  
end;

