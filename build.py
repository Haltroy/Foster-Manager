#Copyright © 2021 - 2022 haltroy

#Use of this source code is governed by a GNU General Public License version 3.0 that can be found in github.com/haltroy/Foster/blob/master/COPYING

import os, sys, subprocess, shutil
if sys.version_info < (3, 5):
    print('Please upgrade your Python version to 3.5 or higher')
    sys.exit()
rids = ['linux-x64', 'linux-musl-x64','linux-arm','linux-arm64', 'win-x64','win-x86','win-arm','win-arm64','osx-x64',]
rootFolder = os.getcwd()
publishDir = os.path.join(rootFolder,"publish")

if not("--skip-folder-deletion" in sys.argv) and not("-s" in sys.argv):
    try:
        shutil.rmtree(publishDir)
    except OSError as e:
        print("Error on deleting publish directory - " + e.strerror)
    quickBuild = ("q" in sys.argv) or ("quick" in sys.argv)


def buildrid(rid):
    ridDir = os.path.join(publishDir,rid)
    cmd = ['dotnet', 'publish', '--self-contained', '-f','netcoreapp3.1' ,'-v','d','-r',rid,'-c','Release','-o', ridDir]
    try:
        print("Building for platform '" + rid + "'...")
        result = subprocess.run(cmd, stdout=subprocess.PIPE, check=True)
        print("Done building for platform '" + rid + "'.")
    except Exception as ex:
        print(result.stdout.decode('utf-8'))
        print("Error on platform '" + rid + "', exception caught: " + str(ex))

if "only" in sys.argv:
    onlyPos = sys.argv.index("only")
    onlyrids = sys.argv[onlyPos + 1:]
    for onlyrid in onlyrids:
        buildrid(onlyrid)
elif "o" in sys.argv:
    onlyPos = sys.argv.index("o")
    onlyrids = sys.argv[onlyPos + 1:]
    for onlyrid in onlyrids:
        buildrid(onlyrid)
else:
    print("Welcome to auto-build process of Foster Manager!")
    print("This script will build a self-contained app for all available platforms.")
    if not quickBuild:
        print("This script will ask for you to press ENTER to continue.")
        print("The reason for this is that if an error shows up, you can easily see it.")
        print("To bypass this, run this script with 'q' or 'quick' option.")
    for rid in rids:
        if not quickBuild:
            input("Press Enter to build for '" + rid + "'...")
        buildrid(rid)
    print("Done building all.")