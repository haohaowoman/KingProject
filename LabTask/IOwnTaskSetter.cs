using LabMCESystem.LabElement;

namespace LabMCESystem.Task
{
    public interface IOwnTaskSetter
    {
        TaskSetter Setter { get; set; }
    }
}