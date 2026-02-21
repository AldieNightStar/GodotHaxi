#!/bin/env python3
import subprocess
import shutil
import sys
import os

ADDON_PATH = ("addons", "GodotHaxi")
IGNORE_PATTERNS = ("*.uid",)
GIT_MAIN_BRANCH = "main"

currentDir = os.path.dirname(os.path.realpath(__file__))
projectDir = os.path.abspath(os.path.join(currentDir, ".."))

def main(args: list):
    if len(args) < 1:
        print("Please enter Godot project directory to install addon")
        return
    if install(args[0]):
        print("OK")

def install(target_project: str):
    if not is_godot_dir(target_project):
        print("This directory is not Godot Project")
        return False

    pull_last()

    addons_dir1 = os.path.join(projectDir, *ADDON_PATH)
    addons_dir2 = os.path.join(target_project, *ADDON_PATH)

    if os.path.exists(addons_dir2):
        shutil.rmtree(addons_dir2)

    shutil.copytree(addons_dir1, addons_dir2, ignore=shutil.ignore_patterns(*IGNORE_PATTERNS))
    return True

def is_godot_dir(root: str):
    gd = os.path.join(root, "project.godot")
    return os.path.exists(gd)

def pull_last():
    subprocess.call(("git", "pull", "origin", GIT_MAIN_BRANCH), cwd=projectDir)

if __name__ == "__main__":
    main(sys.argv[1:])