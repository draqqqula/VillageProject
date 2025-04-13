using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class WorkEventSource : IWorkEventSource<WorkResult>
{
    public event Action<WorkResult> OnFinished;

    public void Finish(WorkResult result)
    {
        OnFinished?.Invoke(result);
    }
}
