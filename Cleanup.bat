@ECHO OFF
echo This script will delete all Temporary Files left behind by unclean Visual Studio shutdowns.
echo It will also cleanup the windows system droppings (like thumbs.db)
pause
del /S *.TMP
del /S /ASH Thumbs.db
del /S *.bak