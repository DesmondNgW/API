using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Reflection;
using X.Util.Core;
using X.Util.Core.Log;
using X.Util.Entities;

namespace X.Util.Other
{
    public class SharpZipHelper
    {
        /// <summary>
        /// 压缩
        /// </summary> 
        /// <param name="filename"> 压缩后的文件名(包含物理路径)</param>
        /// <param name="directory">待压缩的文件夹(包含物理路径)</param>
        public static void PackFiles(string filename, string directory)
        {
            try
            {
                var fz = new FastZip
                {
                    CreateEmptyDirectories = true
                };
                fz.CreateZip(filename, directory, true, string.Empty);
                fz = null;
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { filename, directory }), e, LogDomain.Util);
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="file">待解压文件名(包含物理路径)</param>
        /// <param name="dir"> 解压到哪个目录中(包含物理路径)</param>
        public static bool UnpackFiles(string file, string dir)
        {
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var s = new ZipInputStream(File.OpenRead(file));
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    var directoryName = Path.GetDirectoryName(theEntry.Name);
                    var fileName = Path.GetFileName(theEntry.Name);
                    if (!directoryName.IsNullOrEmpty()) Directory.CreateDirectory(dir + directoryName);
                    if (!fileName.IsNullOrEmpty())
                    {
                        var streamWriter = File.Create(dir + theEntry.Name);
                        var size = 2048;
                        var data = new byte[size];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
                s.Close();
                return true;
            }
            catch (Exception e)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { file, dir }), e, LogDomain.Util);
            }
            return false;
        }

        #region 私有方法
        /// <summary>
        /// 递归压缩文件夹方法
        /// </summary>
        private static bool ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName)
        {
            var res = true;
            string[] folders, filenames;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            try
            {
                var entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/"));
                s.PutNextEntry(entry);
                s.Flush();
                filenames = Directory.GetFiles(FolderToZip);
                foreach (var file in filenames)
                {
                    fs = File.OpenRead(file);
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/" + Path.GetFileName(file)))
                    {
                        DateTime = DateTime.Now,
                        Size = fs.Length
                    };
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                res = false;
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { FolderToZip, s, ParentFolderName }), e, LogDomain.Util);
            }
            finally
            {
                fs?.Close();
                GC.Collect();
                GC.Collect(1);
            }
            folders = Directory.GetDirectories(FolderToZip);
            foreach (string folder in folders)
            {
                if (!ZipFileDictory(folder, s, Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip))))
                {
                    return false;
                }
            }
            return res;
        }

        /// <summary>
        /// 压缩目录
        /// </summary>
        /// <param name="FolderToZip">待压缩的文件夹，全路径格式</param>
        /// <param name="ZipedFile">压缩后的文件名，全路径格式</param>
        private static bool ZipFileDictory(string FolderToZip, string ZipedFile, int level)
        {
            if (!Directory.Exists(FolderToZip)) return false;
            var s = new ZipOutputStream(File.Create(ZipedFile));
            s.SetLevel(level);
            var res = ZipFileDictory(FolderToZip, s, string.Empty);
            s.Finish();
            s.Close();
            return res;
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="FileToZip">要进行压缩的文件名</param>
        /// <param name="ZipedFile">压缩后生成的压缩文件名</param>
        private static bool ZipFile(string FileToZip, string ZipedFile, int level)
        {
            if (!File.Exists(FileToZip)) return false;
            FileStream ZipFile = null;
            ZipOutputStream ZipStream = null;
            bool res = true;
            try
            {
                ZipFile = File.OpenRead(FileToZip);
                var buffer = new byte[ZipFile.Length];
                ZipFile.Read(buffer, 0, buffer.Length);
                ZipFile.Close();
                ZipFile = File.Create(ZipedFile);
                ZipStream = new ZipOutputStream(ZipFile);
                ZipEntry ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                ZipStream.PutNextEntry(ZipEntry);
                ZipStream.SetLevel(level);
                ZipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (ZipStream != null)
                {
                    ZipStream.Finish();
                    ZipStream.Close();
                }
                ZipFile?.Close();
                GC.Collect();
                GC.Collect(1);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="FileToZip">待压缩的文件目录</param>
        /// <param name="ZipedFile">生成的目标文件</param>
        /// <param name="level">6</param>
        public static bool Zip(string FileToZip, string ZipedFile, int level)
        {
            if (Directory.Exists(FileToZip))
            {
                return ZipFileDictory(FileToZip, ZipedFile, level);
            }
            else if (File.Exists(FileToZip))
            {
                return ZipFile(FileToZip, ZipedFile, level);
            }
            else
            {
                return false;
            }
        }
    }
}
