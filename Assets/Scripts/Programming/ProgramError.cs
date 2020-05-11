public class ProgramError
{
    public bool HasError = false;
    public string Message = "";

    public ProgramError(){}

    public ProgramError(bool hasError, string message)
    {
        HasError = hasError;
        Message = message;
    }
}
