using UnityEditor;

namespace SceneLoader
{
    public partial class QuickSceneChanger
    {
#if UNITY_EDITOR
        [MenuItem("Scenes/GameScene")]
        public static void LoadGameScene() { OpenScene("Assets/Scenes/GameScene.unity"); }
        [MenuItem("Scenes/MenuScene")]
        public static void LoadMenuScene() { OpenScene("Assets/Scenes/MenuScene.unity"); }
#endif
    }
}