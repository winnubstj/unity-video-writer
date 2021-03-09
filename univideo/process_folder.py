import io
import struct
import subprocess
import os
import sys
import shutil
import glob
from univideo import process_univideo

def process_folder(base_folder):
    file_list = glob.glob(f'{os.path.join(base_folder,"*.univideo")}')
    print(f'Found {len(file_list)} univideo files in {base_folder}\n')
    for counter,file in enumerate(file_list):
        print(f'Processing: {os.path.basename(file)} ({counter+1} of {len(file_list)})')
        process_univideo(file)

if __name__ == "__main__":
    process_folder(sys.argv[1])