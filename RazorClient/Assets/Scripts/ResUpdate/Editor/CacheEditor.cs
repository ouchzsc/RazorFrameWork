using UnityEditor;
using UnityEngine;

namespace ResUpdate.Editor
{
    public static class CacheEditor
    {
        [MenuItem("Ouchz/Browse Cache")]
        public static void OpenCache()
        {
            System.Diagnostics.Process.Start("explorer.exe", Application.temporaryCachePath.Replace('/','\\'));
        }
    }
}