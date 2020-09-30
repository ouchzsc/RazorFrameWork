using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GenResCsv : EditorWindow
{
    [MenuItem("Ouchz/GenResCsv")]
    public static void Gen()
    {
        string[] allbundles = AssetDatabase.GetAllAssetBundleNames();
        List<string> lines = new List<string>();
        foreach (var bundleName in allbundles)
        {
            string[] assetPaths =
                AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);

            foreach (var astPath in assetPaths)
            {
                string assetName = Path.GetFileNameWithoutExtension(astPath);
                StringBuilder lineBuilder = new StringBuilder();
                lineBuilder.Append($"{astPath},{bundleName},{assetName},{Path.GetExtension(astPath)}");
                lines.Add(lineBuilder.ToString());
            }
        }

        string fileName = Path.Combine(System.Environment.CurrentDirectory, "csv","asset", "asset.csv");
        using (var stream = File.OpenWrite(fileName))
        {
            StreamWriter sw = new StreamWriter(stream, Encoding.GetEncoding(936));
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            sw.WriteLine("##assetPath,bundleName,assetName,type");            
            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }

            sw.Close();
        }

        Debug.Log($"Generate Assets.csv successfully at path {fileName}");
    }
}