# Cemu Update Tool
[![GitHub latest release](https://img.shields.io/github/release/Fs00/CemuUpdateTool)](https://github.com/Fs00/CemuUpdateTool/releases/latest)
[![GitHub downloads count](https://img.shields.io/github/downloads/Fs00/CemuUpdateTool/total)](https://github.com/Fs00/CemuUpdateTool/releases)
[![CodeFactor Grade](https://img.shields.io/codefactor/grade/github/Fs00/CemuUpdateTool/master)](https://www.codefactor.io/repository/github/Fs00/CemuUpdateTool)

This tool, written in C# using WinForms GUI library, has been designed to **update Cemu while keeping each version of the emulator in a separate folder**, in order to painlessly rollback to the previous one in case of regressions.  
To do this, the program allows the user to migrate data selectively from an older Cemu installation to a newer one with a single click, thanks to an intuitive user interface. It has also other features to automate repetitive tasks when updating, including:
- download and extraction of the latest version of Cemu before migrating data
- desktop shortcut creation (with optional parameter for custom MLC folder)
- compatibility options setting for Cemu executable (no full-screen optimizations, HiDPI scaling behavior, etc.)
- ability to update an existing Cemu installation in-place

The behavior of the tool is deeply customizable with a substantial number of options.  
To download it, head to Releases. **.NET Framework 4.5** is at least required for the program to work.

### Status
The project is not maintained anymore. Contributions to fix any bugs or to improve certain aspects of the application are always welcome.
