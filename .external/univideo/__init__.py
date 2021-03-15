import io
import struct
import subprocess
import os
import shutil
import glob
from tqdm import tqdm


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
    byte_size = os.path.getsize(os.path.join(base_dir,input_file_name))
    print('Starting..')
    with open(os.path.join(base_dir,input_file_name), "rb") as file, open(export_file_name, "w") as export_file, tqdm(total=byte_size) as pbar:
        while file.read(1):
            counter+=1
            file.seek(-1,1)
            # get first 4 bytes (im size in bytes)
            im_byte_size = int.from_bytes(file.read(4),byteorder='little')
            pbar.update(im_byte_size+4+8) # im+sizeheader+durationheader
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
    subprocess.call(f'ffmpeg -y -f concat -i export.txt -filter:v "vflip" "../{os.path.splitext(input_file_name)[0]}.mp4"', shell=True)
    os.chdir(base_dir)
    # delete temp folder.
    if os.path.isfile(os.path.join(base_dir,f'{os.path.splitext(input_file_name)[0]}.mp4')):
        shutil.rmtree(temp_folder)
        # delete binary file.
        os.remove(os.path.join(base_dir,input_file_name))
        return True
    else:
        return False