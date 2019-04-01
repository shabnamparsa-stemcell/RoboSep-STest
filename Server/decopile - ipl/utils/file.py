import os
import re
from ipl.utils.path import path

def findMatchingDirs(dirRE, startDir = '.', reFlags = re.IGNORECASE):
    """Returns a list of directories underneath the startDir directory that
    match the supplied regular expression. reFlags sets regexp flags; by 
    default, we do case-insensitive searches."""
    pattern = re.compile(dirRE, reFlags)
    return filter(pattern.search, path(startDir).walkdirs())


                                       
def findMatchingFiles(fileRE, searchPath = '.', reFlags = re.IGNORECASE):
    """Returns a list of files in the searchDir directory that match the 
    supplied regular expression. reFlags sets regexp flags; by default, we do
    case-insensitive searches."""
    pattern = re.compile(fileRE, reFlags)
    return filter(pattern.search, path(searchPath).files())



def getFileList(searchPath, extMask = None):
    """Return a list of files in the specified path. If the extMask is set,
    then only files matching that extension are returned."""
    fileList = []
    files = os.listdir(searchPath)
    for entry in files:
        file = os.path.join(searchPath, entry)
        if os.path.isfile(file):
            ext = os.path.splitext(file)[1].replace('.', '')
            if ((extMask == None) or (ext == extMask)):
                fileList.append(file)

    return fileList


					 
def getFreeDiskSpace(path):
    """Return the free disk space (in bytes) for the specified path, from
    which we get the disk drive to look at."""
    drive = os.path.splitdrive(path)[0]
    f = os.popen(('dir /-C %s' % drive), 'r')
    data = f.readlines()
    f.close()
    return int(data[-1].split()[2].replace(',', ''))



def createDir(newdir):
    """creates a dir under these conditions if it doesn't already exist as
    either a matching directory or a file name"""
    if os.path.isdir(newdir):
        pass
    elif os.path.isfile(newdir):
        print "createDir error - file name conflicts with dir name"
    else:
        head, tail = os.path.split(newdir)
        if head and not os.path.isdir(head):
            _mkdir(head)
        print "createDir %s" % repr(newdir)
        if tail:
            os.mkdir(newdir)