using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OneDrive.Tools
{
    public static class FileNameTool
    {
        /// <summary>
        /// 获取有效的文件或文件夹名称
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static string GetValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return "unkown name";
            }
            string vaildFileName = "";
            if (fileName.Length > 110)
            {
                vaildFileName = fileName.Substring(0, 110);
            }
            else
            {
                vaildFileName = fileName;
            }
            //去除特殊符号
            string illegalPathString = Regex.Replace(vaildFileName, @"[\\\\\\?\\/\\*\\|<>:\\\""\a\b\f\n\r\t\v\0�]", "");
            StringBuilder builder = new StringBuilder();
            try
            {
                char[] invalidChars = Path.GetInvalidFileNameChars();
                for (int i = 0; i < illegalPathString.Length; i++)
                {
                    char pathChar = illegalPathString[i];
                    int charInt = (int)pathChar;
                    if (charInt == 65533 || charInt == 127)
                    {
                        continue;
                    }
                    bool isAppend = true;
                    foreach (var item in invalidChars)
                    {
                        if (item == pathChar)
                        {
                            isAppend = false;
                            break;
                        }
                    }
                    if (isAppend)
                    {
                        builder.Append(illegalPathString[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogLib.Log.WriteLog(string.Format("StackTrace:{0}---Message:{1}", ex.StackTrace, ex.Message));
            }
            string resultString = builder.ToString();
            if (!string.IsNullOrEmpty(resultString))
            {
                resultString = resultString.Replace("AUX", "");
                resultString = resultString.Replace("AUx", "");
                resultString = resultString.Replace("AuX", "");
                resultString = resultString.Replace("aUX", "");
                resultString = resultString.Replace("aUx", "");
                resultString = resultString.Replace("aux", "");
                resultString = resultString.Replace(",", "");
            }
            return resultString;
        }

    }
}
