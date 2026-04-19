// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Handles the generation and packaging of distribution-ready assets.
    /// This utility automates the creation of C# initializer code from templates, 
    /// manages temporary file operations for Zip archiving (including scene cleaning 
    /// via <see cref="DemoKitSceneUtils"/>), and exports internal code as Unity Packages.
    /// It also ensures that hidden system files and metadata are correctly handled or 
    /// excluded during the packaging process.
    /// </summary>
    internal static class DemoDistributionUtils
    {
        private const string initializerTemplateGuid = "5dfa209db522c274985e9bd8b7a70331";

        internal static readonly string InternalCheckClassName = typeof(DemoOrchestrator).FullName;
        internal static readonly string EditorCheckClassName = typeof(DemoKitEditorUtils).FullName;


        internal static bool GenerateAutoInitializerCode(DemoSetupInfo sInfo)
        {
            try
            {
                var templateFilePath = Path.GetFullPath(DemoKitEditorUtils.GetAssetPath(initializerTemplateGuid)).Replace("\\", "/");
                var content = File.ReadAllText(templateFilePath);

                
                content = DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
                {
                    return content
                        .Replace("##DEMO_KIT_NAME##", DemoKitUtils.PackageName)
                        .Replace("##CT_SITE_URL##", DemoKitUtils.WebsiteUrl)
                        .Replace("##CT_GITHUB_URL##", DemoKitUtils.GitHubUrl)
                        .Replace("##CT_ASSET_STORE_URL##", DemoKitUtils.AssetStoreUrl)
                        .Replace("##EDITOR_CHECK_GUID##", DemoKitUtils.PresetGUID.EditorFolder)
                        .Replace("##IMPORT_CHECK_GUID##", DemoKitUtils.PresetGUID.DemoOrchestratorGuid)
                        .Replace("##DEMO_SCENE_FILE_NAME##", sInfo.DemoMainSceneFileName)
                        .Replace("##CHECK_CLASS_NAME##", InternalCheckClassName)
                        .Replace("##CLS_NAME##", Path.GetFileNameWithoutExtension(sInfo.DistributionDemoInitializerCodeFileName))
                        .Replace("##DEMO_NAME##", sInfo.DisplayDemoName)
                        .Replace("##ZIP_FILE_NAME##", sInfo.DistributionAssetsZipFileName)
                        .Replace("##UNITY_PACKAGE_FILE_NAME##", DemoKitUtils.DefaultValues.DistributionInternalCodePackageName);
                });

                File.WriteAllText(
                    sInfo.DistributionDemoInitializerCodeFullPath,
                    content
                );

                AssetDatabase.Refresh();
                
                return true;
            }
            catch (System.Exception ex)
            {
                DemoKitLog.Exception(ex);
                return false;
            }
        }


        private const string _tempPrefix = "temp_";

        internal static bool GenerateZipArchiveOfDistributionFolder(DemoSetupInfo sInfo, string tempPath)
        {
            var copiedSceneFileName = $"{_tempPrefix}{sInfo.DemoMainSceneFileName}";
            var copiedSceneFileFullPath = DemoKitPaths.GetCombinedPath(sInfo.workspaceFolderPath, copiedSceneFileName);
            var copiedSceneFileRelativePath = DemoKitPaths.GetCombinedPath(sInfo.RelativeWorkspaceFolderPath, copiedSceneFileName);

            bool copyOk;

            try
            {
                AssetDatabase.StartAssetEditing();

                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(copiedSceneFileRelativePath) != null)
                {
                    AssetDatabase.DeleteAsset(copiedSceneFileRelativePath);
                    AssetDatabase.SaveAssets();
                }

                copyOk = AssetDatabase.CopyAsset(sInfo.RelativeWorkspaceDemoMainScenePath, copiedSceneFileRelativePath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            if (copyOk)
            {
                AssetDatabase.ImportAsset(copiedSceneFileRelativePath, ImportAssetOptions.ForceUpdate);
            }

            if (copyOk)
            {
                DemoKitSceneUtils.EditScene(
                    copiedSceneFileRelativePath,
                    scene =>
                    {
                        DemoKitSceneUtils.RemoveEventSystemFromScene(scene);
                    }
                );
            }

            AssetDatabase.SaveAssets();

            try
            {
                var tempDi = new DirectoryInfo(tempPath);

                if (!tempDi.Exists)
                {
                    tempDi.Create();
                }

                var tempSceneFilePath = DemoKitPaths.GetCombinedPath(tempPath, sInfo.DemoMainSceneFileName);

                File.Move(copiedSceneFileFullPath, tempSceneFilePath);
                File.Move($"{copiedSceneFileFullPath}.meta", $"{tempSceneFilePath}.meta");

                var workspaceDi = new DirectoryInfo(sInfo.workspaceFolderPath);

                var dirs = workspaceDi
                    .GetDirectories("*", SearchOption.AllDirectories)
                    .Where(di => !di.Name.StartsWith("."))
                    .ToArray();
                
                foreach (var di in dirs)
                {
                    var rPath = Path.GetRelativePath(sInfo.workspaceFolderPath, di.FullName);
                    var tPath = DemoKitPaths.GetCombinedPath(tempPath, rPath);
                    if (!Directory.Exists(tPath))
                    {
                        Directory.CreateDirectory(tPath);
                    }
                }

                var files = DemoDistributionUtils.GetFileInfoToArchive(
                    sInfo.workspaceFolderPath,
                    new string[] { sInfo.DemoMainSceneFileName, $"{sInfo.DemoMainSceneFileName}.meta" }
                );

                foreach (var fi in files)
                {
                    var rPath = Path.GetRelativePath(sInfo.workspaceFolderPath, fi.FullName);
                    fi.CopyTo(DemoKitPaths.GetCombinedPath(tempPath, rPath), true);
                }

                var destZipFileFullPath = sInfo.DistributionAssetsZipFileFullPath;
                if (File.Exists(destZipFileFullPath))
                {
                    File.Delete(destZipFileFullPath);
                }

                ZipFile.CreateFromDirectory(tempPath, destZipFileFullPath);

                return true;
            }
            catch (Exception ex) when (ex is not ExitGUIException)
            {
                DemoKitLog.Exception(ex);
                return false;
            }
            finally
            {
                if (Directory.Exists(tempPath))
                {
                    Directory.Delete(tempPath, true);
                }

                AssetDatabase.Refresh();
            }
        }


        private static readonly object[] PresetExcludeFileNamePatterns =
        {
            //  hidden file and folder
            new Regex(@"^\."),

            // for windows
            new Regex("Thumbs.db", RegexOptions.IgnoreCase),
        };

        private static bool IsMatchExcludeFile(string fileName, IEnumerable<object> patterns)
        {
            foreach (var p in patterns)
            {
                if (p is string s && fileName == s)
                {
                    return true;
                }
                
                if (p is Regex regex && regex.IsMatch(fileName))
                {
                    return true;
                }
            }
            return false;
        }

        internal static List<FileInfo> GetFileInfoToArchive(string folderPath, string[] excludeFileNames = null)
        {
            var excludePatterns = PresetExcludeFileNamePatterns.ToList();
            if (excludeFileNames != null)
            {
                excludePatterns.AddRange(excludeFileNames);
            }

            var di = new DirectoryInfo(folderPath);

            if (!di.Exists)
            {
                return null;
            }

            var res = new List<FileInfo>();

            foreach (var file in di.GetFiles("*.*", SearchOption.AllDirectories))
            {
                var fName = file.Name;

                if (IsMatchExcludeFile(fName, excludePatterns))
                {
                    continue;
                }
                
                res.Add(file);
            }

            return res;
        }

        
        internal static bool ExportDistributionAssets(string srcFolderPath, string destZipFileFullPath, string[] excludeFileNames = null)
        {
            var tempPath = DemoKitPaths.GenerateTempFolderFullPath();

            var tempDi = new DirectoryInfo(tempPath);

            if (tempDi.Exists)
            {
                tempDi.Delete(true);
            }

            tempDi.Create();

            try
            {
                var files = GetFileInfoToArchive(srcFolderPath, excludeFileNames);
                foreach (var f in files)
                {
                    var rPath = Path.GetRelativePath(srcFolderPath, f.FullName);
                    var zipTempPath = DemoKitPaths.GetCombinedPath(tempPath, rPath);

                    var zipFi = new FileInfo(zipTempPath);

                    if (!Directory.Exists(zipFi.DirectoryName))
                    {
                        Directory.CreateDirectory(zipFi.DirectoryName);
                    }

                    f.CopyTo(zipFi.FullName);
                }

                if (File.Exists(destZipFileFullPath))
                {
                    File.Delete(destZipFileFullPath);
                }

                ZipFile.CreateFromDirectory(tempPath, destZipFileFullPath);
            }
            catch (System.Exception ex)
            {
                DemoKitLog.Exception(ex);
            }
            finally
            {
                if (tempDi.Exists)
                {
                    tempDi.Delete(true);
                }
            }

            return false;
        }

        internal static bool ArchiveDistributionFolder(string archiveFolderFullPath, string destinationFullPath)
        {
            try
            {
                if (File.Exists(destinationFullPath))
                {
                    File.Delete(destinationFullPath);
                }

                return ExportDistributionAssets(archiveFolderFullPath, destinationFullPath);
            }
            catch (System.Exception ex)
            {
                DemoKitLog.Exception(ex);
                return false;
            }
        }

        internal static bool ExportInternalCodePackage(string destFullPath)
        {
            if (string.IsNullOrEmpty(destFullPath))
            {
                return false;
            }
            
            try
            {
                var internalFolderPath = DemoKitEditorUtils.GetAssetPath(DemoKitUtils.PresetGUID.InternalFolder);

                AssetDatabase.ExportPackage(
                    internalFolderPath,
                    destFullPath,
                    ExportPackageOptions.Recurse
                );
                
                return true;
            }
            catch (System.Exception ex)
            {
                DemoKitLog.Exception(ex);
                return false;
            }
            
        }
    }
}