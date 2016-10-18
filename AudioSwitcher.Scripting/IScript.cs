namespace AudioSwitcher.Scripting
{
    public interface IScript
    {

        string Name
        {
            get;
        }

        string Body
        {
            get;
        }

        string MediaType
        {
            get;
        }

    }
}