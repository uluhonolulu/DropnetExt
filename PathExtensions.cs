using System.IO;
using System.Linq;

namespace ChpokkWeb.Infrastructure {
	public static class PathExtensions {
		[NotNull] 
		public static string AppendPathEvenIfItIsRooted(this string path, [NotNull] params string[] appended) {
			foreach (var next in appended) {
				var toAppend = next;
				if (Path.IsPathRooted(next)) {
					toAppend = next.Substring(Path.GetPathRoot(next).Length);
				}
				path = Path.Combine(path, toAppend?? string.Empty);
			}
			return path;
		}

		/// <summary>
		/// Gets filename from either url or physicl path
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[NotNull] 
		public static string GetFileNameUniversal([NotNull] this string path) {
			var parts = path.Split('/', '\\');
			return parts.Last();
		}

		[NotNull] 
		public static string RemoveExtension([NotNull] this string name) {
			return Path.GetFileNameWithoutExtension(name);
		}

		public static string RemoveFromStart(this string path, string root) {
			return path.Substring(root.Length);
		}
	}
}
