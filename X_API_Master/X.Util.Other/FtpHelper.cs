using System;
using System.IO;
using System.Net;
using System.Reflection;
using X.Util.Core.Log;
using X.Util.Entities.Enums;

namespace X.Util.Other
{
    ///<summary>
    /// Ftp上传
    ///</summary>
    public class FtpHelper
    {
        /// <summary>
        /// GetFtpWebRequest
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="ftpUser"></param>
        /// <param name="ftpPassWord"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static FtpWebRequest GetFtpWebRequest(string uri, string ftpUser, string ftpPassWord, string method)
        {
            var ftp = (FtpWebRequest)WebRequest.Create(uri);
            ftp.Credentials = new NetworkCredential(ftpUser, ftpPassWord);
            ftp.Method = method;
            return ftp;
        } 

        /// <summary>
        /// 创建FTP目录，返回值是否创建成功
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="ftpUser">FTP的用户名</param>
        /// <param name="ftpPassWord">FTP的密码</param>
        private static bool CreateDirection(string uri, string ftpUser, string ftpPassWord)
        {
            var flag = true;
            try
            {
                var ftp = GetFtpWebRequest(uri, ftpUser, ftpPassWord, WebRequestMethods.Ftp.MakeDirectory);
                var response = (FtpWebResponse)ftp.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { uri, ftpUser, ftpPassWord }), ex, LogDomain.Util);
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 判断FTP上目录是否存在
        /// </summary>
        /// <param name="uri">FTP上的目录</param>
        /// <param name="ftpUser">FTP的用户名</param>
        /// <param name="ftpPassWord">FTP的密码</param>
        /// <returns></returns>
        private static bool FtpIsExistsFile(string uri, string ftpUser, string ftpPassWord)
        {
            var flag = true;
            try
            {
                var ftp = GetFtpWebRequest(uri, ftpUser, ftpPassWord, WebRequestMethods.Ftp.ListDirectory);
                var response = (FtpWebResponse)ftp.GetResponse();
                response.Close();
            }
            catch (Exception e)
            {
                flag = false;
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { uri, ftpUser, ftpPassWord }), e, LogDomain.Util);
            }
            return flag;
        }


        /// <summary>
        /// 上传文件到服务器
        /// </summary>
        /// <param name="hostName">服务器地址，如：ftp://192.168.1.101</param>
        /// <param name="fileName">上传的文件本地路径</param>
        /// <param name="uploadDir">上传到服务器的目录，如:ftp://192.168.1.101/Test </param>
        /// <param name="ftpUser">FTP的用户名</param>
        /// <param name="ftpPassWord">FTP的密码</param>
        /// <returns></returns>
        public static void UploadFile(string hostName, string fileName, string uploadDir, string ftpUser, string ftpPassWord)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(uploadDir)) return;
            if (!FtpIsExistsFile(uploadDir, ftpUser, ftpPassWord) && !CreateDirection(uploadDir, ftpUser, ftpPassWord)) return;
            var fileinfo = new FileInfo(fileName);
            var uri = uploadDir + fileinfo.Name;
            var ftp = GetFtpWebRequest(uri, ftpUser, ftpPassWord, WebRequestMethods.Ftp.UploadFile);
            ftp.KeepAlive = false;
            ftp.UseBinary = true;
            ftp.UsePassive = true;
            ftp.ContentLength = fileinfo.Length;
            const int bufferSize = 2048;
            var content = new byte[bufferSize];
            var fs = fileinfo.OpenRead();
            try
            {
                var rs = ftp.GetRequestStream();
                int dataRead;
                do
                {
                    dataRead = fs.Read(content, 0, bufferSize);
                    rs.Write(content, 0, dataRead);
                } while (!(dataRead < bufferSize));
                rs.Close();
            }
            catch (Exception ex)
            {
                Logger.Client.Error(Logger.Client.GetMethodInfo(MethodBase.GetCurrentMethod(), new object[] { hostName, fileName, uploadDir, ftpUser, ftpPassWord }), ex, LogDomain.Util);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
