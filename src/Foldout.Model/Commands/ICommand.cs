namespace Foldout.Model.Commands
{
    public interface ICommand
    {
        void Do(Outline outline);
        ICommand GetReverseCommand();
    }
}
