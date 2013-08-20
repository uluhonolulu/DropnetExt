using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DropNet;
using DropNet.Models;
using FubuCore;
using ChpokkWeb.Infrastructure;

namespace ChpokkWeb.Features.Remotes.Dropbox {
	public static class DropnetExtensions {
		public static IEnumerable<MetaData> GetMetadataRecursive(MetaData metaData) {
			var result = new List<MetaData>() {metaData};
			if (metaData.Contents != null) {
				foreach (var data in metaData.Contents) {
					result.AddRange(GetMetadataRecursive(data));
				}				
			}
			return result;
		}

		/// <summary>
		/// Get all remote folder names recursively
		/// </summary>
		/// <param name="client"></param>
		/// <param name="path">Parent path, defaults to "/"</param>
		/// <returns></returns>
		public static IEnumerable<string> GetFoldernames(this DropNetClient client, string path = "/") {
			var metaData = client.GetMetaData(path);
			var folders = from meta in GetMetadataRecursive(metaData)
			              where meta.Is_Dir && !meta.Is_Deleted
			              select meta.Path;
			return folders;
		}

		/// <summary>
		/// Get all remote file names recursively
		/// </summary>
		/// <param name="client"></param>
		/// <param name="path">Parent path, defaults to "/"</param>
		/// <returns></returns>
		public static IEnumerable<string> GetFilenames(this DropNetClient client, string path = "/") {
			var metaData = client.GetMetaData(path);
			var folders = from meta in GetMetadataRecursive(metaData)
			              where !meta.Is_Dir && !meta.Is_Deleted
			              select meta.Path;
			return folders;
		}

		/// <summary>
		/// Download a file from Dropbox
		/// </summary>
		/// <param name="client">Dropnet client</param>
		/// <param name="remotePath">The file's path on Dropbox (use forward slashes for directory separators)</param>
		/// <param name="localPath">The target path</param>
		public static void DownloadFile(this DropNetClient client, string remotePath, string localPath) {
			var bytes = client.GetFile(remotePath);
			var targetFolder = localPath.ParentDirectory();
			if (!Directory.Exists(targetFolder)) {
				Directory.CreateDirectory(targetFolder);
			}
			File.WriteAllBytes(localPath, bytes);
		}

		/// <summary>
		/// Downloads a folder from Dropbox
		/// </summary>
		/// <param name="client"></param>
		/// <param name="remoteFolder">The path to the source folder.  </param>
		/// <param name="localFolder"></param>
		/// <param name="includeSelf">If false, only the contents of the source folder are downloaded, not the source folder itself. If true, the source folder is downloaded into the target folder.</param>
		public static void DownloadFolder(this DropNetClient client, string remoteFolder, string localFolder, bool includeSelf) {
			foreach (var remoteFile in client.GetFilenames(remoteFolder)) {
				var parent = includeSelf ? remoteFolder.ParentDirectory() : remoteFolder;
				var relativeRemotePath = remoteFile.RemoveFromStart(parent);
				var localFilePath = localFolder.AppendPathEvenIfItIsRooted(relativeRemotePath.Replace('/', Path.DirectorySeparatorChar));
				client.DownloadFile(remoteFile, localFilePath);
			}
		}

		/// <summary>
		/// Upload a file
		/// </summary>
		/// <param name="client"></param>
		/// <param name="localPath">Local filename</param>
		/// <param name="remotePath">Filename on Dropbox</param>
		public static void UploadFile(this DropNetClient client, string localPath, string remotePath) {
			var path = remotePath.ParentDirectory();
			var fileName = remotePath.GetFileNameUniversal();
			client.UploadFile(path, fileName, File.ReadAllBytes(localPath));
		}

		/// <summary>
		/// Upload a folder
		/// </summary>
		/// <param name="client"></param>
		/// <param name="localFolder">Source</param>
		/// <param name="remoteFolder">Destination</param>
		/// <param name="includeSelf">If false, only the contents of the source folder are uploaded, not the source folder itself. If true, the source folder is uploaded into the target folder.</param>
		public static void UploadFolder(this DropNetClient client, string localFolder, string remoteFolder, bool includeSelf) {
			foreach (var localPath in Directory.GetFiles(localFolder, "*.*", SearchOption.AllDirectories)) {
				var parent = includeSelf ? localFolder.ParentDirectory() : localFolder;
				var relativeLocalPath = localPath.PathRelativeTo(parent);
				var relativeRemotePath = relativeLocalPath.Replace(Path.DirectorySeparatorChar, '/');
				var remotePath = remoteFolder.AppendPath(relativeRemotePath).Replace(Path.DirectorySeparatorChar, '/');
				client.UploadFile(localPath, remotePath);
			}
		}
	}
}