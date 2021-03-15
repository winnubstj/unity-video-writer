set root=C:\Anaconda

call %root%\Scripts\activate.bat %root%
call conda activate univideo
call python C:\\repos\\unity-video-writer\\univideo\\process_folder.py C:\\temp
pause