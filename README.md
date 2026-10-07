# PFDB_API

A project that aims to have a treasure trove of statistics from Phantom Forces and compile it into a database that is available for people to view.

## Installation

**(Note that the following instructions may not work as I am constantly changing the project. If you have any problems, make an issue and I'll (hopefully) look into it.)**

1. Download .NET 8 SDK and ASP.NET 8 SDK
2. Download Python 3.14+ (earliest version)
3. Download Tesseract (For Linux, install the package using your package manager of choice. For Windows, download a binary with the training data and executable and point to it at runtime, and hopefully PyTesseract will handle it)
4. Download source code
5. Set up Python virtual environment in the root directory of this project
6. Download opencv-python and pytesseract with pip in the virtual environment

I am planning on making a pre-compiled thing once I get this to a minimum state of having the API and CLI commands work.

## Structure of the project

### Calculator

This is an older thing that can convert between rank and XP. It is currently not used (subject to change)

### ComponentTester

Main entry point of the CLI application. This handles building, testing and a few more things. Currently I have testing working, but I need to work on the building.

### Conversion

Middle part of the project that defines conversions.

### FileParse

Parses the files that come from PyExec. This is separate from PyExec so that the building isn't necessary to parse the outputs; you only need to build the files once.

Does some automatic cleanup of statistics and builds statistic collections.

### Frontend

Defines the frontend (API) part of the application.

### ImageParseForAPI (ipfapi)

Python script that reads the images. Uses PyTesseract and OpenCV.

### Logger

Handles logging used throughout the application.

### ManualProofread

Handles the manual proofreading that the autoproofreading wasn't able to handle.

### p_invokewrapper

Connects Calculator to C#.

### PFDB_SS

ScreenShot tool to systematically capture the images from PF.

### PyExec

Handles executing the Python script.

### SQLiteHandler

Handles all of the weapon data and storing it into databases.

### StatisticStructure

Defines statistics that are used in FileParse.

### WeaponStructure

High-level definitions for weapons, categories and classes.

### WeaponUtility2

Defines fundamental objects such as PhantomForcesVersion and WeaponIdentification.

