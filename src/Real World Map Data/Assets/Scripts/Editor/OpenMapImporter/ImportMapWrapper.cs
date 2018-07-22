using System;
using UnityEngine;

/*
    Copyright (c) 2018 Sloan Kelly

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

internal sealed class ImportMapWrapper
{
    private ImportMapDataEditorWindow _window;
    private string _mapFile;
    private Material _roadMaterial;
    private Material _buildingMaterial;

    public ImportMapWrapper(ImportMapDataEditorWindow window, 
                            string mapFile, 
                            Material roadMaterial, 
                            Material buildingMaterial)
    {
        _window = window;
        _mapFile = mapFile;
        _roadMaterial = roadMaterial;
        _buildingMaterial = buildingMaterial;
    }

    public void AddAction(Action action)
    {
        _window.AddMessage(new ActionMessage(action));
    }

    public void Import()
    {
        _window.StartImport();

        var mapReader = new MapReader();
        mapReader.Read(_mapFile);

        var buildingMaker = new BuildingMaker(mapReader, _buildingMaterial);
        var roadMaker = new RoadMaker(mapReader, _roadMaterial);

        Process(buildingMaker, "Importing buildings");
        Process(roadMaker, "Importing roads");

        _window.EndImport();
    }

    private void Process(BaseInfrastructureMaker maker, string progressText)
    {
        float nodeCount = maker.NodeCount;
        var progress = 0f;

        foreach (var node in maker.Process(this))
        {
            progress = node / nodeCount;
            _window.UpdateProgress(progress, progressText, false);
        }
        _window.UpdateProgress(0, string.Empty, true);
    }
}
