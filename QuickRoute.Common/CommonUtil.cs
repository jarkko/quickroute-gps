﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace QuickRoute.Common
{
  public class CommonUtil
  {
    public static string GetApplicationDataPath()
    {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuickRoute");
    }

    public static string GetTempFileName()
    {
      return GetTempFileName(null);
    }

    public static string GetTempFileName(string extension)
    {
      if (extension == null) extension = "tmp";
      var path = GetTempPath();
      string fileName;
      do
      {
        fileName = path + DateTime.Now.Ticks + "_" + new Random().Next(0, 999999) + "." + extension;
      } while (File.Exists(fileName));
      return fileName;
    }

    public static string GetTempFileName(string suggestedFileName, string extension)
    {
      var path = GetTempPath();
      string fileName = path + CreateValidFileName(GetTempFileNameHelper(suggestedFileName, extension));
      var count = 0;
      while (File.Exists(fileName))
      {
        count++;
        var fn = Path.GetFileNameWithoutExtension(suggestedFileName) + string.Format(" ({0})", count) +
                 Path.GetExtension(suggestedFileName);
        fileName = path + CreateValidFileName(GetTempFileNameHelper(fn, extension));
      }
      return fileName;
    }

    private static string GetTempFileNameHelper(string fileName, string extension)
    {
      return extension != null
               ? Path.GetFileNameWithoutExtension(fileName) + "." + extension
               : Path.GetFileName(fileName);
    }


    /// <summary>
    /// Includes a trailing \
    /// </summary>
    /// <returns></returns>
    public static string GetTempPath()
    {
      return Path.Combine(GetApplicationDataPath(), "Temp");
    }

    public static void ClearTempFolder()
    {
      var dir = new DirectoryInfo(GetTempPath());
      var files = dir.GetFiles("*.*");
      foreach (var file in files)
      {
        file.Delete();
      }
      var directories = dir.GetDirectories("*.*");
      foreach (var directory in directories)
      {
        directory.Delete(true);
      }
    }

    public static string GetDownloadedFileName(string fileName)
    {
      // todo: add gui for download meter, cancelable
      if (fileName.StartsWith("http"))
      {
        var client = new WebClient();
        var downloadedFileName = GetTempFileName(fileName, null);
        client.DownloadFile(fileName, downloadedFileName);
        return downloadedFileName;
      }
      return fileName;
    }    

    public static string CreateValidFileName(string fileName)
    {
      string validFileName = fileName.Trim();

      foreach (char invalChar in Path.GetInvalidFileNameChars())
      {
        validFileName = validFileName.Replace(invalChar.ToString(), "");
      }
      foreach (char invalChar in Path.GetInvalidPathChars())
      {
        validFileName = validFileName.Replace(invalChar.ToString(), "");
      }
      return validFileName;
    }

  }
}
