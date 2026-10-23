#!/usr/bin/bash

# Assume project directory root
# Make python virtual environment
echo "Creating Python virtual environment..."
python -m venv $PWD/.venv --without-scm-ignore-files && echo "Created."

echo "Activating Python virtual environment..."
# Assume bash for now and activate the venv
source $PWD/bin/activate && echo "Activated."


if [ -z "${PFDB_PYTHON_PATH+x}" ]; then
    export PFDB_PYTHON_PATH=$PWD/.venv/bin # For current shell session
    echo "export PFDB_PYTHON_PATH=$PWD/.venv/bin #PFDB API Python path" | tee -a $HOME/.bashrc && source $HOME/.bashrc
fi

echo "Installing Python dependencies..."
$PFDB_PYTHON_PATH/python -m pip install pytesseract opencv-python && echo "Installed."

make

if [ -z "${PFDB_API_PATH+x}" ]; then
    export PFDB_API_PATH=$PWD # For current shell session
    echo "export PFDB_API_PATH=$PWD #PFDB API root path" | tee -a $HOME/.bashrc && source $HOME/.bashrc
fi

cd ComponentTester && make all

cd ..

if [ -z "${TESSBINPATH+x}" ]; then
    #Uncomment the next two lines if you have a desired place for your tessbin/ folder to live. This should be a place that isn't temporary.
    #export TESSBINPATH=/home/nephtalima/Programming # For current shell session
    #echo "export TESSBINPATH=/home/nephtalima/Programming # Directory which contains tessbin/ directory" | tee -a $HOME/.bashrc && source $HOME/.bashrc
fi

if [ -z "${PFDB_IMAGE_PATH+x}" ]; then
    #Uncomment the next two lines if you have a desired place for your image folder to live. This should be a place that isn't temporary.
    #export PFDB_IMAGE_PATH=/home/nephtalima/Programming/PFDB_Images # For current shell session
    #echo "export PFDB_IMAGE_PATH=/home/nephtalima/Programming/PFDB_Images # Directory which contains tessbin/ directory" | tee -a $HOME/.bashrc && source $HOME/.bashrc
fi

echo "Testing Python configuration. If successful, you should see a quote from one of my favourite people following:"
$PFDB_PYTHON_PATH/python ipfapi/impa.py