namespace X.Util.Entities.Interface
{
    public interface IModule
    {
        bool CreateModule(Module module);

        Module GetModule(string moduleFullName);
    }
}
