using System;

namespace CodeImp.DoomBuilder.UDBScript
{
	class ProgressInfo
	{
		IProgress<int> progress;
		IProgress<string> status;
		IProgress<string> _log;

		public ProgressInfo(IProgress<int> progress, IProgress<string> status, IProgress<string> log)
		{
			this.progress = progress;
			this.status = status;
			_log = log;
		}

		public void setProgress(int p)
		{
			progress.Report(p);
		}
		
		public void setStatus(string s)
		{
			status.Report(s);
		}

		public void log(string s)
		{
			_log.Report(s);
		}
	}
}
