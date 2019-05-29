# File: s (Python 2.3)

import win32security
import win32api

def AdjustPrivilege(priv, enable = 1):
    """Adjust the current process's security privileges.
    Taken from the errata to the O'Reilly Python cookbook.
    """
    flags = win32security.TOKEN_ADJUST_PRIVILEGES | win32security.TOKEN_QUERY
    htoken = win32security.OpenProcessToken(win32api.GetCurrentProcess(), flags)
    id = win32security.LookupPrivilegeValue(None, priv)
    if enable:
        newPrivileges = [
            (id, win32security.SE_PRIVILEGE_ENABLED)]
    else:
        newPrivileges = [
            (id, 0)]
    
    try:
        return win32security.AdjustTokenPrivileges(htoken, 0, newPrivileges)
    except:
        return None


