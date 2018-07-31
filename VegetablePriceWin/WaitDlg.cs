using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Common
{
    public class WaitDlg
    {
        public static void Show(ContainerControl mainDlg, Action codeToRun)
        {
            ManualResetEvent dialogLoadedFlag = new ManualResetEvent(false);

            // open the dialog on a new thread so that the dialog window gets
            // drawn. otherwise our long running code will run and the dialog
            // window never renders.
            (new Thread(() =>
            {
                Form waitDialog = new Form()
                {
                    Name = "WaitForm",
                    Text = "Please Wait...",
                    ControlBox = false,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    Width = 240,
                    Height = 80,
                    Enabled = true
                };

                ProgressBar ScrollingBar = new ProgressBar()
                {
                    Style = ProgressBarStyle.Marquee,
                    Parent = waitDialog,
                    Dock = DockStyle.Fill,
                    Enabled = true
                };

                waitDialog.Load += new EventHandler((x, y) =>
                {
                    dialogLoadedFlag.Set();
                });

                waitDialog.Shown += new EventHandler((x, y) =>
                {
                    // note: if codeToRun function takes a while it will 
                    // block this dialog thread and the loading indicator won't draw
                    // so launch it too in a different thread
                    (new Thread(() =>
                    {
                        codeToRun();

                        // after that code completes, kill the wait dialog which will unblock 
                        // the main thread
                        mainDlg.Invoke((MethodInvoker)(() => waitDialog.Close()));
                    })).Start();
                });

                mainDlg.Invoke((MethodInvoker)(() => waitDialog.ShowDialog(mainDlg)));
            })).Start();

            while (dialogLoadedFlag.WaitOne(100, true) == false)
                Application.DoEvents(); // note: this will block the main thread once the wait dialog shows
        }
    }


}
