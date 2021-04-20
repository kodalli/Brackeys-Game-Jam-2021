using UnityEngine;

using UnityEditor;
using UnityEditor.SceneManagement;

using System.Collections.Generic;
using System.IO;

// Scene selection

class sceneWindow : EditorWindow {

	[MenuItem("Window/Scenes")]
	public static void ShowWindow() {

		EditorWindow.GetWindow(typeof(sceneWindow), false, "Scenes");

	}

	int selected; int changedSelected;
	List<string> scenes = new List<string>();

	void OnGUI() {

		EditorGUILayout.Space();

		scenes.Clear();

		string[] dropOptions = new string[EditorBuildSettings.scenes.Length];

		for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {

			EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];

			string sceneName = Path.GetFileNameWithoutExtension(scene.path);

			scenes.Add(sceneName);

			dropOptions[i] = scenes[i];

			if (scene.path == EditorSceneManager.GetActiveScene().path) {

				selected = changedSelected = i;

			}

		}

		// Selection

		changedSelected = EditorGUILayout.Popup(selected, dropOptions);

		if (selected != changedSelected) {

			selected = changedSelected;

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(EditorBuildSettings.scenes[changedSelected].path);

		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Scenes", EditorStyles.boldLabel);

		for (int i = 0; i < scenes.Count; i++) {

			if (GUILayout.Button(scenes[i], GUILayout.Height(20))) {

				selected = changedSelected = i;

				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(EditorBuildSettings.scenes[i].path);

			}

			dropOptions[i] = scenes[i];

		}

	}

}