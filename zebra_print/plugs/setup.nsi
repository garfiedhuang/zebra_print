; �ýű�ʹ�� HM VNISEdit �ű��༭���򵼲���

; ��װ�����ʼ���峣��
!define PRODUCT_NAME "Xp Resin Print"
!define PRODUCT_VERSION "1.0.0.0"
!define PRODUCT_PUBLISHER "GuangZhou Xiaopeng Automobile Co., Ltd"
!define PRODUCT_WEB_SITE "http://mes.xiaopeng.local"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\Xp.Resin.Print.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

SetCompressor lzma

; ------ MUI �ִ����涨�� (1.67 �汾���ϼ���) ------
!include "MUI.nsh"

; MUI Ԥ���峣��
!define MUI_ABORTWARNING
!define MUI_ICON "D:\Work\Demo\RMES\zebra_print\xp.resin.print\Assets\Images\icon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; ��ӭҳ��
!insertmacro MUI_PAGE_WELCOME
; ��װĿ¼ѡ��ҳ��
!insertmacro MUI_PAGE_DIRECTORY
; ��װ����ҳ��
!insertmacro MUI_PAGE_INSTFILES
; ��װ���ҳ��
!define MUI_FINISHPAGE_RUN "$INSTDIR\Xp.Resin.Print.exe"
!insertmacro MUI_PAGE_FINISH

; ��װж�ع���ҳ��
!insertmacro MUI_UNPAGE_INSTFILES

; ��װ�����������������
!insertmacro MUI_LANGUAGE "SimpChinese"

; ��װԤ�ͷ��ļ�
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI �ִ����涨����� ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "Setup.exe"
InstallDir "$PROGRAMFILES\Xp Resin Print"
InstallDirRegKey HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show
BrandingText " "

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File "..\xp.resin.print\bin\Release\Xp.Resin.Print.pdb"
  CreateDirectory "$SMPROGRAMS\Xp Resin Print"
  CreateShortCut "$SMPROGRAMS\Xp Resin Print\Xp Resin Print.lnk" "$INSTDIR\Xp.Resin.Print.exe"
  CreateShortCut "$DESKTOP\Xp Resin Print.lnk" "$INSTDIR\Xp.Resin.Print.exe"
  File "..\xp.resin.print\bin\Release\Xp.Resin.Print.exe.config"
  File "..\xp.resin.print\bin\Release\Xp.Resin.Print.exe"
  File "..\xp.resin.print\bin\Release\WpfMultiStyle.dll"
  File "..\xp.resin.print\bin\Release\System.Windows.Interactivity.dll"
  File "..\xp.resin.print\bin\Release\System.ValueTuple.xml"
  File "..\xp.resin.print\bin\Release\System.ValueTuple.dll"
  File "..\xp.resin.print\bin\Release\NLog.xml"
  File "..\xp.resin.print\bin\Release\NLog.dll"
  File "..\xp.resin.print\bin\Release\NLog.config"
  File "..\xp.resin.print\bin\Release\Newtonsoft.Json.xml"
  File "..\xp.resin.print\bin\Release\Newtonsoft.Json.dll"
  File "..\xp.resin.print\bin\Release\HandyControl.xml"
  File "..\xp.resin.print\bin\Release\HandyControl.dll"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.xml"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.Platform.xml"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.Platform.pdb"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.Platform.dll"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.pdb"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.Extras.xml"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.Extras.pdb"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.Extras.dll"
  File "..\xp.resin.print\bin\Release\GalaSoft.MvvmLight.dll"
  File "..\xp.resin.print\bin\Release\EQuality.Tools.dll"
  File "..\xp.resin.print\bin\Release\CommonServiceLocator.dll"
  File "..\xp.resin.print\bin\Release\Fnthex32.dll"
  File /r "..\xp.resin.print\bin\Release\*.*"
SectionEnd

Section -AdditionalIcons
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\Xp Resin Print\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\Xp Resin Print\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\Xp.Resin.Print.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\Xp.Resin.Print.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

/******************************
 *  �����ǰ�װ�����ж�ز���  *
 ******************************/

Section Uninstall
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\CommonServiceLocator.dll"
  Delete "$INSTDIR\Fnthex32.dll"
  Delete "$INSTDIR\EQuality.Tools.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Extras.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Extras.pdb"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Extras.xml"
  Delete "$INSTDIR\GalaSoft.MvvmLight.pdb"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Platform.dll"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Platform.pdb"
  Delete "$INSTDIR\GalaSoft.MvvmLight.Platform.xml"
  Delete "$INSTDIR\GalaSoft.MvvmLight.xml"
  Delete "$INSTDIR\HandyControl.dll"
  Delete "$INSTDIR\HandyControl.xml"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\Newtonsoft.Json.xml"
  Delete "$INSTDIR\NLog.config"
  Delete "$INSTDIR\NLog.dll"
  Delete "$INSTDIR\NLog.xml"
  Delete "$INSTDIR\System.ValueTuple.dll"
  Delete "$INSTDIR\System.ValueTuple.xml"
  Delete "$INSTDIR\System.Windows.Interactivity.dll"
  Delete "$INSTDIR\WpfMultiStyle.dll"
  Delete "$INSTDIR\Xp.Resin.Print.exe"
  Delete "$INSTDIR\Xp.Resin.Print.exe.config"
  Delete "$INSTDIR\Xp.Resin.Print.pdb"

  Delete "$SMPROGRAMS\Xp Resin Print\Uninstall.lnk"
  Delete "$SMPROGRAMS\Xp Resin Print\Website.lnk"
  Delete "$DESKTOP\Xp Resin Print.lnk"
  Delete "$SMPROGRAMS\Xp Resin Print\Xp Resin Print.lnk"

  RMDir "$SMPROGRAMS\Xp Resin Print"
  
  RMDir /r "$INSTDIR\logs"
  RMDir /r "$INSTDIR\tr"
  RMDir /r "$INSTDIR\ru"
  RMDir /r "$INSTDIR\pt-BR"
  RMDir /r "$INSTDIR\pl"
  RMDir /r "$INSTDIR\ko-KR"
  RMDir /r "$INSTDIR\fr"
  RMDir /r "$INSTDIR\fa"
  RMDir /r "$INSTDIR\en"
  RMDir /r "$INSTDIR\Data"
  RMDir /r "$INSTDIR\ca-ES"

  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

#-- ���� NSIS �ű��༭�������� Function ���α�������� Section ����֮���д���Ա��ⰲװ�������δ��Ԥ֪�����⡣--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "��ȷʵҪ��ȫ�Ƴ� $(^Name) ���������е������" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) �ѳɹ��ش����ļ�����Ƴ���"
FunctionEnd
