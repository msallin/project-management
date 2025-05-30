namespace ProjectManagement.Core;

public interface IProjectVisitor
{
    void Start();

    void Visit(TaskNode node, int level);

    void End();
}
