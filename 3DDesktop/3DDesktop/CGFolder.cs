using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Runtime.InteropServices;

namespace _3DDesktop
{
[System.Runtime.InteropServices.ComVisible(false)]
public sealed class CGFolder : IDisposable
{

private static class NativeMethods
{

public static Guid IID_IShellFolder = new Guid(
"{000214E6-0000-0000-C000-000000000046}"
);

[DllImport("shell32.dll")]
public static extern Int32 SHGetDesktopFolder(
out IShellFolder ppshf
);

public enum SHCONTF
{
SHCONTF_FOLDERS = 0x0020,
SHCONTF_NONFOLDERS = 0x0040,
SHCONTF_INCLUDEHIDDEN = 0x0080,
SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
SHCONTF_NETPRINTERSRCH = 0x0200,
SHCONTF_SHAREABLE = 0x0400,
SHCONTF_STORAGE = 0x0800
}

[Flags]
public enum SHCIDS : uint
{
SHCIDS_ALLFIELDS = 0x80000000,
SHCIDS_CANONICALONLY = 0x10000000,
SHCIDS_BITMASK = 0xFFFF0000,
SHCIDS_COLUMNMASK = 0x0000FFFF
}

[Flags]
public enum FOLDERFLAGS
{
FWF_AUTOARRANGE = 0x1,
FWF_ABBREVIATEDNAMES = 0x2,
FWF_SNAPTOGRID = 0x4,
FWF_OWNERDATA = 0x8,
FWF_BESTFITWINDOW = 0x10,
FWF_DESKTOP = 0x20,
FWF_SINGLESEL = 0x40,
FWF_NOSUBFOLDERS = 0x80,
FWF_TRANSPARENT = 0x100,
FWF_NOCLIENTEDGE = 0x200,
FWF_NOSCROLL = 0x400,
FWF_ALIGNLEFT = 0x800,
FWF_NOICONS = 0x1000,
FWF_SHOWSELALWAYS = 0x2000,
FWF_NOVISIBLE = 0x4000,
FWF_SINGLECLICKACTIVATE = 0x8000,
FWF_NOWEBVIEW = 0x10000,
FWF_HIDEFILENAMES = 0x20000,
FWF_CHECKSELECT = 0x40000
}

public enum FOLDERVIEWMODE
{
FVM_FIRST = 1,
FVM_ICON = 1,
FVM_SMALLICON = 2,
FVM_LIST = 3,
FVM_DETAILS = 4,
FVM_THUMBNAIL = 5,
FVM_TILE = 6,
FVM_THUMBSTRIP = 7,
FVM_LAST = 7
}

[System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
public struct STRRET
{
[System.Runtime.InteropServices.FieldOffset(0)]
UInt32 uType;
[System.Runtime.InteropServices.FieldOffset(4)]
IntPtr pOleStr;
[System.Runtime.InteropServices.FieldOffset(4)]
IntPtr pStr;
[FieldOffset(4)]
UInt32 uOffset;
[FieldOffset(4)]
IntPtr cStr;
}

[Flags()]
public enum SFGAO : uint
{
SFGAO_CANCOPY = 0x000000001,
SFGAO_CANMOVE = 0x000000002,
SFGAO_CANLINK = 0x000000004,
SFGAO_STORAGE = 0x000000008,
SFGAO_CANRENAME = 0x00000010,
SFGAO_CANDELETE = 0x00000020,
SFGAO_HASPROPSHEET = 0x00000040,
SFGAO_DROPTARGET = 0x00000100,
SFGAO_CAPABILITYMASK = 0x00000177,
SFGAO_ENCRYPTED = 0x00002000,
SFGAO_ISSLOW = 0x00004000,
SFGAO_GHOSTED = 0x00008000,
SFGAO_LINK = 0x00010000,
SFGAO_SHARE = 0x00020000,
SFGAO_READONLY = 0x00040000,
SFGAO_HIDDEN = 0x00080000,
SFGAO_DISPLAYATTRMASK = 0x000FC000,
SFGAO_FILESYSANCESTOR = 0x10000000,
SFGAO_FOLDER = 0x20000000,
SFGAO_FILESYSTEM = 0x40000000,
SFGAO_HASSUBFOLDER = 0x80000000,
SFGAO_CONTENTSMASK = 0x80000000,
SFGAO_VALIDATE = 0x01000000,
SFGAO_REMOVABLE = 0x02000000,
SFGAO_COMPRESSED = 0x04000000,
SFGAO_BROWSABLE = 0x08000000,
SFGAO_NONENUMERATED = 0x00100000,
SFGAO_NEWCONTENT = 0x00200000,
SFGAO_CANMONIKER = 0x00400000,
SFGAO_HASSTORAGE = 0x00400000,
SFGAO_STREAM = 0x00400000,
SFGAO_STORAGEANCESTOR = 0x00800000,
SFGAO_STORAGECAPMASK = 0x70C50008
}

[Flags()]
public enum SHGDN
{
SHGDN_NORMAL = 0,
SHGDN_INFOLDER = 1,
SHGDN_FORADDRESSBAR = 16384,
SHGDN_FORPARSING = 32768
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
public int left;
public int top;
public int right;
public int bottom;

public RECT(
Rectangle r
)
{
left = r.Left;
top = r.Top;
right = r.Right;
bottom = r.Bottom;
}

}

[StructLayout(LayoutKind.Sequential)]
public struct FOLDERSETTINGS
{
public FOLDERFLAGS ViewMode;
public FOLDERVIEWMODE fFlags;
}

public enum SVUIA_STATUS
{
SVUIA_DEACTIVATE = 0,
SVUIA_ACTIVATE_NOFOCUS = 1,
SVUIA_ACTIVATE_FOCUS = 2,
SVUIA_INPLACEACTIVATE = 3
}

[ComImportAttribute()]
[GuidAttribute(
"000214F2-0000-0000-C000-000000000046")]
[InterfaceTypeAttribute(
ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumIDList
{
[PreserveSig]
int Next(
int celt,
ref IntPtr
rgelt, out
int pceltFetched
);
[PreserveSig]
int Skip(int celt);
[PreserveSig]
int Reset();
[PreserveSig]
int Clone(
ref IEnumIDList ppenum
);
}

[ComImport()]
[Guid(
"000214E2-0000-0000-C000-000000000046")]
[InterfaceType(
ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellBrowser
{
[PreserveSig]
int GetWindow(out IntPtr phwnd);
[PreserveSig]
int ContextSensitiveHelp(bool fEnterMode);
[PreserveSig]
int InsertMenusSB(
IntPtr hmenuShared,
ref IntPtr lpMenuWidths
);
[PreserveSig]
int SetMenuSB(
IntPtr hmenuShared,
IntPtr holemenuRes,
IntPtr hwndActiveObject
);
[PreserveSig]
int RemoveMenusSB(
IntPtr hmenuShared
);
[PreserveSig]
int SetStatusTextSB(
string pszStatusText
);
[PreserveSig]
int EnableModelessSB(
bool fEnable
);
[PreserveSig]
int TranslateAcceleratorSB(
IntPtr pmsg,
short wID
);
[PreserveSig]
int BrowseObject(
IntPtr pidl,
uint wFlags
);
[PreserveSig]
int GetViewStateStream(
long grfMode,
ref UCOMIStream ppStrm
);
[PreserveSig]
int GetControlWindow(
uint id,
ref IntPtr phwnd
);
[PreserveSig]
int SendControlMsg(
uint id,
uint uMsg,
short wParam,
long lParam,
ref long pret
);
[PreserveSig]
int QueryActiveShellView(
ref IShellView ppshv
);
[PreserveSig]
int OnViewWindowActive(
IShellView pshv
);
[PreserveSig]
int SetToolbarItems(
IntPtr lpButtons,
uint nButtons,
uint uFlags
);
}

[ComImport()]
[Guid("000214E3-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown )]
public interface IShellView
{
[PreserveSig]
int GetWindow(out IntPtr phwnd);
[PreserveSig]
int ContextSensitiveHelp(bool fEnterMode);
[PreserveSig]
int TranslateAcceleratorA(IntPtr pmsg);
[PreserveSig]
int EnableModeless(bool fEnable);
[PreserveSig]
int UIActivate(SVUIA_STATUS uState);
[PreserveSig]
int Refresh();
[PreserveSig]
int CreateViewWindow(
IShellView psvPrevious,
ref FOLDERSETTINGS pfs,
IShellBrowser psb,
ref RECT prcView,
out IntPtr phWnd
);
[PreserveSig]
int DestroyViewWindow();
[PreserveSig]
int GetCurrentInfo(ref FOLDERSETTINGS pfs);
[PreserveSig]
int AddPropertySheetPages(
long dwReserved,
ref IntPtr pfnPtr,
int lparam
);
[PreserveSig]
int SaveViewState();
[PreserveSig]
int SelectItem(IntPtr pidlItem, uint uFlags);
[PreserveSig]
int GetItemObject(
uint uItem,
ref Guid riid,
ref IntPtr ppv
);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown )]
[Guid("000214E6-0000-0000-C000-000000000046")]
public interface IShellFolder
{
[PreserveSig]
Int32 ParseDisplayName(
IntPtr hwnd,
IntPtr pbc,
[MarshalAs(UnmanagedType.LPWStr)]
string pszDisplayName,
ref UInt32 pchEaten,
out IntPtr ppidl,
ref UInt32 pdwAttributes
);
[PreserveSig]
Int32 EnumObjects(
IntPtr hwnd,
SHCONTF grfFlags,
out IEnumIDList ppenumIDList
);
[PreserveSig]
Int32 BindToObject(
IntPtr pidl,
IntPtr pbc,
ref Guid riid,
out IntPtr ppv
);
[PreserveSig]
Int32 BindToStorage(
IntPtr pidl,
IntPtr pbc,
ref Guid riid,
out IntPtr ppv
);
[PreserveSig]
Int32 CompareIDs(
SHCIDS lParam,
IntPtr pidl1,
IntPtr pidl2
);
[PreserveSig]
Int32 CreateViewObject(
IntPtr hwndOwner,
ref Guid riid,
out IShellView ppv
);
[PreserveSig]
Int32 GetAttributesOf(
UInt32 cidl,
[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
IntPtr[] apidl,
ref SFGAO rgfInOut
);
[PreserveSig]
Int32 GetUIObjectOf(
IntPtr hwndOwner,
UInt32 cidl,
IntPtr[] apidl,
Guid riid,
ref UInt32 rgfReserved,
out IntPtr ppv
);
[PreserveSig]
Int32 GetDisplayNameOf(
IntPtr pidl,
SHGDN uFlags,
out STRRET pName
);
[PreserveSig]
Int32 SetNameOf(
IntPtr hwnd,
IntPtr pidl,
[MarshalAs(UnmanagedType.LPWStr)]
string pszName,
UInt32 uFlags,
out IntPtr ppidlOut
);
}
}

private static readonly NativeMethods.IShellFolder
c_desktopFolder;
private NativeMethods.IShellFolder m_folder;
private CGPidl m_pidl;

public CGPidl Pidl
{
get { return m_pidl; }
}

public object Interface
{
get { return m_folder; }
}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
static CGFolder()
{

// Get a reference to the desktop folder.
NativeMethods.SHGetDesktopFolder(
out c_desktopFolder
);

}

public CGFolder()
{

}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public CGFolder(
Environment.SpecialFolder specialFolder
)
{
Open(specialFolder);
}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public CGFolder(
string fullPath
)
{
Open(fullPath);
}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public CGFolder(
CGPidl pidl
)
{
Open(pidl);
}

public void Dispose()
{
Close();
}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public void Open(
Environment.SpecialFolder specialFolder
)
{

// Free any open resources.
Close();

// Create the identifier.
m_pidl = new CGPidl(specialFolder);

// Create the folder reference.
InitializeFolder();

}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public void Open(
string fullPath
)
{

// Create the identifier.
m_pidl = new CGPidl(fullPath);

// Create the folder reference.
InitializeFolder();

}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public void Open(
CGPidl pidl
)
{

// Clone the identifier.
m_pidl = (CGPidl)pidl.Clone();

// Create the folder reference.
InitializeFolder();

}

public void Close()
{

// Should we cleanup the identifier?
if (m_pidl != null)
m_pidl.Dispose();

m_pidl = null;

// Should we cleanup the folder?
if (m_folder != null)
Marshal.ReleaseComObject(m_folder);

m_folder = null;

}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
public System.Collections.ArrayList GetChildren(
bool showHiddenObjects,
bool showNonFolders,
bool sortResults
)
{

ArrayList children = new ArrayList();
NativeMethods.IEnumIDList enumList = null;

try
{
// Sanity check the folder before attempting to use it.
if (m_pidl == null || !m_pidl.IsFolder || m_folder == null)
return children;

// Get the enumerator for the object.
int hr = m_folder.EnumObjects(
IntPtr.Zero,
NativeMethods.SHCONTF.SHCONTF_FOLDERS |
(showNonFolders ?
NativeMethods.SHCONTF.SHCONTF_NONFOLDERS : 0) |
(showHiddenObjects ?
NativeMethods.SHCONTF.SHCONTF_INCLUDEHIDDEN : 0),
out enumList
);

// Did we fail?
if (hr != 0)
Marshal.ThrowExceptionForHR(hr);

IntPtr pidl = IntPtr.Zero;
int fetched = 0;

// Loop and walk through the child objects.
while (enumList.Next(1, ref pidl, out fetched) == 0 &&
fetched == 1)
{

// Create a new identifier for the object.
CGPidl child = new CGPidl(
m_pidl.Pidl,
pidl
);

// Add the object to the array.
children.Add(child);

} // End while there are more child objects.

// Should we sort the results?
if (sortResults)
children.Sort();

} // End try

finally
{

// Should we cleanup the enumerator reference?
if (enumList != null)
Marshal.ReleaseComObject(enumList);

enumList = null;

} // End finally

// Return the list of children.
return children;

}

[SecurityPermission(SecurityAction.LinkDemand,
Flags = SecurityPermissionFlag.UnmanagedCode)]
private void InitializeFolder()
{

// Should we simply obtain a reference to the desktop?
if (m_pidl.IsDesktop)
NativeMethods.SHGetDesktopFolder(
out m_folder
);
else
{

NativeMethods.IShellFolder parentFolder = null;

try
{

IntPtr ptr;

// Get a reference to the folder.
int hr = c_desktopFolder.BindToObject(
m_pidl.Pidl,
IntPtr.Zero,
ref NativeMethods.IID_IShellFolder,
out ptr
);

// Did we fail?
if (hr != 0)
Marshal.ThrowExceptionForHR(hr);

// Unwrap the inteface.
m_folder =
(NativeMethods.IShellFolder)
Marshal.GetTypedObjectForIUnknown(
ptr,
typeof(NativeMethods.IShellFolder)
);

} // End try

finally
{

// Should we cleanup the parent folder?
if (parentFolder != null)
Marshal.ReleaseComObject(parentFolder);

parentFolder = null;

} // End finally

} // End else we should handle the general case.
}
}


[ComVisible(false)]
public sealed class CGPidl : IDisposable, IComparable, ICloneable
{

private static class NativeMethods
{

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
public int left;
public int top;
public int right;
public int bottom;

public RECT(
Rectangle r
)
{
left = r.Left;
top = r.Top;
right = r.Right;
bottom = r.Bottom;
}

}

[Flags]
public enum CSIDL
{
CSIDL_FLAG_CREATE = 0x8000,
CSIDL_ADMINTOOLS = 0x0030,
CSIDL_ALTSTARTUP = 0x001d,
CSIDL_APPDATA = 0x001a,
CSIDL_BITBUCKET = 0x000a,
CSIDL_CDBURN_AREA = 0x003b,
CSIDL_COMMON_ADMINTOOLS = 0x002f,
CSIDL_COMMON_ALTSTARTUP = 0x001e,
CSIDL_COMMON_APPDATA = 0x0023,
CSIDL_COMMON_DESKTOPDIRECTORY = 0x0019,
CSIDL_COMMON_DOCUMENTS = 0x002e,
CSIDL_COMMON_FAVORITES = 0x001f,
CSIDL_COMMON_MUSIC = 0x0035,
CSIDL_COMMON_PICTURES = 0x0036,
CSIDL_COMMON_PROGRAMS = 0x0017,
CSIDL_COMMON_STARTMENU = 0x0016,
CSIDL_COMMON_STARTUP = 0x0018,
CSIDL_COMMON_TEMPLATES = 0x002d,
CSIDL_COMMON_VIDEO = 0x0037,
CSIDL_CONTROLS = 0x0003,
CSIDL_COOKIES = 0x0021,
CSIDL_DESKTOP = 0x0000,
CSIDL_DESKTOPDIRECTORY = 0x0010,
CSIDL_DRIVES = 0x0011,
CSIDL_FAVORITES = 0x0006,
CSIDL_FONTS = 0x0014,
CSIDL_HISTORY = 0x0022,
CSIDL_INTERNET = 0x0001,
CSIDL_INTERNET_CACHE = 0x0020,
CSIDL_LOCAL_APPDATA = 0x001c,
CSIDL_MYDOCUMENTS = 0x000c,
CSIDL_MYMUSIC = 0x000d,
CSIDL_MYPICTURES = 0x0027,
CSIDL_MYVIDEO = 0x000e,
CSIDL_NETHOOD = 0x0013,
CSIDL_NETWORK = 0x0012,
CSIDL_PERSONAL = 0x0005,
CSIDL_PRINTERS = 0x0004,
CSIDL_PRINTHOOD = 0x001b,
CSIDL_PROFILE = 0x0028,
CSIDL_PROFILES = 0x003e,
CSIDL_PROGRAM_FILES = 0x0026,
CSIDL_PROGRAM_FILES_COMMON = 0x002b,
CSIDL_PROGRAMS = 0x0002,
CSIDL_RECENT = 0x0008,
CSIDL_SENDTO = 0x0009,
CSIDL_STARTMENU = 0x000b,
CSIDL_STARTUP = 0x0007,
CSIDL_SYSTEM = 0x0025,
CSIDL_TEMPLATES = 0x0015,
CSIDL_WINDOWS = 0x0024
}

[StructLayout(LayoutKind.Sequential)]
public struct SHFILEINFO
{
public IntPtr hIcon;
public Int32 iIcon;
public UInt32 dwAttributes;
[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
public string szDisplayName;
[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
public string szTypeName;
};

public enum SHCONTF
{
SHCONTF_FOLDERS = 0x0020,
SHCONTF_NONFOLDERS = 0x0040,
SHCONTF_INCLUDEHIDDEN = 0x0080,
SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
SHCONTF_NETPRINTERSRCH = 0x0200,
SHCONTF_SHAREABLE = 0x0400,
SHCONTF_STORAGE = 0x0800
}

[Flags]
public enum SHGFI
{
SHGFI_ICON = 0x000000100,
SHGFI_DISPLAYNAME = 0x000000200,
SHGFI_TYPENAME = 0x000000400,
SHGFI_ATTRIBUTES = 0x000000800,
SHGFI_ICONLOCATION = 0x000001000,
SHGFI_EXETYPE = 0x000002000,
SHGFI_SYSICONINDEX = 0x000004000,
SHGFI_LINKOVERLAY = 0x000008000,
SHGFI_SELECTED = 0x000010000,
SHGFI_ATTR_SPECIFIED = 0x000020000,
SHGFI_LARGEICON = 0x000000000,
SHGFI_SMALLICON = 0x000000001,
SHGFI_OPENICON = 0x000000002,
SHGFI_SHELLICONSIZE = 0x000000004,
SHGFI_PIDL = 0x000000008,
SHGFI_USEFILEATTRIBUTES = 0x000000010,
SHGFI_ADDOVERLAYS = 0x000000020,
SHGFI_OVERLAYINDEX = 0x000000040
}

[Flags]
public enum SHCIDS : uint
{
SHCIDS_ALLFIELDS = 0x80000000,
SHCIDS_CANONICALONLY = 0x10000000,
SHCIDS_BITMASK = 0xFFFF0000,
SHCIDS_COLUMNMASK = 0x0000FFFF
}

[Flags()]
public enum SFGAO : uint
{
SFGAO_CANCOPY = 0x000000001,
SFGAO_CANMOVE = 0x000000002,
SFGAO_CANLINK = 0x000000004,
SFGAO_STORAGE = 0x000000008,
SFGAO_CANRENAME = 0x00000010,
SFGAO_CANDELETE = 0x00000020,
SFGAO_HASPROPSHEET = 0x00000040,
SFGAO_DROPTARGET = 0x00000100,
SFGAO_CAPABILITYMASK = 0x00000177,
SFGAO_ENCRYPTED = 0x00002000,
SFGAO_ISSLOW = 0x00004000,
SFGAO_GHOSTED = 0x00008000,
SFGAO_LINK = 0x00010000,
SFGAO_SHARE = 0x00020000,
SFGAO_READONLY = 0x00040000,
SFGAO_HIDDEN = 0x00080000,
SFGAO_DISPLAYATTRMASK = 0x000FC000,
SFGAO_FILESYSANCESTOR = 0x10000000,
SFGAO_FOLDER = 0x20000000,
SFGAO_FILESYSTEM = 0x40000000,
SFGAO_HASSUBFOLDER = 0x80000000,
SFGAO_CONTENTSMASK = 0x80000000,
SFGAO_VALIDATE = 0x01000000,
SFGAO_REMOVABLE = 0x02000000,
SFGAO_COMPRESSED = 0x04000000,
SFGAO_BROWSABLE = 0x08000000,
SFGAO_NONENUMERATED = 0x00100000,
SFGAO_NEWCONTENT = 0x00200000,
SFGAO_CANMONIKER = 0x00400000,
SFGAO_HASSTORAGE = 0x00400000,
SFGAO_STREAM = 0x00400000,
SFGAO_STORAGEANCESTOR = 0x00800000,
SFGAO_STORAGECAPMASK = 0x70C50008
}

[Flags()]
public enum SHGDN
{
SHGDN_NORMAL = 0,
SHGDN_INFOLDER = 1,
SHGDN_FORADDRESSBAR = 16384,
SHGDN_FORPARSING = 32768
}

[StructLayout(LayoutKind.Explicit)]
public struct STRRET
{
[FieldOffset(0)]
UInt32 uType;
[FieldOffset(4)]
IntPtr pOleStr;
[FieldOffset(4)]
IntPtr pStr;
[FieldOffset(4)]
UInt32 uOffset;
[FieldOffset(4)]
IntPtr cStr;
}

public enum SVUIA_STATUS
{
SVUIA_DEACTIVATE = 0,
SVUIA_ACTIVATE_NOFOCUS = 1,
SVUIA_ACTIVATE_FOCUS = 2,
SVUIA_INPLACEACTIVATE = 3
}

[StructLayout(LayoutKind.Sequential)]
public struct FOLDERSETTINGS
{
public FOLDERFLAGS ViewMode;
public FOLDERVIEWMODE fFlags;
}

[Flags]
public enum FOLDERFLAGS
{
FWF_AUTOARRANGE = 0x1,
FWF_ABBREVIATEDNAMES = 0x2,
FWF_SNAPTOGRID = 0x4,
FWF_OWNERDATA = 0x8,
FWF_BESTFITWINDOW = 0x10,
FWF_DESKTOP = 0x20,
FWF_SINGLESEL = 0x40,
FWF_NOSUBFOLDERS = 0x80,
FWF_TRANSPARENT = 0x100,
FWF_NOCLIENTEDGE = 0x200,
FWF_NOSCROLL = 0x400,
FWF_ALIGNLEFT = 0x800,
FWF_NOICONS = 0x1000,
FWF_SHOWSELALWAYS = 0x2000,
FWF_NOVISIBLE = 0x4000,
FWF_SINGLECLICKACTIVATE = 0x8000,
FWF_NOWEBVIEW = 0x10000,
FWF_HIDEFILENAMES = 0x20000,
FWF_CHECKSELECT = 0x40000
}

public enum FOLDERVIEWMODE
{
FVM_FIRST = 1,
FVM_ICON = 1,
FVM_SMALLICON = 2,
FVM_LIST = 3,
FVM_DETAILS = 4,
FVM_THUMBNAIL = 5,
FVM_TILE = 6,
FVM_THUMBSTRIP = 7,
FVM_LAST = 7
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown )]
[Guid("000214E6-0000-0000-C000-000000000046")]
public interface IShellFolder
{
[PreserveSig]
Int32 ParseDisplayName(
IntPtr hwnd,
IntPtr pbc,
[MarshalAs(UnmanagedType.LPWStr)]
string pszDisplayName,
ref UInt32 pchEaten,
out IntPtr ppidl,
ref UInt32 pdwAttributes
);
[PreserveSig]
Int32 EnumObjects(
IntPtr hwnd,
SHCONTF grfFlags,
out IEnumIDList ppenumIDList
);
[PreserveSig]
Int32 BindToObject(
IntPtr pidl,
IntPtr pbc,
ref Guid riid,
out IntPtr ppv
);
[PreserveSig]
Int32 BindToStorage(
IntPtr pidl,
IntPtr pbc,
ref Guid riid,
out IntPtr ppv
);
[PreserveSig]
Int32 CompareIDs(
SHCIDS lParam,
IntPtr pidl1,
IntPtr pidl2
);
[PreserveSig]
Int32 CreateViewObject(
IntPtr hwndOwner,
ref Guid riid,
out IShellView ppv
);
[PreserveSig]
Int32 GetAttributesOf(
UInt32 cidl,
[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
IntPtr[] apidl,
ref SFGAO rgfInOut
);
[PreserveSig]
Int32 GetUIObjectOf(
IntPtr hwndOwner,
UInt32 cidl,
IntPtr[] apidl,
Guid riid,
ref UInt32 rgfReserved,
out IntPtr ppv
);
[PreserveSig]
Int32 GetDisplayNameOf(
IntPtr pidl,
SHGDN uFlags,
out STRRET pName
);
[PreserveSig]
Int32 SetNameOf(
IntPtr hwnd,
IntPtr pidl,
[MarshalAs(UnmanagedType.LPWStr)]
string pszName,
UInt32 uFlags,
out IntPtr ppidlOut
);
}

[ComImportAttribute()]
[GuidAttribute("000214F2-0000-0000-C000-000000000046")]
[InterfaceTypeAttribute(
ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumIDList
{
[PreserveSig]
int Next(
int celt,
ref IntPtr rgelt,
out int pceltFetched
);
[PreserveSig]
int Skip(int celt);
[PreserveSig]
int Reset();
[PreserveSig]
int Clone(ref IEnumIDList ppenum);
}

[ComImport()]
[Guid("000214E3-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown )]
public interface IShellView
{
[PreserveSig]
int GetWindow(out IntPtr phwnd);
[PreserveSig]
int ContextSensitiveHelp(bool fEnterMode);
[PreserveSig]
int TranslateAcceleratorA(IntPtr pmsg);
[PreserveSig]
int EnableModeless(bool fEnable);
[PreserveSig]
int UIActivate(SVUIA_STATUS uState);
[PreserveSig]
int Refresh();
[PreserveSig]
int CreateViewWindow(
IShellView psvPrevious,
ref FOLDERSETTINGS pfs,
IShellBrowser psb,
ref RECT prcView,
out IntPtr phWnd
);
[PreserveSig]
int DestroyViewWindow();
[PreserveSig]
int GetCurrentInfo(
ref FOLDERSETTINGS pfs
);
[PreserveSig]
int AddPropertySheetPages(
long dwReserved,
ref IntPtr pfnPtr,
int lparam
);
[PreserveSig]
int SaveViewState();
[PreserveSig]
int SelectItem(
IntPtr pidlItem,
uint uFlags
);
[PreserveSig]
int GetItemObject(
uint uItem,
ref Guid riid,
ref IntPtr ppv
);
}

[ComImport()]
[Guid("000214E2-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown )]
public interface IShellBrowser
{
[PreserveSig]
int GetWindow(out IntPtr phwnd);
[PreserveSig]
int ContextSensitiveHelp(bool fEnterMode);
[PreserveSig]
int InsertMenusSB(
IntPtr hmenuShared,
ref IntPtr lpMenuWidths
);
[PreserveSig]
int SetMenuSB(
IntPtr hmenuShared,
IntPtr holemenuRes,
IntPtr hwndActiveObject
);
[PreserveSig]
int RemoveMenusSB(IntPtr hmenuShared);
[PreserveSig]
int SetStatusTextSB(string pszStatusText);
[PreserveSig]
int EnableModelessSB(bool fEnable);
[PreserveSig]
int TranslateAcceleratorSB(
IntPtr pmsg,
short wID
);
[PreserveSig]
int BrowseObject(
IntPtr pidl,
uint wFlags
);
[PreserveSig]
int GetViewStateStream(
long grfMode,
ref UCOMIStream ppStrm
);
[PreserveSig]
int GetControlWindow(uint id, ref IntPtr phwnd);
[PreserveSig]
int SendControlMsg(
uint id,
uint uMsg,
short wParam,
long lParam,
ref long pret
);
[PreserveSig]
int QueryActiveShellView(
ref IShellView ppshv
);
[PreserveSig]
int OnViewWindowActive(
IShellView pshv
);
[PreserveSig]
int SetToolbarItems(
IntPtr lpButtons,
uint nButtons,
uint uFlags
);
}

[DllImport("shell32.dll", EntryPoint = "#18")]
public static extern IntPtr ILClone(
IntPtr pidl
);

[DllImport("shell32.dll", EntryPoint = "#25")]
public static extern IntPtr ILCombine(
IntPtr pidlA,
IntPtr pidlB
);

[DllImport("shell32.dll", EntryPoint = "#17")]
public static extern bool ILRemoveLastID(
IntPtr pidl
);

[DllImport("shell32.dll")]
public static extern Int32 SHGetDesktopFolder(
out IShellFolder ppshf
);

[DllImport("shell32.dll")]
public static extern Int32 SHGetFolderLocation(
IntPtr hwndOwner,
CSIDL nFolder,
IntPtr hToken,
UInt32 dwReserved,
out IntPtr ppidl
);

[DllImport("shell32.dll")]
public static extern IntPtr SHGetFileInfo(
string fileName,
uint dwFileAttributes,
ref SHFILEINFO psfi,
int cbSizeFileInfo,
SHGFI flags
);

[DllImport("shell32.dll")]
public static extern IntPtr SHGetFileInfo(
IntPtr pidl,
uint dwFileAttributes,
ref SHFILEINFO psfi,
int cbSizeFileInfo,
SHGFI flags
);

[DllImport("shell32.dll")]
public static extern bool SHGetPathFromIDList(
IntPtr pidl,
StringBuilder pszPath
);



}

private static readonly
NativeMethods.IShellFolder c_desktopFolder;
private static readonly IntPtr c_desktopPidl;
private IntPtr m_pidl;
private string m_displayName;
private string m_typeName;
private string m_physicalPath;
private NativeMethods.SFGAO m_attributes;

public IntPtr Pidl
{
get {return m_pidl;}
}

public string DisplayName
{
get {return m_displayName;}
}

public string TypeName
{
get {return m_typeName;}
}

public string PhysicalPath
{
get {return m_physicalPath;}
}

public bool CanRename
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_CANRENAME) != 0;
}
}

public bool CanMove
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_CANMOVE) != 0;
}
}

public bool CanDelete
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_CANDELETE) != 0;
}
}

public bool CanCopy
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_CANCOPY) != 0;
}
}

public bool IsReadOnly
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_READONLY) != 0;
}
}

public bool IsEncrypted
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_ENCRYPTED) != 0;
}
}

