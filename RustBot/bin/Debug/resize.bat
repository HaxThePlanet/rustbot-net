@echo off
set "source_folder=C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images"
set "result_folder_1=C:\Users\bob\Documents\TrainYourOwnYOLO\Data\Source_Images\Test_Images\out"

for %%a in ("%source_folder%\*png") do (
   call resize.bat -source "%%~fa" -target "%result_folder_1%\%%~nxa" -max-height 1000 -max-width 1000 -keep-ratio yes -force yes
)

pause