Imports System.ComponentModel
Public Enum MMSYSERR
    <Description("Success.")>
    SUCCESS = 0
    <Description("Unspecified error.")>
    UNKNOWNERROR = 1
    <Description("Device ID out of range.")>
    BADDEVICEID = 2
    <Description("Driver failed enable.")>
    NOTENABLED = 3
    <Description("Device already allocated.")>
    ALLOCATED = 4
    <Description("Device handle is invalid.")>
    INVALHANDLE = 5
    <Description("No device driver present.")>
    NODRIVER = 6
    <Description("Memory allocation error.")>
    NOMEM = 7
    <Description("Function isn't supported.")>
    NOTSUPPORTED = 8
    <Description("Error value out of range.")>
    BADERRNUM = 9
    <Description("Invalid flag passed.")>
    INVALFLAG = 1
    <Description("Invalid parameter passed.")>
    INVALPARAM = 11
    <Description("Handle being used simultaneously on another thread (eg callback).")>
    HANDLEBUSY = 12
    <Description("Specified alias not found.")>
    INVALIDALIAS = 13
    <Description("Bad registry database.")>
    BADDB = 14
    <Description("Registry key not found.")>
    KEYNOTFOUND = 15
    <Description("Registry read error.")>
    READERROR = 16
    <Description("Registry write error.")>
    WRITEERROR = 17
    <Description("Registry delete error.")>
    DELETEERROR = 18
    <Description("Registry value not found.")>
    VALNOTFOUND = 19
    <Description("Driver does not call DriverCallback.")>
    NODRIVERCB = 20
    <Description("Last error in range.")>
    LASTERROR = 20
End Enum
