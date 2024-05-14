using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Managers
{
    public class MeshManager : MonoBehaviour
    {
        public static MeshManager Instance;

        //private Dictionary<string, GameObject> meshes;
        private Dictionary<string, Dictionary<string, GameObject>> meshes;

        private void OnEnable()
        {
            Instance = this;

            LoadMeshes();
        }

        void LoadMeshes()
        {
            meshes = new Dictionary<string, Dictionary<string, GameObject>>();

            LoadMeshesFromDirectory("Assets/Graphics/Meshes");
        }

        void LoadMeshesFromDirectory(string path)
        {
            string[] subDirs = Directory.GetDirectories(path);
            foreach (var dir in subDirs)
            {
                LoadMeshesFromDirectory(dir);
            }

            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                string directoryName = Path.GetDirectoryName(file);
                var prefix = "Assets" + Path.DirectorySeparatorChar + "Graphics" + Path.DirectorySeparatorChar + "Meshes" +
                             Path.DirectorySeparatorChar;
                directoryName = directoryName.Replace(prefix, "");

                if (directoryName.Contains("Meshes"))
                    break;
                if (Path.GetExtension(file).Equals(".mat") || Path.GetExtension(file).Equals(".meta"))
                    continue;
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(file);

                

                if (!meshes.ContainsKey(directoryName))
                {
                    meshes[directoryName] = new Dictionary<string, GameObject>();
                }
                
                var mesh_go = Instantiate(prefab, transform, true);
                mesh_go.SetActive(false);
                
                meshes[directoryName].Add(Path.GetFileNameWithoutExtension(file), mesh_go);
            }
        }

        public GameObject GetMesh(string raceName, string meshName)
        {
            return meshes[raceName][meshName];
        }
    }
}