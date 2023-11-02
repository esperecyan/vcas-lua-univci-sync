using System;
using System.IO;
using System.Threading;
using UnityEditor;
using VCI;

namespace Esperecyan.VCasLuaUniVCISync
{
    internal static class Sync
    {
        private static readonly string SourceFolderPath
            = @"%USERPROFILE%\AppData\LocalLow\VirtualCast\VirtualCast\EmbeddedScriptWorkspace";

        private static SynchronizationContext MainThreadContext;
        private static string SourceFolderFullPath;
        private static FileSystemWatcher Watcher;

        [InitializeOnLoadMethod]
        private static void Run()
        {
            if (Sync.Watcher != null)
            {
                return;
            }

            Sync.MainThreadContext = SynchronizationContext.Current;

            Sync.SourceFolderFullPath = Environment.ExpandEnvironmentVariables(Sync.SourceFolderPath);

            Sync.Watcher = new FileSystemWatcher()
            {
                Path = Sync.SourceFolderFullPath,
                Filter = "*.lua",
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
            };
            Sync.Watcher.Changed += Sync.Watcher_Changed;
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            var parentFolderPath = Path.GetDirectoryName(e.FullPath);
            if (Path.GetDirectoryName(parentFolderPath) != Sync.SourceFolderFullPath)
            {
                return;
            }

            var vciName = Path.GetFileName(parentFolderPath);
            Sync.MainThreadContext.Post(_ =>
            {
                foreach (var guid in AssetDatabase.FindAssets(vciName + " t:Prefab", new[] { "Assets" }))
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);

                    if (Path.GetFileNameWithoutExtension(path) != vciName)
                    {
                        continue;
                    }

                    var vciObject = AssetDatabase.LoadAssetAtPath<VCIObject>(path);
                    if (vciObject == null)
                    {
                        continue;
                    }

                    var scriptName = Path.GetFileNameWithoutExtension(e.FullPath);
                    var script = vciObject.Scripts.Find(s => s.name == scriptName);
                    if (script == null || script.filePath == e.FullPath)
                    {
                        break;
                    }

                    if (script.textAsset != null)
                    {
                        var luaAssetPath = AssetDatabase.GetAssetPath(script.textAsset);
                        File.WriteAllText(Path.GetFullPath(luaAssetPath), File.ReadAllText(e.FullPath));
                        AssetDatabase.ImportAsset(luaAssetPath);
                    }
                    else if (!string.IsNullOrEmpty(script.filePath))
                    {
                        File.Copy(sourceFileName: e.FullPath, destFileName: script.filePath, overwrite: true);
                    }
                    else
                    {
                        script.source = File.ReadAllText(e.FullPath);
                    }

                    break;
                }
            }, state: null);
        }
    }
}
