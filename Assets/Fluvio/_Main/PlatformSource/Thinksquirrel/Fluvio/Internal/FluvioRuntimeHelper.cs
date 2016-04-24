using UnityEngine;

namespace Thinksquirrel.Fluvio.Internal
{
    using Threading;

    // This class sets up implementations of certain platform-specific features.
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    class FluvioRuntimeHelper : MonoBehaviour
    {
        void OnEnable()
        {
            #if UNITY_EDITOR
            // Create settings object in the editor
            var serializedInstance = Resources.Load("FluvioManager", typeof (FluvioSettings)) as FluvioSettings;
            if (!serializedInstance)
            {
                var instance = FluvioSettings.GetFluvioSettingsObject();
                serializedInstance = CreateProjectSettingsAsset(instance, "Resources", "FluvioManager.asset");                
            }
            FluvioSettings.SetFluvioSettingsObject(serializedInstance);
            FluvioComputeShader.SetIncludeParser(new ComputeIncludeParser());
            #endif

            // OpenCL support
            #if UNITY_STANDALONE// || UNITY_ANDROID // TODO - Android support for OpenCL is WIP
            Cloo.Bindings.CLInterface.SetInterface(new Cloo.Bindings.CL12());
            #endif

            // Multithreading
            #if !UNITY_WEBGL && !UNITY_WINRT
            Parallel.InitializeThreadPool(new FluvioThreadPool());
            #endif
        }

#if UNITY_EDITOR
        static T CreateProjectSettingsAsset<T>(T obj, string folder, string fileName) where T : Object
        {
            string path;

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                path = Application.dataPath.Replace("/", "\\") + "\\Fluvio-ProjectSettings\\" + folder;
            }
            else
            {
                path = Application.dataPath + "/Fluvio-ProjectSettings/" + folder;
            }
            System.IO.Directory.CreateDirectory(path);

            var path2 = "Assets/Fluvio-ProjectSettings/" + folder + "/" + fileName;

            var currentObj = UnityEditor.AssetDatabase.LoadAssetAtPath(path2, typeof(T)) as T;
            if (currentObj)
            {
                UnityEditor.EditorUtility.CopySerialized(obj, currentObj);
                UnityEditor.AssetDatabase.Refresh();
            }
            else
            {
                UnityEditor.AssetDatabase.CreateAsset(obj, path2);
                UnityEditor.AssetDatabase.Refresh();
                currentObj = obj;
            }

            return currentObj;
        }
#endif    
    }
}
