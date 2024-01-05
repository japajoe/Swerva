using System;
using System.IO;
using System.Threading.Tasks;

namespace Swerva
{
    /// <summary>
    /// Helper class to load and cache page content
    /// </summary>
    public class HttpPageContent
    {
        public string Path { get; set; }
        public string Content  { get; set; }
        public FileTimeInfo FileInfo { get; set; }

        public HttpPageContent(string path)
        {
            this.Path = path;
            this.Content = string.Empty;
            this.FileInfo = new FileTimeInfo();
        }

        public async Task<bool> LoadAsync()
        {
            if(!File.Exists(Path))
                return false;

            var info = new FileTimeInfo(new FileInfo(Path));
            
            if(FileInfo.LastWriteTime != info.CreationTime)
            {
                Content = await File.ReadAllTextAsync(Path);
                FileInfo = info;
            }

            return true;
        }

        public bool Load()
        {
            if(!File.Exists(Path))
                return false;

            var info = new FileTimeInfo(new FileInfo(Path));
            
            if(FileInfo.LastWriteTime != info.CreationTime)
            {
                Content = File.ReadAllText(Path);
                FileInfo = info;
            }

            return true;
        }        
    }

    public struct FileTimeInfo
    {
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime LastWriteTime { get; set; }

        public FileTimeInfo()
        {
            this.CreationTime = DateTime.Now;
            this.LastAccessTime = DateTime.Now;
            this.LastWriteTime = DateTime.Now;
        }

        public FileTimeInfo(FileInfo info)
        {
            this.CreationTime = info.CreationTime;
            this.LastAccessTime = info.LastAccessTime;
            this.LastWriteTime = info.LastWriteTime;
        }
    }
}