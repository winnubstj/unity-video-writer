# unity-video-writer
Unity high performance video writer

## Install
1. Create anaconda environment from yml file:
    ```
    conda env create -f environment.yml
    ``` 
1. Install module from within repo directory:
    ```
    pip install -e .
    ```
1. Add unity-package to unity project with package manager (Window->Package Manager-> '+' sign -> add package from disk)
1. Create the video writer (Window->VideoWriter->CreateVideoWriter)
1. Select the VideoWriter in the inspector and change the outputfolder,framerate, and quality as needed.
1. Copy the 'Process Videos.bat' file to different location
1. Adjust file locations in the bat file as needed

## Process video files.
1. Run the bat file.

## Update
To update the anaconda environment run in repo directory:
```
conda env update --name univideo --file environment.yml
```