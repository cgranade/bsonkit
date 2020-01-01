// Copyright ⓒ Christopher Granade.
// Licensed under the MIT License.

using System.CommandLine;
using System.CommandLine.Invocation;

namespace BsonKit
{
    public static class Extensions
    {
        public static Command WithHandler(this Command command, ICommandHandler handler)
        {
            command.Handler = handler;
            return command;
        }
    }
}
