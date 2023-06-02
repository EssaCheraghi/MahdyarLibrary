using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mahdyar_Library.Classes
{
    public class DirectorySearcher
    {
        private Stack<int> entry;
        bool for_skip = false;
        public event Action<string> Evt_File;
        public event Action<string> Evt_Directory;
        public event Action Evt_Complete;
        BackgroundWorker bgw = new BackgroundWorker();

        void Starts(string root_directory)
        {
            if (!Directory.Exists(root_directory)) throw new DirectoryNotFoundException();

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
                   // try{
                        Evt_Directory?.Invoke(folder);
                        for (int a = 0; a < Directory.GetFiles(folder).Length; a++)
                        {
                            Evt_File?.Invoke(Directory.GetFiles(folder)[a]);
                            if (for_skip)
                            {
                                for_skip = false;
                                break;
                            }
                        }
                    //}
                    //catch
                    //{
                    //}
                    value = entry.Pop();
                    if (folder != root_directory)
                        //try{
                            folder = Directory.GetParent(folder).FullName;
                        //}catch{ }
                }
                if(bgw.CancellationPending) break;
            }
        }

        public void Start(string root)
        {
            bgw = new BackgroundWorker();
            bgw.WorkerSupportsCancellation = true;
            
            bgw.DoWork += (o, ea) =>
            {
                Starts(root);
            };
            bgw.RunWorkerCompleted += (o, ea) =>
            {
                Evt_Complete?.Invoke();
            };
            bgw.RunWorkerAsync();
        }

        public void Stop()
        {
            bgw.CancelAsync();
        }

        public bool IsBusy => bgw.IsBusy;

        public void SkipCurrentFolder()
        {
            for_skip = true;
        }
    }
}
