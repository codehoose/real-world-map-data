using System.IO;
using UnityEditor;
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

public class SettingsSerializer
{
    private string _assetName;
    private string _relativePath;
    private string _absolutePath;
    
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="assetName">The name of the settings file.</param>
    public SettingsSerializer(string assetName)
    {
        _relativePath = "Assets/Resources/Editor/" + assetName;
        _absolutePath = Path.Combine(Application.dataPath, "Resources/Editor");
        _assetName = assetName;
    }

    /// <summary>
    /// Read the map import settings.
    /// </summary>
    /// <returns>The map import settings.</returns>
    public MapImportSettings Read()
    {
        // Create the "_absolutePath" folder if it does not exist
        if (!Directory.Exists(_absolutePath))
            Directory.CreateDirectory(_absolutePath);

        // Try to load the map import settings from the file
        var mapImportSettings = AssetDatabase.LoadAssetAtPath<MapImportSettings>(_relativePath);

        // If the file exists, return the instance to the caller
        if (mapImportSettings != null)
            return mapImportSettings;
        
        // Create a new asset and save it to disk in the "Resources/Editor" folder
        mapImportSettings = ScriptableObject.CreateInstance<MapImportSettings>();
        AssetDatabase.CreateAsset(mapImportSettings, _relativePath);

        // Return that new asset to the caller
        return mapImportSettings;
    }

    public void Save(MapImportSettings settings)
    {
        AssetDatabase.SaveAssets();
    }
}

