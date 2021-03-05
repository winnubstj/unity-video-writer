import io
import struct
import subprocess
import os
import shutil
import glob

def process_folder(base_dir):
    base_folder = 'C:/temp'
    file_list = glob.glob(f'{os.path.join(base_folder,"*.univideo")}')
    print(f'Found {len(file_list)} univideo files in {base_folder}\n')
    for counter,file in enumerate(file_list):
        print(f'Processing: {os.path.basename(file)} ({counter} of {len(file_list)})')
        process_univideo(file)

def process_univideo(input_file):
    base_dir = os.path.dirname(input_file)
    print(base_dir)
    input_file_name = os.path.basename(input_file)
    print(input_file_name)
    # Create temp folder.
    temp_folder = os.path.join(base_dir,'temp')
    if not os.path.isdir(temp_folder):
        os.mkdir(temp_folder)
    counter = 0
    export_file_name = os.path.join(temp_folder,"export.txt") # stores frame file names and duration
    with open(os.path.join(base_dir,input_file_name), "rb") as file, open(export_file_name, "w") as export_file:
        while file.read(1):
            counter+=1
            file.seek(-1,1)
            # get first 4 bytes (im size in bytes)
            im_byte_size = int.from_bytes(file.read(4),byteorder='little')
            # get jpeg.
            im_bytes = file.read(im_byte_size)
            if file.read(1):
                file.seek(-1,1)
                output_file_name = f'frame_{counter}.jpeg'
                # get duration of frame
                im_duration = struct.unpack('d',file.read(8))[0]
                # write export settings.
                export_file.write(f'file \'{output_file_name}\'\n' )
                export_file.write(f'duration {im_duration}\n' )
                # write jpeg file.
                with open(os.path.join(temp_folder,output_file_name), "wb") as output_file:
                    output_file.write(im_bytes)
    # run ffmpeg.
    os.chdir(temp_folder)
    subprocess.call(f'ffmpeg -y -f concat -i export.txt -filter:v "vflip" ../{os.path.splitext(input_file_name)[0]}.mp4', shell=True)
    os.chdir(base_dir)
    # delete temp folder.
    shutil.rmtree(temp_folder)
    # delete binary file.
    os.remove(os.path.join(base_dir,input_file_name))