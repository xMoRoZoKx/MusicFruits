/*  
    SQU1KI(ENOT) СОЗДАТЕЛЬ ЭТОЙ ПАРАШИ
    КОД МОЖНО МЕНЯТЬ ПОД СЕБЯ,   РАЗРЕШАЮ
*/

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using System.Text;
using System.IO;
using UnityEngine;

namespace SceneLoader
{
    public partial class QuickSceneChanger
    {
        private const string PATH_TO_SCENES_FOLDER = "/Scenes/"; // Путь к папки со сценами
        private const string PATH_TO_OUTPUT_SCRIPT_FILE = "/_Scripts/EnotFolder/SceneList.cs"; // Вывод сцен в отдельный скрипт
        private const string ASSETS_SCENE_PATH = "Assets/Scenes/";

        [MenuItem("Tools/Generate Scene Load Menu Code")]
        public static void GenerateSceneLoadMenuCode()
        {
            AssetDatabase.Refresh();
            
            StringBuilder result = new StringBuilder();
            string basePath = Application.dataPath + PATH_TO_SCENES_FOLDER;

            AddClassHeader(result);
            AddCodeForDirectory(new DirectoryInfo(basePath), result);
            AddClassFooter(result);

            string scriptPath = Application.dataPath + PATH_TO_OUTPUT_SCRIPT_FILE;
            File.WriteAllText(scriptPath, result.ToString());

            void AddCodeForDirectory(DirectoryInfo directoryInfo, StringBuilder result)
            {
                FileInfo[] fileInfoList = directoryInfo.GetFiles();
                
                for (int i = 0; i < fileInfoList.Length; i++)
                {
                    FileInfo fileInfo = fileInfoList[i];

                    if (fileInfo.Extension == ".unity")
                        AddCodeForFile(fileInfo, result);
                }

                DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    AddCodeForDirectory(subDirectories[i], result);
                }

                void AddCodeForFile(FileInfo fileInfo, StringBuilder result)
                {
                    
                    string subPath = fileInfo.FullName.Replace('\\', '/').Replace(basePath, "");
                    string assetPath = ASSETS_SCENE_PATH + subPath;

                    string functionName = fileInfo.Name.Replace(".unity", "").Replace(" ", "").Replace("-", "");

                    result.Append("        [MenuItem(\"Scenes/")
                            .Append(subPath.Replace(".unity", ""))
                            .Append("\")]")
                            .Append(Environment.NewLine);

                    result.Append("        public static void Load").Append(functionName)
                            .Append("() { OpenScene(\"")
                            .Append(assetPath).Append("\"); }")
                            .Append(Environment.NewLine);
                }
            }
        }

        private static void OpenScene(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private static void AddClassHeader(StringBuilder result) // Добавляет using UnityEditor; и #if UNITY_EDITOR в SceneDropdowns
        {
            result.Append(@"using UnityEditor;

namespace SceneLoader
{
    public partial class QuickSceneChanger
    {
");
            result.Append(@"#if UNITY_EDITOR
");
        }

        private static void AddClassFooter(StringBuilder result) // Добавляет #endif в SceneDropdowns
        {
            result.Append(@"#endif
    }
}");
        }
    }
}
#endif