# 2023-11-24
Moved the default branch from `master` to `main`. If you fix a bug, please branch off main and do a pull request. Thanks!

# Real World Map Data in Unity
Data from Open Street Map (http://openstreetmap.org) can be imported into Unity at run-time or design-time using the scripts from the projects in this repository.

Both scripts require you to export map data (a .osm file) and rename it to a .txt file. In the case of the run-time version of the scripts you will usually place these .txt files in the "Resources" folder. For the design-time version you can store them on your local hard disk.

## src/Read World Map Data
A design-time set of scripts to import Open Street Map data.

## src/Scene Based Real World Map Data
A run-time set of scripts to build a world during the run of a game.
