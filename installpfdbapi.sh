#!/usr/bin/bash

# Assume project directory root
# Make python virtual environment
echo "Creating Python virtual environment..."
python -m venv $PWD && echo "Created."

echo "Activating Python virtual environment..."
# Assume bash for now and activate the venv
source $PWD/bin/activate && echo "Activated."

echo "Installing Python dependencies..."
bin/python -m pip install tesseract opencv-python && echo "Installed."

make

cd ComponentTester && make all


#echo "export TESSBINPATH=

