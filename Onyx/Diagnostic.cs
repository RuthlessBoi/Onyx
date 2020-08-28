using Onyx.Text;

namespace Onyx
{
    public sealed class Diagnostic
    {
        public bool IsError { get; }
        public TextLocation Location { get; }
        public string Message { get; }
        public bool IsWarning { get; }

        private Diagnostic(bool isError, TextLocation location, string message)
        {
            IsError = isError;
            Location = location;
            Message = message;
            IsWarning = !IsError;
        }

        public override string ToString() => Message;

        public static Diagnostic Error(TextLocation location, string message) => new Diagnostic(isError: true, location, message);
        public static Diagnostic Warning(TextLocation location, string message) => new Diagnostic(isError: false, location, message);
    }
}
