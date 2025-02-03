#!/bin/bash

handle_error() {
    echo "Error on line $1"
    exit 1
}

trap 'handle_error $LINENO' ERR


pip install -r requirements.txt

python benchmark.py
pytest test

python -m unittest discover -s test -p "*_test.py"