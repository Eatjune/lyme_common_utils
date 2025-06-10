#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LymeGame.Utils.Common {
	/// <summary>
	/// 默认打开场景
	/// </summary>
	public class SceneOpenWindow : EditorWindow {
		private static readonly string m_filePath = "/SceneCfg.dat";
		private readonly string m_defaultScenePath = "Assets/Launcher.unity";
		private static string m_openScene;
		private static SceneAsset m_sceneAsset;
		private static bool m_isOpen = true;

		private SceneAsset SceneAsset {
			get => m_sceneAsset;
			set {
				if (m_sceneAsset == value) return;
				m_sceneAsset = value;
				var path = m_defaultScenePath;
				if (m_sceneAsset != null) path = AssetDatabase.GetAssetOrScenePath(m_sceneAsset);
				SetPlayModeStartScene(path);
			}
		}

		private bool IsOpen {
			get => m_isOpen;
			set {
				if (m_isOpen == value) return;
				m_isOpen = value;
				if (!value) {
					EditorSceneManager.playModeStartScene = null;
					SaveSceneOpen();
				} else if (!string.IsNullOrEmpty(m_openScene)) SetPlayModeStartScene(m_openScene);
			}
		}

		protected void OnGUI() {
			EditorGUILayout.LabelField(new GUIContent("在下方直接拖入需要默认开启的场景"));
			SceneAsset = (SceneAsset) EditorGUILayout.ObjectField(new GUIContent("默认开启场景"), SceneAsset, typeof(SceneAsset), false);
			IsOpen = EditorGUILayout.Toggle("默认开启", IsOpen);
		}

		protected void SetPlayModeStartScene(string scenePath) {
			var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
			if (sceneAsset != null) {
				EditorSceneManager.playModeStartScene = sceneAsset;
				SaveSceneName(scenePath);
			} else Debug.Log("Could not find Scene " + scenePath);
		}

		[MenuItem("Tools/LymeGame/SetStartScene")]
		protected static void Open() {
			LoadConfig();
			GetWindow<SceneOpenWindow>();
		}

		// 第一次打开 Unity 编辑器运行一次，之后每次进入 Play 模式都运行一次
		[InitializeOnLoadMethod]
		private static void InitOnLoad() {
			LoadConfig();
			if (m_sceneAsset) {
				if (m_isOpen) {
					EditorSceneManager.playModeStartScene = m_sceneAsset;
					Debug.Log($"读取默认场景:{m_openScene}");
				} else EditorSceneManager.playModeStartScene = null;
			} else {
				Debug.Log("读取默认场景失败:Could not find Scene " + m_openScene);
				m_openScene = "None";
			}
		}

		private static void SaveSceneName(string scenePath) {
			if (string.IsNullOrEmpty(scenePath)) return;
			var path = Application.persistentDataPath + m_filePath;
			if (File.Exists(path)) {
				File.WriteAllText(path, "");
			}

			m_openScene = scenePath;
			FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
			var bytes = System.Text.Encoding.Default.GetBytes(scenePath + "_" + m_isOpen);

			fs.Seek(0, SeekOrigin.Begin);
			fs.Write(bytes, 0, bytes.Length);
			fs.Flush();
			fs.Close();
			fs.Dispose();
		}

		private static void SaveSceneOpen() {
			if (string.IsNullOrEmpty(m_openScene)) return;

			FileStream fs = new FileStream(Application.persistentDataPath + m_filePath, FileMode.OpenOrCreate);
			var bytes = System.Text.Encoding.Default.GetBytes(m_openScene + "_" + m_isOpen);

			fs.Seek(0, SeekOrigin.Begin);
			fs.Write(bytes, 0, bytes.Length);
			fs.Flush();
			fs.Close();
			fs.Dispose();
		}

		private static void LoadConfig() {
			m_isOpen = true;
			var filePath = Application.persistentDataPath + m_filePath;
			if (File.Exists(filePath)) {
				var content = File.ReadAllText(filePath);
				var stringArr = content.Split('_');
				if (stringArr.Length != 2) {
					File.Delete(filePath);
					return;
				}

				var scenePath = stringArr[0];
				var sceneOpen = stringArr[1];
				m_openScene = scenePath;
				m_sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
				m_isOpen = Convert.ToBoolean(sceneOpen);
			} else m_openScene = "None";
		}
	}
}
#endif