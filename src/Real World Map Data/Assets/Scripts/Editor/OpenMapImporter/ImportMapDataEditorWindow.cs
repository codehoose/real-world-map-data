using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Threading;
using System.Collections.Generic;

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

public class ImportMapDataEditorWindow : EditorWindow
{
    private Material _roadMaterial;
    private Material _buildingMaterial;
    private string _mapFilePath = "None (Choose OpenMap File)";
    private string _progressText;
    private float _progress;
    private bool _disableUI;
    private bool _validFile;
    private bool _importing;

    private object _lock = new object();
    private List<ActionMessage> _actions = new List<ActionMessage>();

    [MenuItem("Window/Import OpenMap Data")]
    public static void ShowEditorWindow()
    {
        var window = GetWindow<ImportMapDataEditorWindow>();
        window.titleContent = new GUIContent("Import OpenMap");
        window.Show();
    }
    
    internal void StartImport()
    {
        ResetProgress();

        AddMessage(new ActionMessage(() =>
        {
            _importing = true;
        }));
    }

    internal void EndImport()
    {
        AddMessage(new ActionMessage(() =>
        {
            _importing = false;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }));
    }

    internal void AddMessage(ActionMessage message)
    {
        lock(_lock)
        {
            _actions.Add(message);
        }
    }

    public void ResetProgress()
    {
        AddMessage(new ActionMessage(() =>
        {
            _progress = 0f;
            _progressText = "";
        }));
    }

    public void UpdateProgress(float progress, string progressText, bool done)
    {
        AddMessage(new ActionMessage(() =>
        {
            if (done)
            	EditorUtility.ClearProgressBar();
            else
                EditorUtility.DisplayProgressBar("Importing Map",
                                     string.Format("{0} {1:%}", progressText, progress), 
                                     progress);
        }));
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(_mapFilePath);
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("..."))
        {
            var filePath = EditorUtility.OpenFilePanel("Select OpenMap File",
                                                       Application.dataPath,
                                                       "txt");
            if (filePath.Length > 0)
                _mapFilePath = filePath;

            _validFile = _mapFilePath.Length > 0;
        }

        EditorGUILayout.EndHorizontal();

        _roadMaterial = EditorGUILayout.ObjectField("Road Material",
                                                    _roadMaterial,
                                                    typeof(Material),
                                                    false) as Material;
        _buildingMaterial = EditorGUILayout.ObjectField("Building Material",
                                                        _buildingMaterial,
                                                        typeof(Material),
                                                        false) as Material;

        EditorGUI.BeginDisabledGroup(!_validFile || _disableUI || _importing);
        if (GUILayout.Button("Import Map File"))
        {
            ThreadPool.QueueUserWorkItem(_ => {

                var mapWrapper = new ImportMapWrapper(this,
                                                      _mapFilePath,
                                                      _roadMaterial,
                                                      _buildingMaterial);

                mapWrapper.Import();
            });
        }

        EditorGUI.EndDisabledGroup();

        if (_disableUI)
        {
            EditorGUILayout.HelpBox("The current scene has not been saved yet!",
                                    MessageType.Warning,
                                    true);
        }
    }

    private void Update()
    {
        _disableUI = EditorSceneManager.GetActiveScene().isDirty;

        lock(_lock)
        {
            foreach (var action in _actions)
            {
                action.Invoke();
            }

            if (_actions.Count > 0)
                Repaint();

            _actions.Clear();
        }
    }
}