public bool IsLink
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_LINK) != 0;
}
}

public bool IsHidden
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_HIDDEN) != 0;
}
}

public bool IsSlow
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_ISSLOW) != 0;
}
}

public bool IsGhosted
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_GHOSTED) != 0;
}
}

public bool IsCompressed
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_COMPRESSED) != 0;
}
}

public bool IsRemovable
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_REMOVABLE) != 0;
}
}

public bool IsShared
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_SHARE) != 0;
}
}

public bool IsFolder
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_FOLDER) != 0;
}
}

public bool IsFileSystem
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_FILESYSTEM) != 0;
}
}

public bool HasSubfolders
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_HASSUBFOLDER) != 0;
}
}

public bool IsBrowsable
{
get {
return (m_attributes &
NativeMethods.SFGAO.SFGAO_BROWSABLE) != 0;
}
}

public bool IsDesktop
{
get
{

// Ask the desktop to determine if they are equal.
return c_desktopFolder.CompareIDs(
NativeMethods.SHCIDS.SHCIDS_CANONICALONLY,
m_pidl,
c_desktopPidl
) == 0;

} // End get

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
static CGPidl()
{

// Get a reference to the desktop folder.
NativeMethods.SHGetDesktopFolder(
out c_desktopFolder
);

IntPtr hToken = new IntPtr(-1);

// Get the pidl for the desktop folder.
int hr = NativeMethods.SHGetFolderLocation(
IntPtr.Zero,
NativeMethods.CSIDL.CSIDL_DESKTOP,
hToken,
0,
out c_desktopPidl
);

}

public CGPidl()
{

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public CGPidl(
IntPtr pidl
)
{
Open(pidl);
}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public CGPidl(
IntPtr parentPidl,
IntPtr pidl
)
{
Open(parentPidl, pidl);
}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public CGPidl(
Environment.SpecialFolder specialFolder
)
{
Open(specialFolder);
}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public CGPidl(
string fullPath
)
{
Open(fullPath);
}

public void Dispose()
{
Close();
}

public override bool Equals(object obj)
{

// Recover a reference to the CGPidl object.
CGPidl pidl = obj as CGPidl;

// Sanity check the type first.
if (pidl == null)
return false;

// Ask the desktop to determine if they are equal.
return c_desktopFolder.CompareIDs(
NativeMethods.SHCIDS.SHCIDS_CANONICALONLY,
m_pidl,
pidl.m_pidl
) == 0;

}

public override int GetHashCode()
{
return base.GetHashCode() ^ m_pidl.GetHashCode();
}

public void Open(
IntPtr pidl
)
{

// Clone to pidl.
m_pidl = NativeMethods.ILClone(pidl);

// Initialize the object.
InitializeObject();

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public void Open(
IntPtr parentPidl,
IntPtr pidl
)
{

// Create a fully qualified pidl for the object.
m_pidl = NativeMethods.ILCombine(
parentPidl,
pidl
);

// Initialize the object.
InitializeObject();

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public void Open(
Environment.SpecialFolder specialFolder
)
{

IntPtr hToken = new IntPtr(-1);

// Get the pidl for the special folder.
int hr = NativeMethods.SHGetFolderLocation(
IntPtr.Zero,
SpecialFolderToCSIDL(specialFolder),
hToken,
0,
out m_pidl
);

// Did we fail?
if (hr != 0)
Marshal.ThrowExceptionForHR(hr);

// Initialize the object.
InitializeObject();

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public void Open(
string fullPath
)
{

uint attr = 0;
uint pchEaten = 0;

// Attempt to get a pidl to the object.
int hr = c_desktopFolder.ParseDisplayName(
IntPtr.Zero,
IntPtr.Zero,
fullPath,
ref pchEaten,
out m_pidl,
ref attr
);

// Did we fail?
if (hr != 0)
Marshal.ThrowExceptionForHR(hr);

// Initialize the object.
InitializeObject();

}

public void Close()
{

// Should we cleanup the pidl?
if (m_pidl != IntPtr.Zero)
Marshal.FreeCoTaskMem(m_pidl);

m_pidl = IntPtr.Zero;

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public CGPidl GetParentFolder()
{

// Sanity check the pidl before attempting to use it.
if (m_pidl == IntPtr.Zero)
return null;

// The desktop folder doesn't have a parent.
if (c_desktopFolder.CompareIDs(
NativeMethods.SHCIDS.SHCIDS_ALLFIELDS,
m_pidl,
c_desktopPidl
) == 0)
return null;

// Start by copying our pidl since we are about to
// modify it to create a pidl for the parent.
IntPtr parentPidl = NativeMethods.ILClone(m_pidl);

// If we failed to remove the last item in the PIDL then
// there really isn't any way to recover.
if (!NativeMethods.ILRemoveLastID(parentPidl))
{

// Free the cloned pidl.
Marshal.FreeCoTaskMem(parentPidl);
parentPidl = IntPtr.Zero;

return null;

} // End if we failed to locate the parent.

// Otherwise, create a CGPidl and return it.
return new CGPidl(parentPidl);

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
public System.Collections.ArrayList GetAncestors()
{

ArrayList list = new ArrayList();

// Sanity check the pidl before attempting to use it.
if (m_pidl == IntPtr.Zero)
return list;

CGPidl pidl = (CGPidl)Clone();

// Loop and find the ancestors.
while (pidl != null)
{

list.Add(pidl);
pidl = pidl.GetParentFolder();

} // End while there are more ancestors.

// Return the family tree.
return list;

}

[SecurityPermission(
SecurityAction.LinkDemand,
Flags=SecurityPermissionFlag.UnmanagedCode)]
private void InitializeObject()
{

NativeMethods.SHFILEINFO shfi =
new NativeMethods.SHFILEINFO();

// Attempt to get the information for the shell object.
NativeMethods.SHGetFileInfo(
m_pidl,
0,
ref shfi,
Marshal.SizeOf(shfi),
NativeMethods.SHGFI.SHGFI_PIDL |
NativeMethods.SHGFI.SHGFI_DISPLAYNAME |
NativeMethods.SHGFI.SHGFI_ATTRIBUTES |
NativeMethods.SHGFI.SHGFI_TYPENAME
);

// Save the information.
m_displayName = shfi.szDisplayName;
m_typeName = shfi.szTypeName;
m_attributes = (NativeMethods.SFGAO)shfi.dwAttributes;

StringBuilder sb = new StringBuilder(260);

// Get the physical path to the shell object.
NativeMethods.SHGetPathFromIDList(
m_pidl,
sb
);

// Save the path.
m_physicalPath = sb.ToString();

}

private static NativeMethods.CSIDL SpecialFolderToCSIDL(
Environment.SpecialFolder sf
)
{

switch (sf)
{

case Environment.SpecialFolder.ApplicationData :

return NativeMethods.CSIDL.CSIDL_APPDATA;

case Environment.SpecialFolder.CommonApplicationData :

return NativeMethods.CSIDL.CSIDL_COMMON_APPDATA;

case Environment.SpecialFolder.CommonProgramFiles :

return NativeMethods.CSIDL.CSIDL_COMMON_PROGRAMS;

case Environment.SpecialFolder.Cookies :

return NativeMethods.CSIDL.CSIDL_COOKIES;

case Environment.SpecialFolder.Desktop :

return NativeMethods.CSIDL.CSIDL_DESKTOP;

case Environment.SpecialFolder.DesktopDirectory :

return NativeMethods.CSIDL.CSIDL_DESKTOPDIRECTORY;

case Environment.SpecialFolder.Favorites :

return NativeMethods.CSIDL.CSIDL_FAVORITES;

case Environment.SpecialFolder.History :

return NativeMethods.CSIDL.CSIDL_HISTORY;

case Environment.SpecialFolder.InternetCache :

return NativeMethods.CSIDL.CSIDL_INTERNET_CACHE;

case Environment.SpecialFolder.LocalApplicationData :

return NativeMethods.CSIDL.CSIDL_LOCAL_APPDATA;

case Environment.SpecialFolder.MyComputer :

return NativeMethods.CSIDL.CSIDL_DRIVES;

case Environment.SpecialFolder.MyMusic :

return NativeMethods.CSIDL.CSIDL_MYMUSIC;

case Environment.SpecialFolder.MyPictures :

return NativeMethods.CSIDL.CSIDL_MYPICTURES;

case Environment.SpecialFolder.Personal :

return NativeMethods.CSIDL.CSIDL_PERSONAL;

case Environment.SpecialFolder.ProgramFiles :

return NativeMethods.CSIDL.CSIDL_PROGRAM_FILES;

case Environment.SpecialFolder.Programs :

return NativeMethods.CSIDL.CSIDL_PROGRAMS;

case Environment.SpecialFolder.Recent :

return NativeMethods.CSIDL.CSIDL_RECENT;

case Environment.SpecialFolder.SendTo :

return NativeMethods.CSIDL.CSIDL_SENDTO;

case Environment.SpecialFolder.StartMenu :

return NativeMethods.CSIDL.CSIDL_STARTMENU;

case Environment.SpecialFolder.Startup :

return NativeMethods.CSIDL.CSIDL_STARTUP;

case Environment.SpecialFolder.System :

return NativeMethods.CSIDL.CSIDL_SYSTEM;

case Environment.SpecialFolder.Templates :

return NativeMethods.CSIDL.CSIDL_TEMPLATES;

} // End switch

return NativeMethods.CSIDL.CSIDL_DESKTOP;

}

public int CompareTo(object obj)
{

// Recover a reference to the CGPidl object.
CGPidl pidl = obj as CGPidl;

// Sanity check the type first.
if (pidl == null)
return 0;

// Ask the shell to do the compare.
return c_desktopFolder.CompareIDs(
NativeMethods.SHCIDS.SHCIDS_CANONICALONLY,
pidl.Pidl,
m_pidl
);

}

public object Clone()
{

// Clone the object.
CGPidl pidl = new CGPidl();
pidl.m_pidl = NativeMethods.ILClone(m_pidl);
pidl.m_displayName = m_displayName;
pidl.m_typeName = m_typeName;
pidl.m_attributes = m_attributes;
pidl.m_physicalPath = m_physicalPath;

// Return the clone.
return pidl;

}

}

}