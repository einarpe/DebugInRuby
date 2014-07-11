using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace info.kubarek.RbDbg
{
  public class RbInOut
  {
    StreamReader stdout = null;
    StreamWriter stdin = null;
    StreamReader stderr = null;

    event DataReceivedEventHandler OnOutputDataReceived;
    event EventHandler OnExit;

    Process p = null;

    public void Initialize(string rubyExecutable, string rubyFileToDebug)
    {
      if (!File.Exists(rubyFileToDebug))
        throw new ArgumentException("File with Ruby source code does not exist");

      if (!File.Exists(rubyExecutable))
        throw new ArgumentException("Ruby executable does not exist");

      ProcessStartInfo psi = new ProcessStartInfo()
      {
        FileName = rubyExecutable,
        Arguments = "-rdebug " + rubyFileToDebug
      };
      p = Process.Start(psi);

      stdout = p.StandardOutput;
      stdin = p.StandardInput;
      stderr = p.StandardError;

      p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
      p.Exited += new EventHandler(p_Exited);
    }

    void p_Exited(object sender, EventArgs e)
    {
      if (OnExit != null)
        OnExit(sender, e);
    }

    void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
      if (OnOutputDataReceived != null)
        OnOutputDataReceived(sender, e);
    }
  }
}
