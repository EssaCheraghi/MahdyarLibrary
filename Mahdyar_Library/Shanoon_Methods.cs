using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mahdyar_Library
{
    public enum FileTypes
    {
        Image
        ,Text
        ,Video
        ,Sound
        ,Other
    };
    public static class Shanoon_Methods
    {
      public static class FileMethods
      {
          public static FileTypes GetFileType(string Filename)
          {
              switch (System.IO.Path.GetExtension(Filename).ToLower())
              {
                  case ".gif":
                  case ".jpeg":
                  case ".icon":
                  case ".wmf":
                  case ".emf":
                  case ".exif":
                  case ".bmp":
                  case ".tiff":
                  case ".png":
                  case ".jpg":return FileTypes.Image;
              }
              return FileTypes.Other;
          }
      }


      public static partial class ProcessMethods
      {
          public static bool Ext_ProcessExists(string name)
          {
              var a = Process.GetProcesses().Where(x => x.ProcessName.ToLower() == name.ToLower()).ToArray().Length;
              if (a == 0) return false;
              return true;
          }
      }

      public class DirectorySearcher
      {
          private Stack<int> entry;
          bool for_skip = false;

          void Starts(string root_directory,Action<string> DirectoyFound=null,Action<string> FileFound=null,Action SearchComplete=null)
          {
              if(!Directory.Exists(root_directory)) throw new DirectoryNotFoundException();

              string folder = root_directory;
              entry = new Stack<int>();

              int value = 0;
              entry.Push(Directory.GetDirectories(folder).Length);
              //if (count_children(folder) == 0) return;

              while (entry.Count > 0)
              {
                  int n = 0;
                  try
                  {
                      n = Directory.GetDirectories(folder).Length;
                  }
                  catch { }
                  if (n > value)
                  {
                      try
                      {
                          folder = Directory.GetDirectories(folder)[value];
                      }
                      catch
                      {
                      }
                      entry.Push(++value);
                      value = 0;
                  }
                  else
                  {
                      try
                      {
                          if (DirectoyFound != null) DirectoyFound.Invoke(folder);
                          for (int a = 0; a < Directory.GetFiles(folder).Length; a++)
                          {
                              if (FileFound != null) FileFound.Invoke(Directory.GetFiles(folder)[a]);
                              if (for_skip)
                              {
                                  for_skip = false;
                                  break;
                              }
                          }
                      }
                      catch
                      {
                      }
                      value = entry.Pop();
                      if (folder != root_directory)
                          try
                          {
                              folder = Directory.GetParent(folder).FullName;
                          }
                          catch
                          {
                          }
                  }
              }
              
          }

          public void Start(string root_directory, Action<string> DirectoyFound = null, Action<string> FileFound = null, Action SearchComplete = null)
          {
             var bgw=new BackgroundWorker();
             
              bgw.DoWork += (o, ea) =>
              {
                  Starts(root_directory, DirectoyFound, FileFound, SearchComplete);
              };
              bgw.RunWorkerCompleted += (o, ea) =>
              {
                  if (SearchComplete != null) SearchComplete.Invoke();
              };
              bgw.RunWorkerAsync();
          }

        
          
          public void skip_current_folder()
          {
              for_skip = true;
          }
      }
    }


}
