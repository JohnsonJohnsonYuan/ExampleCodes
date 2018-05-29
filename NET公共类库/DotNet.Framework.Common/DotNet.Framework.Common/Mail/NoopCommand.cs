﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace DotNet.Framework.Common.Mail
{
    /// <summary>
    /// This class represents the Pop3 NOOP command.
    /// </summary>
    internal sealed class NoopCommand : Pop3Command<Pop3Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoopCommand"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public NoopCommand(Stream stream)
            : base(stream, false, Pop3State.Transaction) { }

        /// <summary>
        /// Creates the NOOP request message.
        /// </summary>
        /// <returns>
        /// The byte[] containing the NOOP request message.
        /// </returns>
        protected override byte[] CreateRequestMessage()
        {
            return GetRequestMessage(Pop3Commands.Noop);
        }
    }
}
