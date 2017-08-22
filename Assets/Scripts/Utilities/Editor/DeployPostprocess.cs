using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Text;

public class DeployPostProcess  {

	public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
	{
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);
		DirectoryInfo[] dirs = dir.GetDirectories();

		if (!dir.Exists)
		{
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDirName);
		}

		if (!Directory.Exists(destDirName))
		{
			Directory.CreateDirectory(destDirName);
		}

		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files)
		{
			string temppath = Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, true);
		}

		if (copySubDirs)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, copySubDirs);
			}
		}
	}
	private static void DeleteFileIfExists(string path)
	{
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}
	private static bool IsTargetPlatformSupported(BuildTarget target)
	{
		return target == BuildTarget.StandaloneWindows64;
	}

	static void CopyFolderToData( string pathToBuiltProject,string name)
	{
		var outDir = Path.GetDirectoryName(pathToBuiltProject);
		var projName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
		var resourcesFolder = name;
		string outputDir = outDir+"/"+projName+"_Data/"+name;

		var inDir = Path.Combine(Application.dataPath, resourcesFolder);

		if (!Directory.Exists(inDir))
		{
			resourcesFolder = Path.Combine("..", resourcesFolder);
			inDir = Path.Combine(Application.dataPath, resourcesFolder);
		}
		DirectoryCopy(inDir.ToString(), outputDir.ToString(), true);
	}
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		if (!IsTargetPlatformSupported(target))
		{
			Debug.Log("Trying to build TxKit"
				+ " for Unsupported target (" + target + ")!");
			return;
		}
		CopyFolderToData(pathToBuiltProject,"Data/");

		//var targetPath = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), "ovrvisionpro_conf.xml");
		//File.Copy("Assets/ovrvisionpro_conf.xml", targetPath, true);
	}

}
