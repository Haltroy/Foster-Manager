import os, shutil
# Script to remove bin and obj folders for mutli-OS workstations
# (such as mine with W1indows 10 & Arch Linux) 
# This script removes all bin and obj file in every single project
# inside this solution so OmniSharp won't go crazy with package
# locations (ex. searching in C:\ in Linux machines)
# and making it impossible to see errors in Visual Studio

def removeFolder(rootFolder):
    subfolders = [ f.path for f in os.scandir(rootFolder) if f.is_dir() ]
    print("Removing all bin and obj folders in " + rootFolder + "...")
    for dir in subfolders:
        # Detect and skip the other folders
        if ("packages" in dir) or (".vs" in dir) or (".git" in dir) or (".github" in dir) or (".vscode" in dir):
            continue
        binDir = os.path.join(dir,"bin");
        objDir = os.path.join(dir,"obj");
        pubDir = os.path.join(dir,"publish");
        # bin
        try:
            if os.path.exists(binDir):
                print("Remove folder: " + binDir)
                shutil.rmtree(binDir)
        except OSError as e:
            print("Error: %s - %s." % (e.filename, e.strerror))
        #obj
        try:
            if os.path.exists(objDir):
                print("Remove folder: " + objDir)
                shutil.rmtree(objDir)
        except OSError as e:
            print("Error: %s - %s." % (e.filename, e.strerror))
        #publish
        try:
            if os.path.exists(pubDir):
                print("Remove folder: " + pubDir)
                shutil.rmtree(pubDir)
        except OSError as e:
            print("Error: %s - %s." % (e.filename, e.strerror))
        removeFolder(dir)
       
removeFolder(os.getcwd())
print("Done.")